#include <ntifs.h>
#include "infinityhook.h"
#include "undoc.h"
#include "Zydis.h"
#include <wchar.h>
#include <stdarg.h>
#include <stdio.h>

CHAR IsImmersiveBrokerFuncOrignalBytes[7] = { 0 };
ULONG_PTR IsImmersiveBrokerFuncAddr = NULL;

CHAR IAMThreadAccessGrantedFuncOrignalBytes[24] = { 0 };
ULONG_PTR IAMThreadAccessGrantedFuncAddr = NULL;

CHAR NtUserEnableIAMAccessFuncOrignalBytes[24] = { 0 };
ULONG_PTR NtUserEnableIAMAccessFuncAddr = NULL;

static int g_enable = TRUE;

NTSTATUS Overwrite(PVOID Address, PVOID Data, ULONG Size) {
	PHYSICAL_ADDRESS PhysAddress = MmGetPhysicalAddress(Address);
	PVOID MappedAddress = MmMapIoSpace(PhysAddress, Size, MmNonCached);

	if (MappedAddress == NULL)
		return STATUS_INSUFFICIENT_RESOURCES;

	RtlCopyMemory(MappedAddress, Data, Size);
	MmUnmapIoSpace(MappedAddress, Size);
	return STATUS_SUCCESS;
}

PVOID get_system_routine_address(LPCWSTR routine_name)
{
	UNICODE_STRING name;
	RtlInitUnicodeString(&name, routine_name);
	return MmGetSystemRoutineAddress(&name);
}

PVOID get_system_module_base(LPCWSTR module_name)
{
	//lkd > dt nt!_LDR_DATA_TABLE_ENTRY - l 0xffff8f8a`0f25f110
	//	at 0xffff8f8a`0f25f110
	//	-------------------------------------------- -
	//	+ 0x000 InLoadOrderlinks : _LIST_ENTRY[0xffff8f8a`0cee8c90 - 0xffff8f8a`0f25b010]
	//	+ 0x010 InMemoryOrderlinks : _LIST_ENTRY[0xfffff3ae`f4708000 - 0x00000000`00017034]
	//	+ 0x020 InInitializationOrderlinks : _LIST_ENTRY[0x00000000`00000000 - 0xffff8f8a`0f25f290]
	//	+ 0x030 DllBase          : 0xfffff3ae`f4520000 Void
	//	+ 0x038 EntryPoint       : 0xfffff3ae`f4751010 Void
	//	+ 0x040 SizeOfImage      : 0x26d000
	//	+ 0x048 FullDllName : _UNICODE_STRING "\SystemRoot\System32\win32kbase.sys"
	//	+ 0x058 BaseDllName : _UNICODE_STRING "win32kbase.sys"
	//	+ 0x068 FlagGroup : [4]  ""

	PVOID module_base = NULL;

	PLIST_ENTRY module_list = (PLIST_ENTRY)(get_system_routine_address(L"PsLoadedModuleList"));

	if (!module_list)
		return NULL;

	UNICODE_STRING name;
	RtlInitUnicodeString(&name, module_name);
	//  InLoadOrderlinks.Flink at 0xffff8f8a`0f25f110
	//	-------------------------------------------- -
	//	+ 0x000 InLoadOrderlinks :  [0xffff8f8a`0cee8c90 - 0xffff8f8a`0f25b010]
	//	+ 0x048 FullDllName : _UNICODE_STRING "\SystemRoot\System32\win32kbase.sys"
	for (PLIST_ENTRY link = module_list; link != module_list->Blink; link = link->Flink)
	{
		LDR_DATA_TABLE_ENTRY* entry = CONTAINING_RECORD(link, LDR_DATA_TABLE_ENTRY, InLoadOrderLinks);

		//DbgPrint( "driver: %ws\n", entry->FullDllName.Buffer );

		if (RtlEqualUnicodeString(&entry->BaseDllName, &name, TRUE))
		{
			module_base = entry->DllBase;
			break;
		}
	}

	return module_base;
}

