#include "ShellObject.h"
#include <stdio.h>

typedef HRESULT(CALLBACK* SetExplorerServerMode)(DWORD flags);
typedef HRESULT(CALLBACK* ShellDDEInit)(BOOL register);
typedef HRESULT(CALLBACK* SCNSystemInitialize)();
typedef HRESULT(CALLBACK* SetShellWindow)(HWND hwnd);
typedef BOOL(WINAPI* NtUserAcquireIAMKey)(
	OUT ULONG64* pkey);
typedef BOOL(WINAPI* NtUserEnableIAMAccess)(
	IN ULONG64 key,
	IN BOOL enable);
typedef HWND(WINAPI* GetTaskmanWindow)();
typedef BOOL(WINAPI* SetTaskmanWindow)(HWND handle);

interface IImmersiveShellController : IUnknown
{
	virtual int Start();
	virtual int Stop(void* unknown);
	virtual int SetCreationBehavior(void* structure);

};
interface IImmersiveShellBuilder : IUnknown
{
	virtual int CreateImmersiveShellController(IImmersiveShellController** other);
};

GetTaskmanWindow GetTaskmanWindowFunc = NULL;
SetTaskmanWindow SetTaskmanWindowFunc = NULL;
SetExplorerServerMode SetExplorerServerModeFunc = NULL;
SCNSystemInitialize SCNSystemInitializeFunc = NULL;
ShellDDEInit ShellDDEInitFunc = NULL;

#pragma section(".imrsiv",read)
int j = 0;


ShellObject::ShellObject()
{

}
#pragma data_seg(.imrsiv)
long thing = 0;
IUnknown* thing2 = NULL;
LRESULT StaticWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	if (msg == WM_CREATE)
	{
		auto user32 = LoadLibrary(TEXT("user32.dll"));
		if (user32 == NULL)
		{
			printf("Failed to load user32.dll\n");
		}
		auto proc2 = (SetShellWindow)GetProcAddress(user32, "SetShellWindow");

		if (proc2 == NULL)
		{
			printf("failed to get setshellwindow pointer\n");
			return 0;
		}

		if (FAILED(proc2(hwnd)))
		{
			printf("SetShellWindow failed\n");
			return 0;
		}

		SendMessageW(hwnd, 295u, 3ui64, 0i64);    // WM_CHANGEUISTATE
		SendMessageW(hwnd, 296u, 0x10001ui64, 0i64);// WM_UPDATEUISTATE

		SetPropW(hwnd, L"AllowConsentToStealFocus", (HANDLE)HANDLE_FLAG_INHERIT);
		SetPropW(hwnd, L"NonRudeHWND", (HANDLE)HANDLE_FLAG_INHERIT);


		if (SUCCEEDED(SHCreateThreadRef(&thing, &thing2)))
		{
			SHSetThreadRef(thing2);
		}
		else
		{
			printf("Failed to create thread reference");
		}

		//auto proc3 = (NtUserAcquireIAMKey)GetProcAddress(user32, (LPCSTR)2509); //NtUserAcquireIAMKey
		//ULONG64 key = 0;
		//proc3(&key);
		//
		//if (key == 0)
		//{
		//	printf("Failed to get IAM key\n");
		//	return 0;
		//}

		//auto proc4 = (NtUserEnableIAMAccess)GetProcAddress(user32, (LPCSTR)2510); //NtUserEnableIAMAccess
		//if (!proc4(key, TRUE))
		//{
		//	printf("Failed to enable IAM with key\n");
		//}
		//else {
		//	printf("successfully enabled IAM\n");
		//}
	}
	else if (msg == WM_SIZE)
	{
		ShowWindow(hwnd, 5);
	}
	else if (msg == WM_CLOSE)
	{
		return -1;
	}
	else if (msg == WM_PAINT)
	{
		PAINTSTRUCT Paint;
		RECT Rect;
		HDC dc = BeginPaint(hwnd, &Paint);
		GetClientRect(hwnd, &Rect);
		//DWORD SysColor = GetSysColor(1);
		//SHFillRectClr(dc, &Rect, SysColor);
		FillRect(dc, &Rect, GetSysColorBrush(1));
		EndPaint(hwnd, &Paint);
		return 0;
	}
	else if (msg == 537)
	{
		if (w == 24 || w == 32772 || w == 0x8000)
		{
			//PostMessageW(*((HWND*)this + 6), 0x44Bu, 0i64, 0i64);

			return 0;
		}
	}
	return DefWindowProc(hwnd, msg, w, l);
}

