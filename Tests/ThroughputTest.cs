using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LoneDMATest.Tests
{
    internal static class ThroughputTest
    {
        public const uint BytesPerRead = 0x1000000;
        /// <summary>
        /// Run Throughput Test in a standalone manner.
        /// </summary>
        public static void RunStandalone()
        {
            try
            {
                Console.Clear();
                using var dma = new DmaConnection();
                dma.GetMemoryMap();
                Run(dma, TimeSpan.FromSeconds(15)).Print();
            }
            catch (Exception ex)
            {
                ConsoleWriteLine($"[FAIL] {ex.Message}", ConsoleColor.Black, ConsoleColor.Red);
            }
        }

        /// <summary>
        /// Run a throughput test.
        /// </summary>
        /// <param name="dma">DMA Connection instance.</param>
        /// <param name="nPages">Number of pages to read.</param>
        /// <returns>Test results.</returns>
        public static ThroughputTestResults Run(DmaConnection dma, TimeSpan testDuration)
        {
            ConsoleWriteLine($"[i] Running Throughput Test for {testDuration.TotalSeconds.ToString("n0")} seconds...", ConsoleColor.Cyan);
            var pages = dma.GetPhysMemPages(
                pageCount: 1000, 
                minimumContiguousMemoryLength: BytesPerRead);
            var pb = new byte[BytesPerRead];
            var h = GCHandle.Alloc(pb, GCHandleType.Pinned);
            try
            {
                long totalCount = 0;
                long failedCount = 0;
                var testSW = Stopwatch.StartNew();
                while (testSW.Elapsed < testDuration)
                {
                    if (!dma.Vmm.LeechCore.ReadSpan(pages[Random.Shared.Next(pages.Length)].PageBase, pb.AsSpan()))
                        failedCount++;
                    totalCount++;
                }
                ConsoleWriteLine("[OK] Throughput Test", ConsoleColor.Black, ConsoleColor.Green);
                return new(totalCount, failedCount, testDuration);
            }
            finally
            {
                h.Free();
            }
        }
    }
}
