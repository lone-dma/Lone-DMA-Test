using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using VmmSharpEx;

namespace LoneDMATest.Tests
{
    internal static class FullTest
    {
        public static void RunStandalone()
        {
            try
            {
                Console.Clear();
                using var dma = new DmaConnection();
                ProcessTest(dma);
                dma.GetMemoryMap();
                var latency = LatencyTest.Run(dma, TimeSpan.FromSeconds(5));
                var tput = ThroughputTest.Run(dma, TimeSpan.FromSeconds(5));
                latency.Print();
                tput.Print();
                bool failed = latency.Result is TestResult.FAIL || tput.Result is TestResult.FAIL;
                Console.WriteLine();
                ConsoleWrite("[i] Overall Test Result: ", ConsoleColor.Cyan);
                ConsoleWrite(failed ? "FAIL" : "PASS", ConsoleColor.Black, failed ? ConsoleColor.Red : ConsoleColor.Green);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                ConsoleWriteLine($"[FAIL] {ex.Message}", ConsoleColor.Black, ConsoleColor.Red);
            }
        }

        private static void ProcessTest(DmaConnection dma)
        {
            const string processTarget = "smss.exe";
            ConsoleWriteLine($"[i] Attempting to locate process {processTarget}...", ConsoleColor.Cyan);
            if (!dma.Vmm.PidGetFromName(processTarget, out uint pid))
                throw new InvalidOperationException($"Unable to locate process {processTarget}!");
            ConsoleWriteLine($"[i] Process {processTarget} running @ PID {pid}", ConsoleColor.Green);
            ConsoleWriteLine($"[i] Attempting to enumerate process modules...", ConsoleColor.Cyan);
            var modules = dma.Vmm.Map_GetModule(pid, false);
            if (modules.Length == 0)
                throw new InvalidOperationException($"No Modules located for {processTarget}!");
            foreach (var module in modules)
                ConsoleWriteLine($"[i] {module.sText} @ 0x{module.vaBase.ToString("x")}", ConsoleColor.Green);
            ConsoleWriteLine("[OK] Process Lookup", ConsoleColor.Black, ConsoleColor.Green);
        }
    }
}