LRESULT TaskmanWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	if (msg == WM_CREATE)
	{
		if (!SetTaskmanWindowFunc(hwnd))
		{
			printf("failed to register taskman window\n");
		}
		RegisterShellHookWindow(hwnd);
	}
	else if (msg == WM_DESTROY)
	{
		if (GetTaskmanWindowFunc() == hwnd)
		{
			SetTaskmanWindowFunc(NULL);
		}
		DeregisterShellHookWindow(hwnd);
	}

	return DefWindowProc(hwnd, msg, w, l);
}
HRESULT ShellObject::RunShellToCompletion()
{
	//Setup our functions
	auto user32 = LoadLibrary(TEXT("user32.dll"));
	auto shell32 = LoadLibrary(TEXT("shell32.dll"));
	auto shdocvw = LoadLibrary(TEXT("shdocvw.dll"));
	if (user32 == NULL)
	{
		printf("Failed to load user32.dll\n");
	}
	if (shell32 == NULL)
	{
		printf("Failed to load shell32.dll\n");
	}
	if (shdocvw == NULL)
	{
		printf("Failed to load shdocvw.dll\n");
	}


	GetTaskmanWindowFunc = (GetTaskmanWindow)GetProcAddress(user32, "GetTaskmanWindow");
	if (GetTaskmanWindowFunc == NULL)
	{
		printf("failed to get pointer to GetTaskmanWindow()\n");
		return GetLastError();
	}
	SetTaskmanWindowFunc = (SetTaskmanWindow)GetProcAddress(user32, "SetTaskmanWindow");
	if (SetTaskmanWindowFunc == NULL)
	{
		printf("failed to get pointer to SetTaskmanWindow()\n");
		return GetLastError();
	}

	SetExplorerServerModeFunc = (SetExplorerServerMode)GetProcAddress(shell32, (LPCSTR)899);
	if (SetExplorerServerModeFunc == NULL)
	{
		printf("failed to get pointer to SetExplorerServerMode()\n");
		return GetLastError();
	}
	SCNSystemInitializeFunc = (SCNSystemInitialize)GetProcAddress(shell32, (LPCSTR)938);
	if (SCNSystemInitializeFunc == NULL)
	{
		printf("failed to get pointer to SCNSystemInit()\n");
		return GetLastError();
	}
	
	ShellDDEInitFunc = (ShellDDEInit)GetProcAddress(shell32, (LPCSTR)188);
	if (ShellDDEInitFunc == NULL)
	{
		printf("failed to get pointer to ShellDDEInit()\n");
		return GetLastError();
	}

	printf("Initialize explorer\n");
	int hr = DoLegacyInitialization();
	if (FAILED(hr)) return hr;


	//Create program manager
	IImmersiveShellBuilder* ImmersiveShellBUilder = NULL;

	WNDCLASSEXW progmanclass; // [rsp+60h] [rbp-58h] BYREF

	progmanclass.cbClsExtra = 0;
	progmanclass.hIcon = 0i64;
	progmanclass.lpszMenuName = 0i64;
	progmanclass.hIconSm = 0i64;
	progmanclass.cbSize = 80;
	progmanclass.style = 8;
	progmanclass.lpfnWndProc = (WNDPROC)StaticWndProc;
	progmanclass.cbWndExtra = 8;
	progmanclass.hInstance = GetModuleHandle(NULL);
	progmanclass.hCursor = LoadCursorW(0, (LPCWSTR)0x7F00);
	progmanclass.hbrBackground = (HBRUSH)2;
	progmanclass.lpszClassName = L"Progman";

	if (!RegisterClassExW(&progmanclass))
	{
		printf("failed to register progman class %d", GetLastError());
		return -1;
	}
	printf("create progman\n");
	auto Progman = CreateWindowW(L"Progman", TEXT("Program Manager"), 0, 0, 0, 0, 0, 0, 0, 0, 0);


	printf("create taskman\n");
	//create taskman class (handles taskbar buttons)
	WNDCLASSEXW taskmanclass; // [rsp+60h] [rbp-58h] BYREF

	taskmanclass.cbClsExtra = 0;
	taskmanclass.hIcon = 0;
	taskmanclass.lpszMenuName = 0;
	taskmanclass.hIconSm = 0;
	taskmanclass.cbSize = sizeof(WNDCLASSEXW);
	taskmanclass.style = 8;
	taskmanclass.lpfnWndProc = (WNDPROC)TaskmanWndProc;
	taskmanclass.cbWndExtra = 8;
	taskmanclass.hInstance = GetModuleHandle(NULL);
	taskmanclass.hCursor = LoadCursorW(0, (LPCWSTR)0x7F00);
	taskmanclass.hbrBackground = (HBRUSH)2;
	taskmanclass.lpszClassName = L"TaskmanWndClass";

	if (!RegisterClassExW(&taskmanclass))
	{
		printf("failed to register taskman class %d", GetLastError());
		return -1;
	}
	auto Taskman = CreateWindowW(L"TaskmanWndClass", NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0);


	GUID guidImmersiveShell;
	CLSIDFromString(L"{c2f03a33-21f5-47fa-b4bb-156362a2f239}", &guidImmersiveShell); //imersive shell

	GUID guid;
	if (FAILED(CLSIDFromString(L"{c71c41f1-ddad-42dc-a8fc-f5bfc61df957}", &guid)))
	{
		printf("failed to read guid1\n");
		return -1;
	}
	GUID guid2;
	if (FAILED(CLSIDFromString(L"{1c56b3e4-e6ea-4ced-8a74-73b72c6bd435}", &guid2)))
	{
		printf("failed to read guid2\n");
		return -1;
	}
	printf("Initialize immersive shell\n");

	hr = CoCreateInstance(
		guid,
		0,
		1u,
		guid2,
		(LPVOID*)&ImmersiveShellBUilder);

	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell builder: %d\n", hr);
		system("pause");
		return hr;
	}
	IImmersiveShellController* thing = NULL;
	hr = ImmersiveShellBUilder->CreateImmersiveShellController(&thing); //CImmersiveShellController
	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell controller: %d\n", hr);
		system("pause");
		return hr;
	}
	hr = thing->Start();
	//HWND t2 = CreateWindowW(NULL, TEXT("TaskmanWndClass"), 0, 0, 0, 0, 0, 0, 0, 0, 0);
	if (FAILED(hr))
	{
		printf("immersive shell start failed with %d\n", hr);
	}
	else
	{
		printf("Immersive shell created. Starting message loop\n");

		MSG msg = { };
		while (TRUE)
		{
			while (!GetMessage(&msg, NULL, 0, 0))
				WaitMessage();
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
		printf("exit!\n");
	}
	return S_OK;
}

HRESULT ShellObject::DoLegacyInitialization()
{
	MSG Msg;
	memset(&Msg, 0, sizeof(MSG));
	SetCurrentProcessExplicitAppUserModelID(L"Microsoft.Windows.Explorer");
	SetErrorMode(0x4001u);
	SetPriorityClass(GetCurrentProcess(), 0x80u);
	EnableMouseInPointer(FALSE);
	SetExplorerServerModeFunc(3);
	SCNSystemInitializeFunc();
	SetPriorityClass(GetCurrentProcess(), 0x20u);


	ShellDDEInitFunc(TRUE);

	SetProcessShutdownParameters(0x4FFu, 1u);

	PeekMessageW(&Msg, 0, 0x12u, 0x12u, 0);

	return S_OK;
}

