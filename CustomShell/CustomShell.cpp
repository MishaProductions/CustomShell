#include "CustomShell.h"
#include <stdio.h>
#include <wtsapi32.h>
#include "undoc.h"

GetTaskmanWindow GetTaskmanWindowFunc = NULL;
SetTaskmanWindow SetTaskmanWindowFunc = NULL;
SetExplorerServerMode SetExplorerServerModeFunc = NULL;
SCNSystemInitialize SCNSystemInitializeFunc = NULL;
ShellDDEInit ShellDDEInitFunc = NULL;
SetShellWindow SetShellWindowFunc = NULL;

AudioHIDInitialize AudioHIDInitializeFunc = NULL;
AudioHIDShutdown AudioHIDShutdownFunc = NULL;
AudioHIDProcessMessage AudioHIDProcessMessageFunc = NULL;
AudioHIDProcessAppCommand AudioHIDProcessAppCommandFunc = NULL;

long PcRef = 0;
IUnknown* ThreadObject = NULL;
UINT shellhook = 0;

IImmersiveShellHookService* ShellHookService;

#pragma data_seg(.imrsiv) long x=0;

CustomShell::CustomShell()
{

}

LRESULT ProgmanWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	if (msg == WM_CREATE)
	{
		if (FAILED(SetShellWindowFunc(hwnd)))
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
	else if (msg == WM_DESTROY)
	{
		if (GetShellWindow() == hwnd)
		{
			RemovePropW(hwnd, L"AllowConsentToStealFocus");
			RemovePropW(hwnd, L"NonRudeHWND");
			SetShellWindowFunc(0);
			WTSUnRegisterSessionNotification(hwnd);
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
		FillRect(dc, &Rect, CreateSolidBrush(RGB(0, 36, 0)));
		EndPaint(hwnd, &Paint);
		return 0;
	}
	else if (msg == WM_TIMER)
	{
		printf("timer!\n");
	}
	return DefWindowProc(hwnd, msg, w, l);
}

LRESULT TaskmanWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	if (msg == WM_CREATE)
	{
		shellhook = RegisterWindowMessageW(L"SHELLHOOK");
		if (!shellhook)
		{
			printf("failed to register shellhook\n");
		}
		if (!SetTaskmanWindowFunc(hwnd))
		{
			printf("failed to register taskman window\n");
		}
		if (!RegisterShellHookWindow(hwnd))
		{
			printf("register shellhook window failed\n");
		}

	}
	else if (msg == WM_DESTROY)
	{
		if (GetTaskmanWindowFunc() == hwnd)
		{
			SetTaskmanWindowFunc(NULL);
		}
		DeregisterShellHookWindow(hwnd);
	}
	else
	{
		if (msg != shellhook && msg != WM_HOTKEY)
		{
			return DefWindowProc(hwnd, msg, w, l);
		}

		if (ShellHookService)
		{
			BOOL handle = TRUE;
			if ((UINT)w == 12)
			{
				ShellHookService->SetTargetWindowForSerialization((HWND)l);
			}
			else if ((UINT)w == 0x32)
			{
				printf("not handling this\n");
				handle = FALSE;
			}
			if (handle)
			{
				ShellHookService->PostShellHookMessage(w, l);
			}
			return 0;
		}

		GUID guidImmersiveShell;
		CLSIDFromString(L"{c2f03a33-21f5-47fa-b4bb-156362a2f239}", &guidImmersiveShell);

		GUID SID_ImmersiveShellHookService;
		CLSIDFromString(L"{4624bd39-5fc3-44a8-a809-163a836e9031}", &SID_ImmersiveShellHookService);

		GUID SID_Unknown;
		CLSIDFromString(L"{914d9b3a-5e53-4e14-bbba-46062acb35a4}", &SID_Unknown);

		IServiceProvider* ImmersiveShell;
		if (CoCreateInstance(guidImmersiveShell, 0, 0x404u, IID_IServiceProvider, (LPVOID*)&ImmersiveShell) >= 0)
		{
			printf("created COM immersive shell thingy\n");

			if (FAILED(ImmersiveShell->QueryService(SID_ImmersiveShellHookService, SID_Unknown, (void**)&ShellHookService)))
			{
				printf("failed to get service instance of SID_ImmersiveShellHookService\n");
			}
		}
		else
		{
			printf("failed to create immersive shell class: %d\n", GetLastError());
		}
	}
	return DefWindowProc(hwnd, msg, w, l);
}

LRESULT TrayWndProc(HWND hwnd, UINT msg, WPARAM w, LPARAM l)
{
	return DefWindowProc(hwnd, msg, w, l);
}

