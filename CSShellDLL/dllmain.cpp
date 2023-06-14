#include "pch.h"
#include <stdio.h>
#include <stdint.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <iostream>
using namespace std;
using string_t = std::basic_string<char_t>;

//Most of the code is copied from: https://github.com/dotnet/samples/blob/main/core/hosting/src/NativeHost/nativehost.cpp#L172

/********************************************************************************************
 * Function used to load and activate .NET Core
 ********************************************************************************************/

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

	if (FAILED(CoInitializeEx(NULL, 2)))
	{
		printf("Failed to initialize COM\n");
		return -1;
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
	const string_t root_path = L"C:\\Users\\Misha\\OneDrive\\_RE\\CPP\\Win10\\customshellhost\\CustomShell\\x64\\Debug\\";
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
		const char_t* message;
		int number;
	};
	// <SnippetCallManaged>
	lib_args args
	{
		L"from host!",
		0
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
	printf("STUB FUNCTION: HamCloseActivity\n");
}


void HamPopulateActivityProperties()
{
	printf("STUB FUNCTION: HamPopulateActivityProperties\n");
}

void HamCreateActivityForProcess()
{
	printf("STUB FUNCTION: HamCreateActivityForProcess\n");
}
void HamStartActivityAsync()
{
	printf("STUB FUNCTION: HamStartActivityAsync\n");
}
void HamConnectToServer()
{
	printf("STUB FUNCTION: HamConnectToServer\n");
}
void HamDisconnectFromServer()
{
	printf("STUB FUNCTION: HamDisconnectFromServer\n");
}