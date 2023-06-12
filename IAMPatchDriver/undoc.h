#include <ntifs.h>

//
// Undocumented Kernel (NT) APIs
//
extern
PVOID NTAPI RtlFindExportedRoutineByName(
	PVOID ImageBase,
	PCCH RoutineName
);
typedef enum _SYSTEM_INFORMATION_CLASS {
	SystemBasicInformation,
	SystemProcessorInformation,
	SystemPerformanceInformation,
	SystemTimeOfDayInformation,
	SystemPathInformation,
	SystemProcessInformation,
	SystemCallCountInformation,
	SystemDeviceInformation,
	SystemProcessorPerformanceInformation,
	SystemFlagsInformation,
	SystemCallTimeInformation,
	SystemModuleInformation,
	SystemLocksInformation,
	SystemStackTraceInformation,
	SystemPagedPoolInformation,
	SystemNonPagedPoolInformation,
	SystemHandleInformation,
	SystemObjectInformation,
	SystemPageFileInformation,
	SystemVdmInstemulInformation,
	SystemVdmBopInformation,
	SystemFileCacheInformation,
	SystemPoolTagInformation,
	SystemInterruptInformation,
	SystemDpcBehaviorInformation,
	SystemFullMemoryInformation,
	SystemLoadGdiDriverInformation,
	SystemUnloadGdiDriverInformation,
	SystemTimeAdjustmentInformation,
	SystemSummaryMemoryInformation,
	SystemMirrorMemoryInformation,
	SystemPerformanceTraceInformation,
	SystemObsolete0,
	SystemExceptionInformation,
	SystemCrashDumpStateInformation,
	SystemKernelDebuggerInformation,
	SystemContextSwitchInformation,
	SystemRegistryQuotaInformation,
	SystemExtendServiceTableInformation,
	SystemPrioritySeperation,
	SystemVerifierAddDriverInformation,
	SystemVerifierRemoveDriverInformation,
	SystemProcessorIdleInformation,
	SystemLegacyDriverInformation,
	SystemCurrentTimeZoneInformation,
	SystemLookasideInformation,
	SystemTimeSlipNotification,
	SystemSessionCreate,
	SystemSessionDetach,
	SystemSessionInformation,
	SystemRangeStartInformation,
	SystemVerifierInformation,
	SystemVerifierThunkExtend,
	SystemSessionProcessInformation,
	SystemLoadGdiDriverInSystemSpace,
	SystemNumaProcessorMap,
	SystemPrefetcherInformation,
	SystemExtendedProcessInformation,
	SystemRecommendedSharedDataAlignment,
	SystemComPlusPackage,
	SystemNumaAvailableMemory,
	SystemProcessorPowerInformation,
	SystemEmulationBasicInformation,
	SystemEmulationProcessorInformation,
	SystemExtendedHandleInformation,
	SystemLostDelayedWriteInformation,
	SystemBigPoolInformation,
	SystemSessionPoolTagInformation,
	SystemSessionMappedViewInformation,
	SystemHotpatchInformation,
	SystemObjectSecurityMode,
	SystemWatchdogTimerHandler,
	SystemWatchdogTimerInformation,
	SystemLogicalProcessorInformation,
	SystemWow64SharedInformationObsolete,
	SystemRegisterFirmwareTableInformationHandler,
	SystemFirmwareTableInformation,
	SystemModuleInformationEx,
	SystemVerifierTriageInformation,
	SystemSuperfetchInformation,
	SystemMemoryListInformation,
	SystemFileCacheInformationEx,
	SystemThreadPriorityClientIdInformation,
	SystemProcessorIdleCycleTimeInformation,
	SystemVerifierCancellationInformation,
	SystemProcessorPowerInformationEx,
	SystemRefTraceInformation,
	SystemSpecialPoolInformation,
	SystemProcessIdInformation,
	SystemErrorPortInformation,
	SystemBootEnvironmentInformation,
	SystemHypervisorInformation,
	SystemVerifierInformationEx,
	SystemTimeZoneInformation,
	SystemImageFileExecutionOptionsInformation,
	SystemCoverageInformation,
	SystemPrefetchPatchInformation,
	SystemVerifierFaultsInformation,
	SystemSystemPartitionInformation,
	SystemSystemDiskInformation,
	SystemProcessorPerformanceDistribution,
	SystemNumaProximityNodeInformation,
	SystemDynamicTimeZoneInformation,
	SystemCodeIntegrityInformation,
	SystemProcessorMicrocodeUpdateInformation,
	SystemProcessorBrandString,
	SystemVirtualAddressInformation,
	SystemLogicalProcessorAndGroupInformation,
	SystemProcessorCycleTimeInformation,
	SystemStoreInformation,
	SystemRegistryAppendString,
	SystemAitSamplingValue,
	SystemVhdBootInformation,
	SystemCpuQuotaInformation,
	SystemNativeBasicInformation,
	SystemErrorPortTimeouts,
	SystemLowPriorityIoInformation,
	SystemTpmBootEntropyInformation,
	SystemVerifierCountersInformation,
	SystemPagedPoolInformationEx,
	SystemSystemPtesInformationEx,
	SystemNodeDistanceInformation,
	SystemAcpiAuditInformation,
	SystemBasicPerformanceInformation,
	SystemQueryPerformanceCounterInformation,
	SystemSessionBigPoolInformation,
	SystemBootGraphicsInformation,
	SystemScrubPhysicalMemoryInformation,
	SystemBadPageInformation,
	SystemProcessorProfileControlArea,
	SystemCombinePhysicalMemoryInformation,
	SystemEntropyInterruptTimingInformation,
	SystemConsoleInformation,
	SystemPlatformBinaryInformation,
	SystemPolicyInformation,
	SystemHypervisorProcessorCountInformation,
	SystemDeviceDataInformation,
	SystemDeviceDataEnumerationInformation,
	SystemMemoryTopologyInformation,
	SystemMemoryChannelInformation,
	SystemBootLogoInformation,
	SystemProcessorPerformanceInformationEx,
	SystemCriticalProcessErrorLogInformation,
	SystemSecureBootPolicyInformation,
	SystemPageFileInformationEx,
	SystemSecureBootInformation,
	SystemEntropyInterruptTimingRawInformation,
	SystemPortableWorkspaceEfiLauncherInformation,
	SystemFullProcessInformation,
	SystemKernelDebuggerInformationEx,
	SystemBootMetadataInformation,
	SystemSoftRebootInformation,
	SystemElamCertificateInformation,
	SystemOfflineDumpConfigInformation,
	SystemProcessorFeaturesInformation,
	SystemRegistryReconciliationInformation,
	SystemEdidInformation,
	SystemManufacturingInformation,
	SystemEnergyEstimationConfigInformation,
	SystemHypervisorDetailInformation,
	SystemProcessorCycleStatsInformation,
	SystemVmGenerationCountInformation,
	SystemTrustedPlatformModuleInformation,
	SystemKernelDebuggerFlags,
	SystemCodeIntegrityPolicyInformation,
	SystemIsolatedUserModeInformation,
	SystemHardwareSecurityTestInterfaceResultsInformation,
	SystemSingleModuleInformation,
	SystemAllowedCpuSetsInformation,
	SystemVsmProtectionInformation,
	SystemInterruptCpuSetsInformation,
	SystemSecureBootPolicyFullInformation,
	SystemCodeIntegrityPolicyFullInformation,
	SystemAffinitizedInterruptProcessorInformation,
	SystemRootSiloInformation,
	SystemCpuSetInformation,
	SystemCpuSetTagInformation,
	SystemWin32WerStartCallout,
	SystemSecureKernelProfileInformation,
	SystemCodeIntegrityPlatformManifestInformation,
	SystemInterruptSteeringInformation,
	SystemSupportedProcessorArchitectures,
	SystemMemoryUsageInformation,
	SystemCodeIntegrityCertificateInformation,
	SystemPhysicalMemoryInformation,
	SystemControlFlowTransition,
	SystemKernelDebuggingAllowed,
	SystemActivityModerationExeState,
	SystemActivityModerationUserSettings,
	SystemCodeIntegrityPoliciesFullInformation,
	SystemCodeIntegrityUnlockInformation,
	SystemIntegrityQuotaInformation,
	SystemFlushInformation,
	SystemProcessorIdleMaskInformation,
	SystemSecureDumpEncryptionInformation,
	SystemWriteConstraintInformation,
	SystemKernelVaShadowInformation,
	SystemHypervisorSharedPageInformation,
	SystemFirmwareBootPerformanceInformation,
	SystemCodeIntegrityVerificationInformation,
	SystemFirmwarePartitionInformation,
	SystemSpeculationControlInformation,
	SystemDmaGuardPolicyInformation,
	SystemEnclaveLaunchControlInformation,
	SystemWorkloadAllowedCpuSetsInformation,
	SystemCodeIntegrityUnlockModeInformation,
	SystemLeapSecondInformation,
	SystemFlags2Information,
	SystemSecurityModelInformation,
	SystemCodeIntegritySyntheticCacheInformation,
	SystemFeatureConfigurationInformation,
	SystemFeatureConfigurationSectionInformation,
	SystemFeatureUsageSubscriptionInformation,
	SystemSecureSpeculationControlInformation,
	SystemSpacesBootInformation,
	SystemFwRamdiskInformation,
	SystemWheaIpmiHardwareInformation,
	SystemDifSetRuleClassInformation,
	SystemDifClearRuleClassInformation,
	SystemDifApplyPluginVerificationOnDriver,
	SystemDifRemovePluginVerificationOnDriver,
	SystemShadowStackInformation,
	SystemBuildVersionInformation,
	SystemPoolLimitInformation,
	SystemCodeIntegrityAddDynamicStore,
	SystemCodeIntegrityClearDynamicStores,
	SystemDifPoolTrackingInformation,
	SystemPoolZeroingInformation,
	SystemDpcWatchdogInformation,
	SystemDpcWatchdogInformation2,
	SystemSupportedProcessorArchitectures2,
	SystemSingleProcessorRelationshipInformation,
	SystemXfgCheckFailureInformation,
	SystemIommuStateInformation,
	SystemHypervisorMinrootInformation,
	SystemHypervisorBootPagesInformation,
	SystemPointerAuthInformation,
	SystemSecureKernelDebuggerInformation,
	SystemOriginalImageFeatureInformation,
	MaxSystemInfoClass
} SYSTEM_INFORMATION_CLASS;
typedef struct _LDR_DATA_TABLE_ENTRY
{
	LIST_ENTRY InLoadOrderLinks;
	LIST_ENTRY InMemoryOrderLinks;
	LIST_ENTRY InInitializationOrderLinks;
	PVOID DllBase;
	PVOID EntryPoint;
	ULONG SizeOfImage;
	UNICODE_STRING FullDllName;
	UNICODE_STRING BaseDllName;
	// ...
} LDR_DATA_TABLE_ENTRY, * PLDR_DATA_TABLE_ENTRY;

