using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LoneDMATest.Misc
{
    internal static partial class PerformanceInterop
    {
        private delegate bool ConsoleEventDelegate(int eventType);
        private static readonly ConsoleEventDelegate _consoleHandler = new(ConsoleEventCallback);
        private static Guid _oldPowerPlan;

        [LibraryImport("kernel32.dll")]
        private static partial EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [LibraryImport("powrprof.dll")]
        private static partial uint PowerSetActiveScheme(IntPtr userRootPowerKey, ref Guid schemeGuid);

        [LibraryImport("powrprof.dll")]
        private static partial uint PowerGetActiveScheme(IntPtr userRootPowerKey, out IntPtr pActivePolicyGuid);

        [LibraryImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, [MarshalAs(UnmanagedType.Bool)] bool add);

        /// <summary>
        /// Sets High Performance mode in Windows Power Plans and Process Priority.
        /// </summary>
        public static void SetHighPerformanceMode()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
            if (PowerGetActiveScheme(IntPtr.Zero, out IntPtr pActivePolicyGuid) == 0)
            {
                _oldPowerPlan = Marshal.PtrToStructure<Guid>(pActivePolicyGuid);
                var highPerformanceGuid = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
                if (PowerSetActiveScheme(IntPtr.Zero, ref highPerformanceGuid) == 0)
                {
                    AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                    SetConsoleCtrlHandler(_consoleHandler, true);
                }
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) =>
            ResetPowerPlan();

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                ResetPowerPlan();
                return false;
            }
            return true;
        }

        private static void ResetPowerPlan()
        {
            PowerSetActiveScheme(IntPtr.Zero, ref _oldPowerPlan);
        }

        [Flags]
        private enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
    }
}
