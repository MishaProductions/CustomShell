#define WIN32_LEAN_AND_MEAN
#define _CRT_SECURE_NO_WARNINGS 1
#include <windows.h>
#include <mutex>
#include <psapi.h>

//INSTALL DETOURS FROM NUGET! (or build from source yourself)
#include <detours.h>
#include "CustomShell.h"
#include <iostream>

int MainHook(
	 HINSTANCE hInstance,
	 HINSTANCE hPrevInstance,
	 LPSTR     lpCmdLine,
	int       nShowCmd
)
{
	printf("Winmain hooked\n");

	if (FAILED(CoInitializeEx(NULL, 2)))
	{
		printf("Failed to initialize COM\n");
		return -1;
	}
	if (FAILED(CoInitializeEx(NULL, 2)))
	{
		printf("Failed to initialize COM\n");
		return -1;
	}
	printf("Starting it\n");

	//create the shell object
	CustomShell* object = new CustomShell();
	if (FAILED(object->Run()))
	{
		printf("Run failed\n");
	}
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
		freopen("CONOUT$", "w", stdout);
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
	printf("STUB FUNCTION: HamCloseActivity\n");
}