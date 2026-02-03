using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LoneDMATest.Tests
{
    internal static class LatencyTest
    {
        public const uint BytesPerRead = 0x1000;

        /// <summary>
        /// Run Latency Test in a standalone manner.
        /// </summary>
        public static void RunStandalone()
        {
            try
            {
                Console.Clear();
                var ts = TimeSpan.FromSeconds(30);
                using var dma = new DmaConnection();
                dma.GetMemoryMap();
                Run(dma, ts).Print();
            }
            catch (Exception ex)
            {
                ConsoleWriteLine($"[FAIL] {ex.Message}", ConsoleColor.Black, ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Run a Latency test.
        /// </summary>
        /// <param name="dma">DMA Connection instance.</param>
        /// <param name="nSeconds">Number of seconds to run test.</param>
        /// <returns>Test results.</returns>
        public static LatencyTestResults Run(DmaConnection dma, TimeSpan testDuration)
        {
            ConsoleWriteLine($"[i] Running Latency Test for {testDuration.TotalSeconds.ToString("n0")} seconds...", ConsoleColor.Cyan);
            var pages = dma.GetPhysMemPages();
            var pb = new byte[BytesPerRead];
            var h = GCHandle.Alloc(pb, GCHandleType.Pinned);
            try
            {
                long totalCount = 0;
                long failedCount = 0;
                TimeSpan minReadSpeed = TimeSpan.MaxValue;
                TimeSpan maxReadSpeed = TimeSpan.MinValue;
                var readSW = new Stopwatch();
                var testSW = Stopwatch.StartNew();
                while (testSW.Elapsed < testDuration)
                {
                    readSW.Restart();
                    if (dma.Vmm.LeechCore.ReadSpan(pages[Random.Shared.Next(pages.Length)].PageBase, pb.AsSpan()))
                    {
                        var speed = readSW.Elapsed;
                        if (speed < minReadSpeed)
                            minReadSpeed = speed;
                        if (speed > maxReadSpeed)
                            maxReadSpeed = speed;
                    }
                    else
                    {
                        failedCount++;
                    }
                    totalCount++;
                }
                ConsoleWriteLine("[OK] Latency Test", ConsoleColor.Black, ConsoleColor.Green);
                return new(totalCount, failedCount, testDuration, minReadSpeed, maxReadSpeed);
            }
            finally
            {
                h.Free();
            }
        }
    }
}
