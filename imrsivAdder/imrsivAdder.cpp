#include <windows.h>
#include <stdio.h>
#include "Shlwapi.h"
#pragma comment(lib, "gdi32.lib")

enum ZBID
{
	ZBID_DEFAULT = 0,
	ZBID_DESKTOP = 1,
	ZBID_UIACCESS = 2,
	ZBID_IMMERSIVE_IHM = 3,
	ZBID_IMMERSIVE_NOTIFICATION = 4,
	ZBID_IMMERSIVE_APPCHROME = 5,
	ZBID_IMMERSIVE_MOGO = 6,
	ZBID_IMMERSIVE_EDGY = 7,
	ZBID_IMMERSIVE_INACTIVEMOBODY = 8,
	ZBID_IMMERSIVE_INACTIVEDOCK = 9,
	ZBID_IMMERSIVE_ACTIVEMOBODY = 10,
	ZBID_IMMERSIVE_ACTIVEDOCK = 11,
	ZBID_IMMERSIVE_BACKGROUND = 12,
	ZBID_IMMERSIVE_SEARCH = 13,
	ZBID_GENUINE_WINDOWS = 14,
	ZBID_IMMERSIVE_RESTRICTED = 15,
	ZBID_SYSTEM_TOOLS = 16,
	ZBID_LOCK = 17,
	ZBID_ABOVELOCK_UX = 18,
};

LRESULT CALLBACK TrashParentWndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_CREATE:
		break;

	case WM_DESTROY:
		PostQuitMessage(0);
		break;

	case WM_WINDOWPOSCHANGING:
		return 0;
	case WM_CLOSE:
		HANDLE myself;
		myself = OpenProcess(PROCESS_ALL_ACCESS, false, GetCurrentProcessId());
		TerminateProcess(myself, 0);

		return true;

		break;

	default:
		break;
	}

	return DefWindowProc(hwnd, message, wParam, lParam);
}

HWND hwnd = NULL;
typedef HWND(WINAPI* CreateWindowInBand)(_In_ DWORD dwExStyle, _In_opt_ ATOM atom, _In_opt_ LPCWSTR lpWindowName, _In_ DWORD dwStyle, _In_ int X, _In_ int Y, _In_ int nWidth, _In_ int nHeight, _In_opt_ HWND hWndParent, _In_opt_ HMENU hMenu, _In_opt_ HINSTANCE hInstance, _In_opt_ LPVOID lpParam, DWORD band);

BOOL CreateWin(HMODULE hModule, UINT zbid, const wchar_t* title, const wchar_t* classname)
{
	{
		HINSTANCE hInstance = hModule;

		WNDCLASSEX wndParentClass = {};
		wndParentClass.cbSize = sizeof(WNDCLASSEX);

		wndParentClass.cbClsExtra = 0;
		wndParentClass.hIcon = NULL;
		wndParentClass.lpszMenuName = NULL;
		wndParentClass.hIconSm = NULL;
		wndParentClass.lpfnWndProc = TrashParentWndProc;
		wndParentClass.hInstance = hInstance;
		wndParentClass.style = CS_HREDRAW | CS_VREDRAW;
		wndParentClass.hCursor = LoadCursor(0, IDC_ARROW);
		wndParentClass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
		wndParentClass.lpszClassName = classname;

		auto res = RegisterClassEx(&wndParentClass);

		const auto hpath = LoadLibrary(L"user32.dll");
		const auto pCreateWindowInBand = CreateWindowInBand(GetProcAddress(hpath, "CreateWindowInBand"));
		printf("calling the function\n");
		auto hwndParent = pCreateWindowInBand(WS_EX_TOPMOST | WS_EX_NOACTIVATE,
			res,
			NULL,
			0x80000000,
			0, 0, 0, 0,
			NULL,
			NULL,
			wndParentClass.hInstance,
			LPVOID(res),
			zbid);

		if (hwndParent == NULL)
		{
			printf("CreateWindowInBand failure: %d", GetLastError());
			return FALSE;
		}

		SetWindowLong(hwndParent, GWL_STYLE, 0);
		SetWindowLong(hwndParent, GWL_EXSTYLE, 0);

		SetWindowPos(hwndParent, nullptr, 40, 40, 600, 600, SWP_SHOWWINDOW | SWP_NOZORDER);
		ShowWindow(hwndParent, SW_SHOW);
		UpdateWindow(hwndParent);

		if (hwndParent != nullptr)
			hwnd = hwndParent;
		return TRUE;
	}
}

DWORD WINAPI Thrd(LPVOID lpParam)
{
	if (CreateWin(NULL, ZBID_SYSTEM_TOOLS, L"Really Genuine Window++", L"TestPlus"))
	{
		MSG msg;
		while (GetMessage(&msg, nullptr, 0, 0))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}

		return 0;
	}

	
}
typedef BOOL(WINAPI* NtUserClearForeground)();

typedef BOOL(WINAPI* NtUserEnableIAMAccess)( //2510
	IN ULONG64 key,
	IN BOOL enable);

typedef BOOL(WINAPI* NtUserAcquireIAMKey)( //2509
	OUT ULONG64* pkey);
typedef HRESULT(CALLBACK* SetShellWindow)(HWND hwnd);
LONG PcRef = 0;
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


		if (SUCCEEDED(SHCreateThreadRef(&PcRef, &thing2)))
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

int main()
{
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
		printf("failed to register progman class %d\n", GetLastError());
		return -1;
	}
	printf("create progman\n");
	auto Progman = CreateWindowW(L"Progman", TEXT("Program Manager"), 0, 0, 0, 0, 0, 0, 0, 0, 0);
	//Thrd(NULL);

	auto lib = LoadLibrary(TEXT("user32.dll"));

	auto clr = (NtUserClearForeground)GetProcAddress(lib, (LPCSTR)2563);
	auto get = (NtUserAcquireIAMKey)GetProcAddress(lib, (LPCSTR)2509);
	auto enable = (NtUserEnableIAMAccess)GetProcAddress(lib, (LPCSTR)2510);
	ULONG64 key;
	if (!get(&key))
	{
		printf("failed to get key: %d\n", GetLastError());
		return -1;
	}
	printf("key: %d\n", key);
	if (!enable(key, TRUE))
	{
		printf("failed to enable access\n");
		return -1;
	}
	if (!clr())
	{
		printf("failure: %d\n", GetLastError());
		system("pause");
	}
	else
	{
		printf("OK\n");
	}
}