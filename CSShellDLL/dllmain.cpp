#include "pch.h"
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <iostream>
#include <sddl.h>
#include "dxgi_imp.h"

#pragma comment(lib, "shlwapi.lib")
#define IS_FLAG_SET(dw,fl)  (((dw) & (fl)) == fl)
using namespace std;
using string_t = std::basic_string<char_t>;

//Most of the code is copied from: https://github.com/dotnet/samples/blob/main/core/hosting/src/NativeHost/nativehost.cpp#L172

/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/


HRESULT ConvertSecurityDescriptor(
	PSECURITY_DESCRIPTOR pSelfRelSD,
	PSECURITY_DESCRIPTOR* ppAbsoluteSD)
{
	BOOL bResult;
	PACL pDacl = nullptr;
	PACL pSacl = nullptr;
	PSID pOwner = nullptr;
	PSID pPrimaryGroup = nullptr;
	DWORD dwAbsoluteSDSize = 0;
	DWORD dwDaclSize = 0;
	DWORD dwSaclSize = 0;
	DWORD dwOwnerSize = 0;
	DWORD dwPrimaryGroupSize = 0;
	size_t sizeNeeded = 0;
	*ppAbsoluteSD = nullptr;

	if (!pSelfRelSD ||
		!IS_FLAG_SET(((SECURITY_DESCRIPTOR*)(pSelfRelSD))->Control, SE_SELF_RELATIVE))
	{
		return E_INVALIDARG;
	}

	MakeAbsoluteSD(
		pSelfRelSD,
		nullptr,
		&dwAbsoluteSDSize,
		nullptr,
		&dwDaclSize,
		nullptr,
		&dwSaclSize,
		nullptr,
		&dwOwnerSize,
		nullptr,
		&dwPrimaryGroupSize
	);

	sizeNeeded =
		dwAbsoluteSDSize +
		dwDaclSize +
		dwSaclSize +
		dwOwnerSize +
		dwPrimaryGroupSize;
	*ppAbsoluteSD = (PSECURITY_DESCRIPTOR)LocalAlloc(LMEM_FIXED, sizeNeeded);

	if (!*ppAbsoluteSD)
		return E_OUTOFMEMORY;

	BYTE* position = (BYTE*)(*ppAbsoluteSD);
	pDacl = reinterpret_cast<PACL>(position += dwAbsoluteSDSize);
	pSacl = reinterpret_cast<PACL>(position += dwDaclSize);
	pOwner = reinterpret_cast<PSID>(position += dwSaclSize);
	pPrimaryGroup = reinterpret_cast<PSID>(position += dwOwnerSize);

	bResult = MakeAbsoluteSD(
		pSelfRelSD,
		*ppAbsoluteSD,
		&dwAbsoluteSDSize,
		pDacl,
		&dwDaclSize,
		pSacl,
		&dwSaclSize,
		pOwner,
		&dwOwnerSize,
		pPrimaryGroup,
		&dwPrimaryGroupSize
	);

	if (!bResult)
	{
		LocalFree(*ppAbsoluteSD);
		return HRESULT_FROM_WIN32(GetLastError());
	}

	return S_OK;
}