typedef struct _SYSTEM_THREAD_INFORMATION {
	LARGE_INTEGER KernelTime;
	LARGE_INTEGER UserTime;
	LARGE_INTEGER CreateTime;
	ULONG WaitTime;
	PVOID StartAddress;
	CLIENT_ID ClientId;
	KPRIORITY Priority;
	LONG BasePriority;
	ULONG ContextSwitches;
	ULONG ThreadState;
	KWAIT_REASON WaitReason;
} SYSTEM_THREAD_INFORMATION, * PSYSTEM_THREAD_INFORMATION;
typedef struct _SYSTEM_PROCESS_INFORMATION {
	ULONG NextEntryOffset;
	ULONG NumberOfThreads;
	LARGE_INTEGER SpareLi1;
	LARGE_INTEGER SpareLi2;
	LARGE_INTEGER SpareLi3;
	LARGE_INTEGER CreateTime;
	LARGE_INTEGER UserTime;
	LARGE_INTEGER KernelTime;
	UNICODE_STRING ImageName;
	KPRIORITY BasePriority;
	HANDLE UniqueProcessId;
	HANDLE InheritedFromUniqueProcessId;
	ULONG HandleCount;
	ULONG SessionId;
	ULONG_PTR PageDirectoryBase;
	SIZE_T PeakVirtualSize;
	SIZE_T VirtualSize;
	ULONG PageFaultCount;
	SIZE_T PeakWorkingSetSize;
	SIZE_T WorkingSetSize;
	SIZE_T QuotaPeakPagedPoolUsage;
	SIZE_T QuotaPagedPoolUsage;
	SIZE_T QuotaPeakNonPagedPoolUsage;
	SIZE_T QuotaNonPagedPoolUsage;
	SIZE_T PagefileUsage;
	SIZE_T PeakPagefileUsage;
	SIZE_T PrivatePageCount;
	LARGE_INTEGER ReadOperationCount;
	LARGE_INTEGER WriteOperationCount;
	LARGE_INTEGER OtherOperationCount;
	LARGE_INTEGER ReadTransferCount;
	LARGE_INTEGER WriteTransferCount;
	LARGE_INTEGER OtherTransferCount;
	SYSTEM_THREAD_INFORMATION Threads[1];
} SYSTEM_PROCESS_INFORMATION, * PSYSTEM_PROCESS_INFORMATION;


