#pragma once
#include <windows.h>
#include <Shobjidl.h>
#include <wrl.h>
#include <Shlwapi.h>
class ShellObject
{
public:
	 ShellObject();
	 HRESULT RunShellToCompletion();
	 HRESULT DoLegacyInitialization();
};