BOOL InitSecurityForAppContainer()
{
	ULONG selfRelativeSecurityDescriptorSize{};
	PSECURITY_DESCRIPTOR pSelfRelativeSecurityDescriptor{};
	PSECURITY_DESCRIPTOR absoluteSecurityDescriptor{};

	if (ConvertStringSecurityDescriptorToSecurityDescriptor(
		L"O:BAG:BAD:(A;;0x7;;;PS)(A;;0x3;;;SY)(A;;0x7;;;BA)(A;;0x3;;;AC)(A;;0x3;;;S-1-5-80-21560277-3893537710-1672964824-3682772274-2051710322)",
		SDDL_REVISION_1,
		&pSelfRelativeSecurityDescriptor,
		&selfRelativeSecurityDescriptorSize))
	{
		if (ConvertSecurityDescriptor(pSelfRelativeSecurityDescriptor, &absoluteSecurityDescriptor) == S_OK)
		{
			if (CoInitializeSecurity(
				absoluteSecurityDescriptor,
				-1,
				NULL,
				NULL,
				RPC_C_AUTHN_LEVEL_DEFAULT,
				RPC_C_IMP_LEVEL_IDENTIFY,
				NULL,
				EOAC_APPID,
				NULL) != S_OK)
			{
				auto lastErr = GetLastError();
			}
		}
		else
		{
			auto lastErr = GetLastError();
		}
	}
	else
	{
		auto lastErr = GetLastError();
	}

	if (pSelfRelativeSecurityDescriptor)
		LocalFree(pSelfRelativeSecurityDescriptor);

	if (absoluteSecurityDescriptor)
		LocalFree(absoluteSecurityDescriptor);

	return TRUE;
}

namespace
{
	// Forward declarations
	void* load_library(const char_t*);
	void* get_export(void*, const char*);

	void* load_library(const char_t* path)
	{
		HMODULE h = ::LoadLibraryW(path);
		assert(h != nullptr);
		return (void*)h;
	}
	void* get_export(void* h, const char* name)
	{
		void* f = ::GetProcAddress((HMODULE)h, name);
		assert(f != nullptr);
		return f;
	}
}


namespace
{
	// Globals to hold hostfxr exports
	hostfxr_initialize_for_runtime_config_fn init_fptr;
	hostfxr_get_runtime_delegate_fn get_delegate_fptr;
	hostfxr_close_fn close_fptr;
}

// Using the nethost library, discover the location of hostfxr and get exports
bool load_hostfxr()
{
	// Pre-allocate a large buffer for the path to hostfxr
	char_t buffer[MAX_PATH];
	size_t buffer_size = sizeof(buffer) / sizeof(char_t);
	int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
	if (rc != 0)
		return false;

	// Load hostfxr and get desired exports
	void* lib = load_library(buffer);
	init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
	get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
	close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

	return (init_fptr && get_delegate_fptr && close_fptr);
}
// Load and initialize .NET Core and get desired function pointer for scenario
load_assembly_and_get_function_pointer_fn get_dotnet_load_assembly(const char_t* config_path)
{
	// Load .NET Core
	void* load_assembly_and_get_function_pointer = nullptr;
	hostfxr_handle cxt = nullptr;
	int rc = init_fptr(config_path, nullptr, &cxt);
	if (rc != 0 || cxt == nullptr)
	{
		std::cout << "Init failed: " << std::hex << std::showbase << rc << std::endl;
		close_fptr(cxt);
		return nullptr;
	}

	// Get the load assembly function pointer
	rc = get_delegate_fptr(
		cxt,
		hdt_load_assembly_and_get_function_pointer,
		&load_assembly_and_get_function_pointer);
	if (rc != 0 || load_assembly_and_get_function_pointer == nullptr)
		std::cout << "Get delegate failed: " << std::hex << std::showbase << rc << std::endl;

	close_fptr(cxt);
	return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
}