PVOID get_system_module_export(LPCWSTR module_name, LPCSTR routine_name)
{
	PVOID lpModule = get_system_module_base(module_name);
	if (!lpModule)
		return NULL;
	return RtlFindExportedRoutineByName(lpModule, routine_name);
}




NTSTATUS
OpenSessionProcessThread(
	_Outptr_ PEPROCESS* Process,
	_Outptr_ PETHREAD* Thread,
	_In_ PUNICODE_STRING ProcessName,
	_In_ ULONG SessionId,
	_Out_ PVOID* Win32Process,
	_Out_ PVOID* Win32Thread,
	_Out_ PCLIENT_ID ClientId
)
{
	ULONG Size;
	NTSTATUS Status = ZwQuerySystemInformation(SystemProcessInformation, NULL, 0, &Size);
	if (Status != STATUS_INFO_LENGTH_MISMATCH)
		return Status;
	const PSYSTEM_PROCESS_INFORMATION SystemProcessInfo = ExAllocatePoolZero(NonPagedPool, 2ull * Size, 'mooD');
	if (SystemProcessInfo == NULL)
		return STATUS_INSUFFICIENT_RESOURCES;
	Status = ZwQuerySystemInformation(SystemProcessInformation,
		SystemProcessInfo,
		2 * Size,
		NULL);
	if (!NT_SUCCESS(Status)) {
		ExFreePool(SystemProcessInfo);
		return Status;
	}

	PSYSTEM_PROCESS_INFORMATION Entry = SystemProcessInfo;
	Status = STATUS_NOT_FOUND;

	while (TRUE) {

		if (Entry->ImageName.Buffer != NULL && RtlEqualUnicodeString(&Entry->ImageName, ProcessName, TRUE)) {
			Status = PsLookupProcessByProcessId(Entry->UniqueProcessId, Process);
			if (NT_SUCCESS(Status)) {
				if (PsGetProcessSessionIdEx(*Process) == SessionId) {
					// hack to (probably) find the main thread ID
					CLIENT_ID MinThreadIdCid = { .UniqueProcess = NULL, .UniqueThread = (HANDLE)MAXULONG_PTR };
					for (ULONG i = 0; i < Entry->NumberOfThreads; ++i) {
						if ((ULONG)(ULONG_PTR)Entry->Threads[i].ClientId.UniqueThread < (ULONG)(ULONG_PTR)MinThreadIdCid.UniqueThread) {
							MinThreadIdCid = Entry->Threads[i].ClientId;
						}
					}

					for (ULONG i = 0; i < Entry->NumberOfThreads; ++i) {
						Status = PsLookupProcessThreadByCid(&MinThreadIdCid, NULL, Thread);
						if (NT_SUCCESS(Status)) {
							if ((*Win32Process = PsGetProcessWin32Process(*Process)) != NULL &&
								(*Win32Thread = PsGetThreadWin32Thread(*Thread)) != NULL) {
								*ClientId = MinThreadIdCid;
								ExFreePool(SystemProcessInfo);
								return STATUS_SUCCESS;
							}
							ObDereferenceObject(*Thread);
						}
					}
				}
				ObDereferenceObject(*Process);
			}
		}

		if (Entry->NextEntryOffset == 0)
			break;

		Entry = (PSYSTEM_PROCESS_INFORMATION)((ULONG_PTR)Entry + Entry->NextEntryOffset);
	}

	ExFreePool(SystemProcessInfo);
	return Status;
}


