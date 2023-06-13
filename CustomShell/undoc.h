#pragma once
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

typedef BOOL(WINAPI* AudioHIDInitialize)(HWND handle);
typedef BOOL(WINAPI* AudioHIDShutdown)();
typedef BOOL(WINAPI* AudioHIDProcessMessage)(UINT uMsg, WPARAM wParam, LPARAM lParam);
typedef BOOL(WINAPI* AudioHIDProcessAppCommand)(UINT appCommand);


interface IImmersiveShellController : IUnknown
{
	virtual HRESULT Start();
	virtual HRESULT Stop(void* unknown);
	virtual HRESULT SetCreationBehavior(void* structure);

};
interface IImmersiveShellBuilder : IUnknown
{
	virtual HRESULT CreateImmersiveShellController(IImmersiveShellController** other);
};

interface IImmersiveShellHookService : IUnknown
{
	virtual HRESULT Register(void** a1,
		IImmersiveShellHookService* thiss,
		const unsigned int* prgMessages,
		unsigned int cMessages,
		IUnknown* pNotification, //IImmersiveShellHookNotification
		unsigned int* pdwCookie);//todo:args
	virtual HRESULT Unregister(UINT cookie);
	virtual HRESULT PostShellHookMessage(WPARAM wParam, LPARAM lParam);
	virtual HRESULT SetTargetWindowForSerialization(HWND hwnd);
	virtual HRESULT PostShellHookMessageWithSerialization(bool a1,
		int a2,
		IImmersiveShellHookService* thiss,
		unsigned int msg,
		int msgParam); //todo:args
	virtual HRESULT UpdateWindowApplicationId(HWND hwnd, LPCWSTR pszAppID);
	virtual HRESULT HandleWindowReplacement(HWND hwndOld, HWND hwndNew); 
	virtual BOOL IsExecutionOnSerializedThread();
};

interface IImmersiveWindowMessageService : IUnknown
{
	virtual HRESULT Register(UINT msg, void* pNotification, UINT* pdwCookie);
	virtual HRESULT Unregister(UINT dwCookie);
	virtual HRESULT SendMessageW(UINT nsg, WPARAM wParam, LPARAM lParam);
	virtual HRESULT PostMessageW(UINT nsg, WPARAM wParam, LPARAM lParam);
	virtual HRESULT RequestHotkeys(); //todo: args
	virtual HRESULT UnrequestHotkeys(UINT dwCookie);
	virtual HRESULT RequestWTSSessionNotification(void* pNotification, unsigned int* pdwCookie);
	virtual HRESULT UnrequestWTSSessionNotification(UINT dwCookie);
	virtual HRESULT RequestPowerSettingNotification(const GUID* pPowerSettingGuid, void* pNotification, UINT* pdwCookie);
	virtual HRESULT UnrequestPowerSettingNotification(UINT pdwCookie);
	virtual HRESULT RequestPointerDeviceNotification(void* pNotification, int notificationType, UINT* pdwCookie);
	virtual HRESULT UnrequestPointerDeviceNotification(UINT dwCookie);
	virtual HRESULT RegisterDwmIconicThumbnailWindow();
};