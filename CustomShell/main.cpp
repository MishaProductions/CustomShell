#define WIN32_LEAN_AND_MEAN
#define _CRT_SECURE_NO_WARNINGS 1
#include <windows.h>
#include <mutex>
#include <psapi.h>

//INSTALL DETOURS FROM NUGET! (or build from source yourself)
#include <detours.h>
#include "CustomShell.h"
#include <iostream>


typedef HWND(WINAPI* CreateWindowInBandAPI)(DWORD, LPWSTR, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, DWORD);
static CreateWindowInBandAPI CreateWindowInBandOrig;

typedef HWND(WINAPI* CreateWindowInBandExAPI)(DWORD, LPWSTR, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, PVOID, DWORD, DWORD);
static CreateWindowInBandExAPI CreateWindowInBandExOrig;

typedef BOOL(WINAPI* GetWindowBandAPI)(HWND, DWORD*);
static GetWindowBandAPI GetWindowBandOrig;

typedef HWND(WINAPI* SetWindowBandApi)(HWND hwnd, HWND hwndInsertAfter, DWORD dwBand);
static SetWindowBandApi SetWindowBandApiOrg;

typedef BOOL(WINAPI* RegisterHotKeyApi)(HWND hwnd, int id, UINT fsMod, UINT vk);
static RegisterHotKeyApi RegisterHotKeyApiOrg;

HWND WINAPI CreateWindowInBandNew(DWORD dwExStyle,
	LPCWSTR lpClassName,
	LPCWSTR lpWindowName,
	DWORD dwStyle,
	int x,
	int y,
	int nWidth,
	int nHeight,
	HWND hWndParent,
	HMENU hMenu,
	HINSTANCE hInstance,
	LPVOID lpParam,
	DWORD dwBand)
{
	DWORD p0 = (DWORD)_ReturnAddress();
	printf("CreateWindowInBandNew\n");
	HWND ret = CreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);

	if (ret)
		SetProp(ret, L"UIA_WindowVisibilityOverriden", (HANDLE)2);
	return ret;
}

HWND WINAPI CreateWindowInBandExNew(DWORD dwExStyle,
	LPCWSTR lpClassName,
	LPCWSTR lpWindowName,
	DWORD dwStyle,
	int x,
	int y,
	int nWidth,
	int nHeight,
	HWND hWndParent,
	HMENU hMenu,
	HINSTANCE hInstance,
	LPVOID lpParam,
	DWORD dwBand, DWORD dwTypeFlags)
{
	DWORD p0 = (DWORD)_ReturnAddress();
	printf("CreateWindowInBandExNew\n");
	HWND ret = CreateWindowExW(dwExStyle, lpClassName, lpWindowName, dwStyle, x, y, nWidth, nHeight, hWndParent, hMenu, hInstance, lpParam);
	SetProp(ret, L"UIA_WindowVisibilityOverriden", (HANDLE)2);
	return ret;
}

BOOL WINAPI SetWindowBandNew(HWND hwnd, HWND hwndInsertAfter, DWORD flags)
{
	printf("SetWindowBandNew\n");
	return TRUE;
}

BOOL WINAPI ReturnZero()
{
	return TRUE;
}

BOOL WINAPI RegisterWindowHotkeyNew(HWND hwnd, int id, UINT mod, UINT vk)
{
	BOOL res = RegisterHotKeyApiOrg(hwnd, id, mod, vk);

	if (!res)
	{
		printf("RegisterWindowHotkeyNew did things\n");
		return TRUE;
	}

	return TRUE;
}

BOOL GetWindowTrackInfoAsyncStub(HWND handle, HWND h2)
{
	printf("GetWindowTrackInfoAsyncStub\n");

	BOOL res2= ChangeWindowMessageFilterEx(handle, 834, MSGFLT_ALLOW, NULL);
	HRESULT hr = GetLastError();
	BOOL b = PostMessageW(handle, 834, 1, 0);
	return TRUE;
}

BOOL ClearForegroundStub()
{
	printf("ClearForegroundStub\n");
	return TRUE;
}

BOOL CreateWindowGroupStub()
{
	printf("CreateWindowGroupStub\n");
	return TRUE;
}

BOOL DeleteWindowGroupStub()
{
	printf("DeleteWindowGroupStub\n");
	return TRUE;
}

BOOL EnableWindowGroupPolicyStub()
{
	printf("EnableWindowGroupPolicyStub\n");
	return TRUE;
}