VOID DriverUnload(PDRIVER_OBJECT DriverObject) {
	UNREFERENCED_PARAMETER(DriverObject);

	PVOID Win32Process = 0;
	PVOID Win32Thread = 0;
	PETHREAD TargetThread = NULL;
	PEPROCESS TargetProcess = NULL;
	CLIENT_ID targetCid = { 0 };
	UNICODE_STRING TargetProcessName = RTL_CONSTANT_STRING(L"winlogon.exe");

	NTSTATUS Status = OpenSessionProcessThread(&TargetProcess, &TargetThread, &TargetProcessName, 1, &Win32Process, &Win32Thread, &targetCid);
	if (!NT_SUCCESS(Status)) {
		DbgPrint("[!] exit: Failed to get a thread from process \"%wZ\"\n", &TargetProcessName);
		return Status;
	}
	Status = PsAcquireProcessExitSynchronization(TargetProcess);
	if (!NT_SUCCESS(Status)) {
		ObDereferenceObject(TargetThread);
		ObDereferenceObject(TargetProcess);
		DbgPrint("[!] exit: Failed to acquire rundown protection on process \"%wZ\"\n", &TargetProcessName);
		return Status;
	}

	DbgPrint("[*] TargetThread 0x%llX at 0x%p, thread 0x%llX at 0x%p\n",
		(ULONG_PTR)targetCid.UniqueProcess, TargetProcess,
		(ULONG_PTR)targetCid.UniqueThread, TargetThread);
	DbgPrint("[*] Win32Process = 0x%p\n", Win32Process);
	DbgPrint("[*] Win32Thread = 0x%p\n", Win32Thread);

	KAPC_STATE state;
	KeStackAttachProcess((PKPROCESS)TargetProcess, &state);

	Status = Overwrite(IsImmersiveBrokerFuncAddr, (PVOID)IsImmersiveBrokerFuncOrignalBytes, 7);

	if (Status != STATUS_SUCCESS)
		DbgPrint("[!] Failed to restore the orignal IsImmersiveBroker function\n");
	else
		DbgPrint("[+] Successfully restored the orignal IsImmersiveBroker function\n");

	Status = Overwrite(IAMThreadAccessGrantedFuncAddr, (PVOID)IAMThreadAccessGrantedFuncOrignalBytes, 18);

	if (Status != STATUS_SUCCESS)
		DbgPrint("[!] Failed to restore the orignal IAMThreadAccessGranted function\n");
	else
		DbgPrint("[+] Successfully restored the orignal IAMThreadAccessGranted function\n");


	Status = Overwrite(NtUserEnableIAMAccessFuncAddr, (PVOID)NtUserEnableIAMAccessFuncOrignalBytes, 24);

	if (Status != STATUS_SUCCESS)
		DbgPrint("[!] Failed to restore the orignal NtUserEnableIAMAccess function\n");
	else
		DbgPrint("[+] Successfully restored the orignal NtUserEnableIAMAccess function\n");

	KeUnstackDetachProcess(&state);
	DbgPrint("[*] Goodbye Cruel World\n");
}
typedef int(IsImmersiveBrokerFunc)(__int64 unknown);




