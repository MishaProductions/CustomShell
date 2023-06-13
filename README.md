# Custom shell
An attempt of making UWP apps work using a custom shell.

# How to use
Build the "customshell" project in Visual Studio 2022, and copy runtimebroker.exe as ms.exe to the output folder and open it. Make sure to kill explorer before, and set ms.exe as the shell in the winlogon registry key. The driver is no longer used as this is an easier method.