int MainHook(
	HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPSTR     lpCmdLine,
	int       nShowCmd
)
{
	printf("Winmain hooked\n");

	TCHAR wszRealDXGIPath[MAX_PATH];
	GetSystemDirectoryW(wszRealDXGIPath, MAX_PATH);
	wcscat_s(wszRealDXGIPath, MAX_PATH, L"\\dxgi.dll");
	SetupDXGIImportFunctions(LoadLibraryW(wszRealDXGIPath));

	if (FAILED(CoInitializeEx(NULL, 2)))
	{
		printf("Failed to initialize COM\n");
		return -1;
	}

	if (!InitSecurityForAppContainer())
	{
		printf("failed to init security for app container\n");
	}
	printf("Starting it\n");
	// Get the current executable's directory
   // This sample assumes the managed assembly to load and its runtime configuration file are next to the host
	const char* str1 = "The basic_string";

	basic_string <char> str3a(5, '9');


	//
	// STEP 1: Load HostFxr and get exported hosting functions
	//
	if (!load_hostfxr())
	{
		assert(false && "Failure: load_hostfxr()");
		return EXIT_FAILURE;
	}

	//printf("root path %ls", root_path.c_str());

	//
	// STEP 2: Initialize and start the .NET Core runtime
	//
	string_t root_path;
	wchar_t pBuf[256];
	size_t len = sizeof(pBuf);
	int bytes = GetModuleFileNameW(NULL, pBuf, len);
	string_t root_file = pBuf;

	const size_t last_slash_idx = root_file.rfind('\\');
	if (std::string::npos != last_slash_idx)
	{
		root_path = root_file.substr(0, last_slash_idx);
	}
	root_path = root_path + L"\\";

	const string_t config_path = root_path +L"CSShellManaged.runtimeconfig.json";
	printf("config: %ls\n", config_path.c_str());
	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
	load_assembly_and_get_function_pointer = get_dotnet_load_assembly(config_path.c_str());
	assert(load_assembly_and_get_function_pointer != nullptr);

	//
    // STEP 3: Load managed assembly and get function pointer to a managed method
    //
	const string_t dotnetlib_path = root_path + L"CSShellManaged.dll";
	const char_t* dotnet_type = L"CSShellManaged.Program, CSShellManaged";
	const char_t* dotnet_type_method = L"Main";
	// <SnippetLoadAndGet>
	// Function pointer to managed delegate
	component_entry_point_fn hello = nullptr;
	int rc = load_assembly_and_get_function_pointer(
		dotnetlib_path.c_str(),
		dotnet_type,
		dotnet_type_method,
		nullptr /*delegate_type_name*/,
		nullptr,
		(void**)&hello);
	// </SnippetLoadAndGet>
	if (rc != 0)
	{
		printf("error: %x", rc);
	}
	assert(rc == 0 && hello != nullptr && "Failure: load_assembly_and_get_function_pointer()");
	//
   // STEP 4: Run managed code
   //
	struct lib_args
	{
		int instance;
		int number;
	};
	// <SnippetCallManaged>
	lib_args args
	{
		0,
		1
	};

	hello(&args, sizeof(args));


	system("pause");

	return -1;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{
		DisableThreadLibraryCalls(hModule);
		AllocConsole();
		freopen("CONIN$", "r", stdin);
		freopen("CONOUT$", "w", stdout);
		freopen("CONOUT$", "w", stderr);
		std::cout << "This works" << std::endl;
		//LaunchCustomShellHost
		MODULEINFO info = {};
		if (!GetModuleInformation(GetCurrentProcess(), NULL, &info, sizeof(MODULEINFO)))
		{
			printf("GetModuleInformation() failure: %d", GetLastError());
		}
		else
		{
			printf("got the module info\n");
			printf("entry point is at %x, hooking it now\n", info.EntryPoint);

			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&info.EntryPoint, (PVOID)MainHook);
			DetourTransactionCommit();
		}
	}
	break;
	case DLL_PROCESS_DETACH:
	{
		//TODO
	}
	break;
	}
	return TRUE;
}

void HamCloseActivity()
{
	DebugBreak();
	printf("STUB FUNCTION: HamCloseActivity\n");
}


void HamPopulateActivityProperties()
{
	DebugBreak();
	printf("STUB FUNCTION: HamPopulateActivityProperties\n");
}

void HamCreateActivityForProcess()
{
	DebugBreak();
	printf("STUB FUNCTION: HamCreateActivityForProcess\n");
}
void HamStartActivityAsync()
{
	DebugBreak();
	printf("STUB FUNCTION: HamStartActivityAsync\n");
}
void HamConnectToServer()
{
	DebugBreak();
	printf("STUB FUNCTION: HamConnectToServer\n");
}
void HamDisconnectFromServer()
{
	DebugBreak();
	printf("STUB FUNCTION: HamDisconnectFromServer\n");
}