ULONG_PTR GetPointerToIAMThreadAccessGranted()
{
	PVOID Win32kFull = get_system_module_base(L"win32kfull.sys");
	if (Win32kFull == NULL)
	{
		DbgPrint("[!] Failed to get pointer to win32kfull.sys\n");
		return NULL;
	}

	PVOID CreateWindowGroup = get_system_module_export(L"win32kfull.sys", "NtUserSetActiveProcessForMonitor");
	char* CreateWindowGroupPointer = (PVOID)CreateWindowGroup;

	DbgPrint("[*] NtUserSetActiveProcessForMonitor offset from win32kfull.sys: %llu\n", (char*)CreateWindowGroup - (char*)Win32kFull);	

	DbgPrint("[*] NtUserSetActiveProcessForMonitor[40]: ");
	for (INT i = 0; i < 40; i++)
		DbgPrint("0x%x ", CreateWindowGroupPointer[i] & 0xff);
	DbgPrint("\n");


	ZydisDecoder Decoder;
	ZydisDecoderInit(&Decoder, ZYDIS_MACHINE_MODE_LONG_64, ZYDIS_STACK_WIDTH_64);
	ZydisFormatter Formatter;
	ZydisFormatterInit(&Formatter, ZYDIS_FORMATTER_STYLE_INTEL);
	UINT64 ReadOffset = 0;
	ZydisDecodedInstruction Instruction;
	ZydisDecodedOperand InstructionOperands[ZYDIS_MAX_OPERAND_COUNT];
	ZyanStatus Status;
	CHAR PrintBuffer[128];
	int CallOpcodeNumb = 0;
	int found = 0;
	while ((Status = ZydisDecoderDecodeFull(&Decoder, (PVOID)((UINT64)CreateWindowGroup + ReadOffset), sizeof(CreateWindowGroup) + 1, &Instruction, &InstructionOperands)) != ZYDIS_STATUS_NO_MORE_DATA) {
		NT_ASSERT(ZYAN_SUCCESS(Status));
		if (!ZYAN_SUCCESS(Status)) {
			ReadOffset++;
			continue;
		}
		CONST UINT64 InstrAddress = (UINT64)((UINT64)CreateWindowGroup + ReadOffset);
		ZydisFormatterFormatInstruction(&Formatter, &Instruction, &InstructionOperands, Instruction.operand_count, PrintBuffer, sizeof(PrintBuffer), InstrAddress, ZYAN_NULL);
		DbgPrint("+%-4X 0x%-16llX\t\t%hs\n", (ULONG)ReadOffset, InstrAddress, PrintBuffer);
		ReadOffset += Instruction.length; //the call instruction address is computed after the instr
		if (Instruction.opcode == 0xE8)
		{
			found = 1;
			DbgPrint("[*] found 1st CALL opcode!\n");
			break;
		}


	}

	if (found == 1)
	{
		DbgPrint("read offset: %d\n", ReadOffset);

		DbgPrint("[*] found it i think: signed: %lld relative: %x value signed: %I64x, operand count: %d, opcode len: %d\n", InstructionOperands[0].imm.value.s, InstructionOperands[0].imm.is_relative, InstructionOperands[0].imm.value.s, Instruction.operand_count, Instruction.length);
		//DbgPrint("[*] %d %I64x\n", InstructionOperands[1].type, InstructionOperands[1].imm.value.s);
		//DbgPrint("[*] %d %I64x\n", InstructionOperands[2].type, InstructionOperands[2].imm.value.s);
		//DbgPrint("[*] %d %I64x\n", InstructionOperands[3].type, InstructionOperands[3].mem.value.s);
		//DbgPrint("[*] %d %I64x\n", Instruction.operands[4].type, Instruction.operands[4].imm.value.s);

		UINT64 ptr2 = ((UINT64)CreateWindowGroup + ReadOffset);
		DbgPrint("[*] Function offset call: %lu\n", ptr2);
		ptr2 += InstructionOperands[0].imm.value.s;
		DbgPrint("[*] Function offset call neg: %lu\n", ptr2);
		//ptr2 -= 12;
		//PVOID result = (PVOID)((UINT64)CreateWindowGroup + ReadOffset + InstructionOperands[0].imm.value.s);
		DbgPrint("[*] Function offset from win32kfull.sys: %lu\n", (char*)ptr2 - (char*)Win32kFull);

		//DbgBreakPoint();
		//return NULL;
		return (ULONG_PTR)ptr2;
	}
	return NULL;
}

NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject,
	PUNICODE_STRING RegistryPath) {
	UNREFERENCED_PARAMETER(RegistryPath);

	DriverObject->DriverUnload = DriverUnload;

	DbgPrint("[*] Hello World\n");
	DbgPrint("[*] Attempting to initialize library\n");

	UNICODE_STRING myUnicodeStr;
	RtlInitUnicodeString(&myUnicodeStr, L"IsImmersiveBroker");

	PVOID Win32Process = 0;
	PVOID Win32Thread = 0;
	PETHREAD TargetThread = NULL;
	PEPROCESS TargetProcess = NULL;
	CLIENT_ID targetCid = { 0 };
	UNICODE_STRING TargetProcessName = RTL_CONSTANT_STRING(L"winlogon.exe");

	NTSTATUS Status = OpenSessionProcessThread(&TargetProcess, &TargetThread, &TargetProcessName, 1, &Win32Process, &Win32Thread, &targetCid);
	if (!NT_SUCCESS(Status)) {
		DbgPrint("[!] Failed to get a thread from process \"%wZ\"\n", &TargetProcessName);
		return Status;
	}
	Status = PsAcquireProcessExitSynchronization(TargetProcess);
	if (!NT_SUCCESS(Status)) {
		ObDereferenceObject(TargetThread);
		ObDereferenceObject(TargetProcess);
		DbgPrint("[!] Failed to acquire rundown protection on process \"%wZ\"\n", &TargetProcessName);
		return Status;
	}

	DbgPrint("[*] TargetThread 0x%llX at 0x%p, thread 0x%llX at 0x%p\n",
		(ULONG_PTR)targetCid.UniqueProcess, TargetProcess,
		(ULONG_PTR)targetCid.UniqueThread, TargetThread);
	DbgPrint("[*] Win32Process = 0x%p\n", Win32Process);
	DbgPrint("[*] Win32Thread = 0x%p\n", Win32Thread);

	KAPC_STATE state;
	KeStackAttachProcess((PKPROCESS)TargetProcess, &state);

#pragma region IsImmersiveBroker Hook

	IsImmersiveBrokerFuncAddr = (ULONG_PTR)get_system_module_export(L"win32kbase.sys", "IsImmersiveBroker");

	if (IsImmersiveBrokerFuncAddr == NULL)
	{
		DbgPrint("[*] Unable to find IsImmersiveBroker\n");
		KeUnstackDetachProcess(&state);
		return STATUS_FAILED_DRIVER_ENTRY;
	}
	else
	{
		DbgPrint("[*] Found IsImmersiveBroker at %lu\n", (ULONG)IsImmersiveBrokerFuncAddr);
	}

	RtlCopyMemory(IsImmersiveBrokerFuncOrignalBytes, IsImmersiveBrokerFuncAddr, 7);

	if (IsImmersiveBrokerFuncOrignalBytes[0])
		DbgPrint("[+] Copied over IsImmersiveBroker\n");
	else {
		DbgPrint("[!] Failed to copy\n");
		KeUnstackDetachProcess(&state);
		return STATUS_FAILED_DRIVER_ENTRY;
	}

	for (INT i = 0; i < 14; i++)
		DbgPrint("[*] IsImmersiveBroker[%d]: 0x%x\n", i,
			IsImmersiveBrokerFuncOrignalBytes[i] & 0xff);

#if defined(_M_X64)
	CHAR IsImmersiveBrokerPatch[] = {
		0x48, 0x31, 0xC0, //xor    rax, rax
		0x48, 0xFF, 0xC0, //inc    rax
		0xC3, //ret
	};;


	Status = Overwrite(IsImmersiveBrokerFuncAddr, (PVOID)IsImmersiveBrokerPatch, sizeof(IsImmersiveBrokerPatch));

	if (Status != STATUS_SUCCESS) {
		DbgPrint("[!] Failed to overwrite IsImmersiveBroker\n");
		KeUnstackDetachProcess(&state);
		return STATUS_FAILED_DRIVER_ENTRY;
	}

	DbgPrint("[+] Successfully overwrote IsImmersiveBroker\n");
#else
	DbgPrint("[!] Unknown architecture");
	return STATUS_FAILED_DRIVER_ENTRY;
#endif

	CHAR Temp[14] = { 0 };
	RtlCopyMemory(Temp, IsImmersiveBrokerFuncAddr, 14);

	for (INT i = 0; i < 14; i++)
		DbgPrint("[*] IsImmersiveBroker[%d]: 0x%x\n", i,
			Temp[i] & 0xff);
#pragma endregion
#pragma region Hook IAMThreadAccessGranted

	IAMThreadAccessGrantedFuncAddr = GetPointerToIAMThreadAccessGranted();

	if (IAMThreadAccessGrantedFuncAddr == NULL)
	{
		DbgPrint("[*] Unable to get pointer to IAMThreadAccessGranted\n");
		KeUnstackDetachProcess(&state);
		return STATUS_SUCCESS;
	}
	else
	{
		DbgPrint("[*] Found IAMThreadAccessGranted\n");
		DbgPrint("[*] IAMThreadAccessGranted pointer: %lu\n", (ULONG)IAMThreadAccessGrantedFuncAddr);
	}
	char* ptr = (char*)IAMThreadAccessGrantedFuncAddr;

	DbgPrint("[*] IAMThreadAccessGranted[40]: ");
	for (INT i = 0; i < 40; i++)
		DbgPrint("0x%x ", ptr[i] & 0xff);
	DbgPrint("\n");

	RtlCopyMemory(IAMThreadAccessGrantedFuncOrignalBytes, IAMThreadAccessGrantedFuncAddr, 24);

	if (IAMThreadAccessGrantedFuncOrignalBytes[0])
		DbgPrint("[+] Copied over IAMThreadAccessGranted\n");
	else {
		DbgPrint("[!] Failed to copy\n");
		for (INT i = 0; i < 14; i++)
			DbgPrint("[*] IAMThreadAccessGranted[%d]: 0x%x\n", i,
				IAMThreadAccessGrantedFuncOrignalBytes[i] & 0xff);
		KeUnstackDetachProcess(&state);
		return STATUS_SUCCESS;
	}

	for (INT i = 0; i < 14; i++)
		DbgPrint("[*] IAMThreadAccessGranted[%d]: 0x%x\n", i,
			IAMThreadAccessGrantedFuncOrignalBytes[i] & 0xff);

#if defined(_M_X64)
	CHAR IAMThreadAccessGrantedPatch[] = {
		0x41, 0x52, //push r10
		0x49, 0xBA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //movabs r10, address
		0x41, 0x8B, 0x02, //mov eax, dword ptr [r10]
		0x41, 0x5A, //pop r10
		0xC3 //ret
	};
	ULONG_PTR sourceAddress = (ULONG_PTR)&g_enable;
	CHAR* sourceAddressBytes = (CHAR*)&sourceAddress;
	RtlCopyMemory(&IAMThreadAccessGrantedPatch[4], sourceAddressBytes, sizeof(ULONG_PTR));

	Status = Overwrite(IAMThreadAccessGrantedFuncAddr, (PVOID)IAMThreadAccessGrantedPatch, sizeof(IAMThreadAccessGrantedPatch));

	if (Status != STATUS_SUCCESS) {
		DbgPrint("[!] Failed to overwrite IAMThreadAccessGranted\n");
		KeUnstackDetachProcess(&state);
		return STATUS_FAILED_DRIVER_ENTRY;
	}

	DbgPrint("[+] Successfully overwrote IAMThreadAccessGranted\n");
#else
	DbgPrint("[!] Unknown architecture");
	return STATUS_FAILED_DRIVER_ENTRY;
#endif

	CHAR Temp2[14] = { 0 };
	RtlCopyMemory(Temp2, IAMThreadAccessGrantedFuncAddr, 14);

	for (INT i = 0; i < 14; i++)
		DbgPrint("[*] IAMThreadAccessGranted[%d]: 0x%x\n", i,
			Temp2[i] & 0xff);
#pragma endregion
#pragma region Hook Enable IAM access

	NtUserEnableIAMAccessFuncAddr = (ULONG_PTR)get_system_module_export(L"win32kfull.sys", "NtUserEnableIAMAccess");// (ULONG_PTR)(((char*)Win32kFull + 0x8f634));

	if (NtUserEnableIAMAccessFuncAddr == NULL)
	{
		DbgPrint("[*] Unable to get pointer to NtUserEnableIAMAccess\n");
		KeUnstackDetachProcess(&state);
		return STATUS_SUCCESS;
	}
	else
	{
		DbgPrint("[*] Found NtUserEnableIAMAccess at %lu\n", (ULONG)NtUserEnableIAMAccessFuncAddr);
	}
	char* ptr3 = (char*)NtUserEnableIAMAccessFuncAddr;

	DbgPrint("[*] NtUserEnableIAMAccess[40]: ");
	for (INT i = 0; i < 40; i++)
		DbgPrint("0x%x ", ptr3[i] & 0xff);
	DbgPrint("\n");

	RtlCopyMemory(NtUserEnableIAMAccessFuncOrignalBytes, NtUserEnableIAMAccessFuncAddr, 24);

	if (NtUserEnableIAMAccessFuncOrignalBytes[0])
		DbgPrint("[+] Copied over NtUserEnableIAMAccess\n");
	else {
		DbgPrint("[!] Failed to copy\n");
		for (INT i = 0; i < 14; i++)
			DbgPrint("[*] NtUserEnableIAMAccess[%d]: 0x%x\n", i,
				NtUserEnableIAMAccessFuncOrignalBytes[i] & 0xff);
		KeUnstackDetachProcess(&state);
		return STATUS_SUCCESS;
	}

	for (INT i = 0; i < 14; i++)
		DbgPrint("[*] NtUserEnableIAMAccess[%d]: 0x%x\n", i,
			NtUserEnableIAMAccessFuncOrignalBytes[i] & 0xff);

#if defined(_M_X64)
	//r15d register contains address to the pointer of g_enabled
	CHAR NtUserEnableIAMAccessPatch[] = {

		 0x41, 0x52, //push r10
		 0x49, 0xBA, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //mov r10, address
		 0x41, 0x89, 0x12, //mov    DWORD PTR [r10], edx
		 0x48, 0x31, 0xC0, //xor    rax, rax
		 0x48, 0xFF, 0xC0, //inc rax
		 0x41, 0x5A, //pop r10
		 0xC3 //ret
	};

	ULONG_PTR sourceAddress2 = (ULONG_PTR)&g_enable;
	CHAR* sourceAddressBytes2 = (CHAR*)&sourceAddress2;
	RtlCopyMemory(&NtUserEnableIAMAccessPatch[4], &sourceAddressBytes2, sizeof(ULONG_PTR));
	DbgPrint("[*] NtUserEnableIAMAccessPatched[]: ");
	for (INT i = 0; i < 14; i++)
		DbgPrint("%x ",NtUserEnableIAMAccessFuncOrignalBytes[i] & 0xff);
	DbgPrint("\n");

	DbgPrint("wrote the address");

	Status = Overwrite(NtUserEnableIAMAccessFuncAddr, (PVOID)NtUserEnableIAMAccessPatch, sizeof(NtUserEnableIAMAccessPatch));

	if (Status != STATUS_SUCCESS) {
		DbgPrint("[!] Failed to overwrite NtUserEnableIAMAccess\n");
		KeUnstackDetachProcess(&state);
		return STATUS_FAILED_DRIVER_ENTRY;
	}

	DbgPrint("[+] Successfully overwrote NtUserEnableIAMAccess\n");
#else
	DbgPrint("[!] Unknown architecture");
	return STATUS_FAILED_DRIVER_ENTRY;
#endif
#pragma endregion



	KeUnstackDetachProcess(&state);
	return STATUS_SUCCESS;
}