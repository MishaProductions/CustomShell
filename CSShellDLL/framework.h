#pragma once

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#define _CRT_SECURE_NO_WARNINGS 1
// Windows Header Files
#include <windows.h>
#include <mutex>
#include <psapi.h>
#include <Shobjidl.h>
#include <wrl.h>
#include <Shlwapi.h>

//INSTALL DETOURS FROM NUGET! (or build from source yourself)
#include <detours.h>
#include <iostream>