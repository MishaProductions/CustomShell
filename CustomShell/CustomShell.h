#pragma once
#include <windows.h>
#include <Shobjidl.h>
#include <wrl.h>
#include <Shlwapi.h>
class CustomShell
{
public:
	 CustomShell();
	 HRESULT Run();
	 HRESULT InitStuff();

private:

};

