#include "CustomShell.h"
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

long PcRef = 0;
IUnknown* ThreadObject = NULL;


CustomShell::CustomShell()
{

}
#pragma data_seg(.imrsiv)

LRESULT ProgmanWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	if (msg == WM_CREATE)
	{
		auto user32 = LoadLibrary(TEXT("user32.dll"));
		if (user32 == NULL)
		{
			printf("Failed to load user32.dll\n");
		}
		auto SetShellWindowFunction = (SetShellWindow)GetProcAddress(user32, "SetShellWindow");

		if (SetShellWindowFunction == NULL)
		{
			printf("failed to get setshellwindow pointer\n");
			return 0;
		}

		if (FAILED(SetShellWindowFunction(hwnd)))
		{
			printf("SetShellWindow failed\n");
			return 0;
		}
		SendMessageW(hwnd, WM_CHANGEUISTATE, 3u, 0);
		SendMessageW(hwnd, WM_UPDATEUISTATE, 0x10001u, 0);

		SetProp(hwnd, TEXT("NonRudeHWND"), (HANDLE)HANDLE_FLAG_INHERIT);
		SetProp(hwnd, TEXT("AllowConsentToStealFocus"), (HANDLE)HANDLE_FLAG_INHERIT);


		if (SUCCEEDED(SHCreateThreadRef(&PcRef, &ThreadObject)))
		{
			SHSetThreadRef(ThreadObject);
		}
		else
		{
			printf("Failed to create thread reference");
		}
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
		FillRect(dc, &Rect, CreateSolidBrush(RGB(0, 255, 0)));
		EndPaint(hwnd, &Paint);
		return 0;
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
HRESULT CustomShell::Run()
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
#pragma region  get function pointers
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

#pragma endregion


	printf("Initialize explorer\n");
	int hr = InitStuff();
	if (FAILED(hr)) return hr;


	//Create program manager

	WNDCLASSEX progmanclass;

	progmanclass.cbClsExtra = 0;
	progmanclass.hIcon = 0;
	progmanclass.lpszMenuName = 0;
	progmanclass.hIconSm = 0;
	progmanclass.cbSize = 80;
	progmanclass.style = 8;
	progmanclass.lpfnWndProc = (WNDPROC)ProgmanWndProc;
	progmanclass.cbWndExtra = 8;
	progmanclass.hInstance = GetModuleHandle(NULL);
	progmanclass.hCursor = LoadCursor(NULL, IDC_ARROW);
	progmanclass.hbrBackground = (HBRUSH)2;
	progmanclass.lpszClassName = TEXT("Progman");

	if (!RegisterClassExW(&progmanclass))
	{
		printf("failed to register progman class %d", GetLastError());
		return -1;
	}
	printf("Create program manager\n");
	auto Progman = CreateWindowExW(128, L"Progman", TEXT("Program Manager"), 0x82000000, GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN), GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN), 0, 0, progmanclass.hInstance, 0);
	ShowWindow(Progman, SW_SHOW);

	printf("create taskman\n");
	//create taskman class (handles taskbar buttons)
	WNDCLASSEX taskmanclass;

	taskmanclass.cbClsExtra = 0;
	taskmanclass.hIcon = 0;
	taskmanclass.lpszMenuName = 0;
	taskmanclass.hIconSm = 0;
	taskmanclass.cbSize = sizeof(WNDCLASSEXW);
	taskmanclass.style = 8;
	taskmanclass.lpfnWndProc = (WNDPROC)TaskmanWndProc;
	taskmanclass.cbWndExtra = 8;
	taskmanclass.hInstance = GetModuleHandle(NULL);
	taskmanclass.hCursor = LoadCursor(NULL, IDC_ARROW);
	taskmanclass.hbrBackground = (HBRUSH)2;
	taskmanclass.lpszClassName = TEXT("TaskmanWndClass");

	if (!RegisterClassExW(&taskmanclass))
	{
		printf("failed to register taskman class %d", GetLastError());
		return -1;
	}
	auto Taskman = CreateWindowW(L"TaskmanWndClass", NULL, 0, 0, 0, 0, 0, 0, 0, 0, 0);


	//create immersive

	IImmersiveShellBuilder* ImmersiveShellBUilder = NULL;
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
	IImmersiveShellController* controller = NULL;
	hr = ImmersiveShellBUilder->CreateImmersiveShellController(&controller); //CImmersiveShellController




	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell controller: %d\n", hr);
		system("pause");
		return hr;
	}

	IUnknown* ppv = NULL;
	int a = CoCreateInstance(guidImmersiveShell, 0, 0x404u, IID_IServiceProvider, (LPVOID*)&ppv);



	hr = controller->Start();

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

HRESULT CustomShell::InitStuff()
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
	return S_OK;
}