BOOL SetBridgeWindowChildStub()
{
	printf("SetBridgeWindowChildStub\n");
	return TRUE;
}
BOOL SetFallbackForegroundStub()
{
	printf("SetFallbackForegroundStub\n");
	return TRUE;
}
BOOL SetWindowArrangementStub()
{
	printf("SetWindowArrangementStub\n");
	return TRUE;
}
BOOL SetWindowGroupStub()
{
	printf("SetWindowGroupStub\n");
	return TRUE;
}

BOOL SetWindowShowStateStub()
{
	printf("SetWindowShowStateStub\n");
	return TRUE;
}

BOOL UpdateWindowTrackingInfoStub()
{
	printf("UpdateWindowTrackingInfoStub\n");
	return TRUE;
}

BOOL AllowSetForegroundWindowStub()
{
	printf("AllowSetForegroundWindowStub\n");
	return TRUE;
}


void hookApis()
{
	printf("hook IAM api\n");
	CreateWindowInBandOrig = decltype(CreateWindowInBandOrig)(GetProcAddress(GetModuleHandle(L"user32.dll"), "CreateWindowInBand"));
	CreateWindowInBandExOrig = decltype(CreateWindowInBandExOrig)(GetProcAddress(GetModuleHandle(L"user32.dll"), "CreateWindowInBandEx"));
	SetWindowBandApiOrg = decltype(SetWindowBandApiOrg)(GetProcAddress(GetModuleHandle(L"user32.dll"), "SetWindowBand"));
	RegisterHotKeyApiOrg = decltype(RegisterHotKeyApiOrg)(GetProcAddress(GetModuleHandle(L"user32.dll"), "RegisterHotKey"));

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach((LPVOID*)&CreateWindowInBandOrig, (PVOID)CreateWindowInBandNew);
	DetourAttach((LPVOID*)&CreateWindowInBandExOrig, (PVOID)CreateWindowInBandExNew);
	DetourAttach((LPVOID*)&SetWindowBandApiOrg, (PVOID)SetWindowBandNew);
	DetourAttach((LPVOID*)&RegisterHotKeyApiOrg, (PVOID)RegisterWindowHotkeyNew);


	PVOID junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2581); // GetWindowTrackInfoAsync
	DetourAttach((LPVOID*)&junk, (PVOID)GetWindowTrackInfoAsyncStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2563); // ClearForeground
	DetourAttach((LPVOID*)&junk, (PVOID)ClearForegroundStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2628); // CreateWindowGroup
	DetourAttach((LPVOID*)&junk, (PVOID)CreateWindowGroupStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2629); // DeleteWindowGroup
	DetourAttach((LPVOID*)&junk, (PVOID)DeleteWindowGroupStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2631); //EnableWindowGroupPolicy
	DetourAttach((LPVOID*)&junk, (PVOID)EnableWindowGroupPolicyStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2627); // SetBridgeWindowChild
	DetourAttach((LPVOID*)&junk, (PVOID)SetBridgeWindowChildStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2511); // SetFallbackForeground
	DetourAttach((LPVOID*)&junk, (PVOID)SetFallbackForegroundStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2566); // SetWindowArrangement
	DetourAttach((LPVOID*)&junk, (PVOID)SetWindowArrangementStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2632); // SetWindowGroup
	DetourAttach((LPVOID*)&junk, (PVOID)SetWindowGroupStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2579); // SetWindowShowState
	DetourAttach((LPVOID*)&junk, (PVOID)SetWindowShowStateStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), (LPCSTR)2585); // UpdateWindowTrackingInfo
	DetourAttach((LPVOID*)&junk, (PVOID)UpdateWindowTrackingInfoStub);
	junk = GetProcAddress(GetModuleHandle(L"user32.dll"), "AllowSetForegroundWindow");
	DetourAttach((LPVOID*)&junk, (PVOID)AllowSetForegroundWindowStub);

	DetourTransactionCommit();

	printf("hooked IAM apis\n");
}

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
	printf("Starting it\n");

	hookApis();

	//create the shell object
	CustomShell* object = new CustomShell();
	if (FAILED(object->Run()))
	{
		printf("Run failed\n");
	}
	system("pause");

	return -1;
}

int main()
{
	hookApis();
	MainHook(GetModuleHandleA(NULL), NULL, NULL, 0);
	return 0;
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