HRESULT CustomShell::Run()
{
	//Setup our functions
	auto user32 = LoadLibrary(TEXT("user32.dll"));
	auto shell32 = LoadLibrary(TEXT("shell32.dll"));
	auto shdocvw = LoadLibrary(TEXT("shdocvw.dll"));
	auto SndVolSSO = LoadLibrary(TEXT("SndVolSSO.dll"));

	if (!user32)
	{
		printf("Failed to load user32.dll\n");
		return GetLastError();
	}
	if (!shell32)
	{
		printf("Failed to load shell32.dll\n");
		return GetLastError();
	}
	if (!shdocvw)
	{
		printf("Failed to load shdocvw.dll\n");
		return GetLastError();
	}
	if (!SndVolSSO)
	{
		printf("Failed to load SndVolSSO.dll\n");
		return GetLastError();
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

	SetShellWindowFunc = (SetShellWindow)GetProcAddress(user32, "SetShellWindow");
	if (SetShellWindowFunc == NULL)
	{
		printf("failed to get pointer to SetShellWindow()\n");
		return GetLastError();
	}

	AudioHIDInitializeFunc = (AudioHIDInitialize)GetProcAddress(SndVolSSO, (LPCSTR)1);
	if (AudioHIDInitializeFunc == NULL)
	{
		printf("failed to get pointer to AudioHIDInitialize()\n");
		return GetLastError();
	}

	AudioHIDInitializeFunc = (AudioHIDInitialize)GetProcAddress(SndVolSSO, (LPCSTR)1);
	if (AudioHIDInitializeFunc == NULL)
	{
		printf("failed to get pointer to AudioHIDInitialize()\n");
		return GetLastError();
	}

	AudioHIDShutdownFunc = (AudioHIDShutdown)GetProcAddress(SndVolSSO, (LPCSTR)2);
	if (AudioHIDShutdownFunc == NULL)
	{
		printf("failed to get pointer to AudioHIDShutdown()\n");
		return GetLastError();
	}

	AudioHIDProcessMessageFunc = (AudioHIDProcessMessage)GetProcAddress(SndVolSSO, (LPCSTR)3);
	if (AudioHIDProcessMessageFunc == NULL)
	{
		printf("failed to get pointer to AudioHIDProcessMessage()\n");
		return GetLastError();
	}

	AudioHIDProcessAppCommandFunc = (AudioHIDProcessAppCommand)GetProcAddress(SndVolSSO, (LPCSTR)4);
	if (AudioHIDProcessAppCommandFunc == NULL)
	{
		printf("failed to get pointer to AudioHIDProcessAppCommand()\n");
		return GetLastError();
	}
#pragma endregion


	printf("Initialize explorer\n");
	int hr = InitStuff();
	if (FAILED(hr)) return hr;


	// Create program manager
	printf("Create program manager\n");

	WNDCLASSEX progmanclass = {};
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

	auto Progman = CreateWindowExW(128, L"Progman", TEXT("Program Manager"), 0x82000000, GetSystemMetrics(SM_XVIRTUALSCREEN), GetSystemMetrics(SM_YVIRTUALSCREEN), GetSystemMetrics(SM_CXVIRTUALSCREEN), GetSystemMetrics(SM_CYVIRTUALSCREEN), 0, 0, progmanclass.hInstance, 0);


	// create taskman class (handles taskbar buttons)
	printf("create taskman\n");
	WNDCLASSEX taskmanclass = {};

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
	auto Taskman = CreateWindowExW(0, L"TaskmanWndClass", NULL, 0x82000000, 0, 0, 0, 0, 0, 0, 0, 0);

	//create immersive shell
	IImmersiveShellBuilder* ImmersiveShellBuilder = NULL;

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
		(LPVOID*)&ImmersiveShellBuilder);

	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell builder: %d\n", hr);
		system("pause");
		return hr;
	}
	IImmersiveShellController* controller = NULL;
	hr = ImmersiveShellBuilder->CreateImmersiveShellController(&controller); //CImmersiveShellController
	if (FAILED(hr))
	{
		printf("Failed to create the immersive shell controller: %d\n", hr);
		system("pause");
		return hr;
	}



	hr = controller->Start();


	if (FAILED(hr))
	{
		printf("immersive shell start failed with %d\n", hr);
	}
	else
	{
		printf("Immersive shell created. Starting message loop\n");

		if (!CreateEventW(0, TRUE, 0, L"Local\\ShellStartupEvent"))
		{
			printf("Failed to create start event: %d\n", GetLastError());

		}

		HANDLE StartEvent = OpenEvent(2, FALSE, TEXT("Local\\ShellStartupEvent"));
		if (!StartEvent)
		{
			printf("Failed to open start event: %d\n", GetLastError());
		}
		if (!SetEvent(StartEvent))
		{
			printf("Failed to set start event: %d\n", GetLastError());
		}
		//create the tray window
		printf("create tray\n");
		WNDCLASSEX trayclass = {};

		trayclass.cbClsExtra = 0;
		trayclass.hIcon = 0;
		trayclass.lpszMenuName = 0;
		trayclass.hIconSm = 0;
		trayclass.cbSize = sizeof(WNDCLASSEXW);
		trayclass.style = 8;
		trayclass.lpfnWndProc = (WNDPROC)TrayWndProc;
		trayclass.cbWndExtra = 8;
		trayclass.hInstance = GetModuleHandle(NULL);
		trayclass.hCursor = LoadCursor(NULL, IDC_ARROW);
		trayclass.hbrBackground = (HBRUSH)2;
		trayclass.lpszClassName = TEXT("Shell_TrayWnd");

		if (!RegisterClassExW(&trayclass))
		{
			printf("failed to register taskman class %d", GetLastError());
			return -1;
		}
		auto tray = CreateWindowExW(384, L"Shell_TrayWnd", NULL, 0x82000000, 0, 0, 0, 0, 0, 0, 0, 0);


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
	MSG Msg = {};
	SetCurrentProcessExplicitAppUserModelID(L"Microsoft.Windows.Explorer");
	SetErrorMode(0x4001);
	SetPriorityClass(GetCurrentProcess(), 0x80);
	EnableMouseInPointer(FALSE);
	SetExplorerServerModeFunc(3);
	SCNSystemInitializeFunc();
	SetPriorityClass(GetCurrentProcess(), 0x20);


	ShellDDEInitFunc(TRUE);
	return S_OK;
}