NTSYSCALLAPI
NTSTATUS
NTAPI
ZwQuerySystemInformation(
	_In_ SYSTEM_INFORMATION_CLASS SystemInformationClass,
	_Out_opt_ PVOID SystemInformation,
	_In_ ULONG SystemInformationLength,
	_Out_opt_ PULONG ReturnLength
);

NTKERNELAPI
NTSTATUS
NTAPI
PsLookupProcessThreadByCid(
	_In_ PCLIENT_ID Cid,
	_Outptr_opt_ PEPROCESS* Process,
	_Out_opt_ PETHREAD* Thread
);

NTKERNELAPI
NTSTATUS
NTAPI
PsAcquireProcessExitSynchronization(
	_In_ PEPROCESS Process
);

NTKERNELAPI
VOID
NTAPI
PsReleaseProcessExitSynchronization(
	_In_ PEPROCESS Process
);

NTKERNELAPI
ULONG
NTAPI
PsGetProcessSessionIdEx(
	_In_ PEPROCESS Process
);

NTKERNELAPI
PVOID
NTAPI
PsGetProcessWin32Process(
	_In_ PEPROCESS Process
);

NTKERNELAPI
PVOID
NTAPI
PsGetThreadWin32Thread(
	_In_ PETHREAD Thread
);
struct tagIAM_THREAD
{
	struct _LIST_ENTRY link;
	struct tagTHREADINFO* pti;
	struct tagDESKTOP* pdesk;
} IAM_THREAD;

unsigned __int64 _strtoui64(
	const char* nptr,
	char** endptr,
	int base
);