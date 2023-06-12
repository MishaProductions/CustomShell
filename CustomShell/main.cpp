#define WIN32_LEAN_AND_MEAN
#define _CRT_SECURE_NO_WARNINGS 1
#include <windows.h>
#include <mutex>

//INSTALL DETOURS FROM NUGET! (or build from source yourself)
#include <detours.h>
#include "CustomShell.h"
#include <iostream>


//Definitions
typedef BOOL(WINAPI* SetWindowBand)(IN HWND hWnd, IN HWND hwndInsertAfter, IN DWORD dwBand);
typedef BOOL(WINAPI* NtUserEnableIAMAccess)(IN ULONG64 key, IN BOOL enable);

//Fields
NtUserEnableIAMAccess lNtUserEnableIAMAccess;
SetWindowBand lSetWindowBand;

ULONG64 g_iam_key = 0x0;
bool g_is_detached = false; //To prevent detaching twice.
std::mutex g_mutex;

//Forward functions
BOOL WINAPI NtUserEnableIAMAccessHook(ULONG64 key, BOOL enable);
BOOL SetWindowBandInternal(HWND hWnd, HWND hwndInsertAfter, DWORD dwBand);

//Function for detouring NtUserEnableIAMAccess
VOID AttachHook()
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach(&(PVOID&)lNtUserEnableIAMAccess, (PVOID)NtUserEnableIAMAccessHook);
	DetourTransactionCommit();
}

//Function for restoring NtUserEnableIAMAccess
VOID DetachHook()
{
	g_mutex.lock();
	if (!g_is_detached)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(PVOID&)lNtUserEnableIAMAccess, NtUserEnableIAMAccessHook);
		DetourTransactionCommit();
		g_is_detached = true;
	}
	g_mutex.unlock();
}

//Our detoured function
BOOL WINAPI NtUserEnableIAMAccessHook(ULONG64 key, BOOL enable)
{
	const auto res = lNtUserEnableIAMAccess(key, enable);

	if (res == TRUE && !g_iam_key)
	{
		g_iam_key = key;
		DetachHook();

		//Example, for testing only. Don't call it here, make an IPC for that.
		SetWindowBandInternal((HWND)0x503B4, NULL, 16);
	}

	return res;
}

//This functions is needed to induce explorer.exe (actually twinui.pcshell.dll) to call NtUserEnableIAMAccess
VOID TryForceIAMAccessCallThread(LPVOID lpParam)
{
	//These 7 calls will force a call into EnableIAMAccess.
	auto hwndFore = GetForegroundWindow();
	auto hwndToFocus = FindWindow(L"Shell_TrayWnd", NULL);
	SetForegroundWindow(GetDesktopWindow()); //This in case Shell_TrayWnd is already focused
	Sleep(100);
	SetForegroundWindow(hwndToFocus); //Focus on the taskbar, should trigger EnableIAMAccess
	Sleep(100);
	SetForegroundWindow(hwndFore); //Restore focus.
}

//Function helper to call SetWindowBand in the proper way.
BOOL SetWindowBandInternal(HWND hWnd, HWND hwndInsertAfter, DWORD dwBand)
{
	if (g_iam_key)
	{
		lNtUserEnableIAMAccess(g_iam_key, TRUE);
		const auto callResult = lSetWindowBand(hWnd, hwndInsertAfter, dwBand);
		lNtUserEnableIAMAccess(g_iam_key, FALSE);

		return callResult;
	}

	return FALSE;
}
typedef BOOL(WINAPI* SetErrorModeTHing)(IN UINT mode);
static SetErrorModeTHing org = NULL;
static BOOL run = true;
BOOL SetErrorModeTHingFunc(UINT mode)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(&(PVOID&)org, (PVOID)SetErrorModeTHingFunc);
	DetourTransactionCommit();

	run = FALSE;
	printf("hook called!\n");


	return TRUE;
}
void DoStuff(LPVOID threadparam)
{
	printf("Waiting for COM to be inited\n");
	//Sleep(1000);
	if (FAILED(CoInitializeEx(NULL, 2)))
	{
		printf("Failed to initialize COM\n");
		return;
	}
	printf("Starting it\n");
	//create the shell object
	CustomShell* object = new CustomShell();
	if (FAILED(object->Run()))
	{
		printf("Run failed\n");
	}

}
int main()
{
    if (FAILED(CoInitializeEx(NULL, 2)))
    {
        printf("Failed to initialize COM\n");
        return 1;
    }
    DoStuff(NULL);
	system("pause");
}

int WinMain(
	 HINSTANCE hInstance,
	 HINSTANCE hPrevInstance,
	 LPSTR     lpCmdLine,
	 int       nShowCmd
)
{
	DoStuff(NULL);
}
//DllMain
//BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
//{
//	switch (ul_reason_for_call)
//	{
//	case DLL_PROCESS_ATTACH:
//	{
//		DisableThreadLibraryCalls(hModule);
//		AllocConsole();
//		freopen("CONOUT$", "w", stdout);
//		std::cout << "This works" << std::endl;
//		//LaunchCustomShellHost
//
//   /*    if (FAILED(CoInitializeEx(NULL, 2)))
//		{
//			printf("Failed to initialize COM\n");
//			return 1;
//		}*/
//
//		//Wait for COM to be initialized
//
//		CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE)&DoStuff, NULL, NULL, NULL);
//
//	}
//	break;
//	case DLL_PROCESS_DETACH:
//	{
//		DetachHook();
//	}
//	break;
//	}
//	return TRUE;
//}

void HamCloseActivity()
{

}