using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using Spectre.Console;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LoneDMATest.Tests
{
    public sealed class ThroughputTest : ITest
    {
        /// <summary>
        /// Singleton Instance of <see cref="ThroughputTest"/>.
        /// </summary>
        internal static ITest Instance { get; } = new ThroughputTest();
        public const uint BytesPerRead = 0x1000000;

        private ThroughputTest() { }

        /// <summary>
        /// Run Throughput Test in a standalone manner.
        /// </summary>
        public void RunStandalone()
        {
            try
            {
                AnsiConsole.Clear();
                using var dma = new DmaConnection();
                dma.GetMemoryMap();
                Run(dma, TimeSpan.FromSeconds(15)).Print();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[FAIL] {ex.Message}")}[/]");
            }
        }

        /// <summary>
        /// Run a throughput test.
        /// </summary>
        /// <param name="dma">DMA Connection instance.</param>
        /// <returns>Test results.</returns>
        public unsafe IResult Run(DmaConnection dma, TimeSpan testDuration)
        {
            AnsiConsole.MarkupLine($"[cyan][[i]] Running Throughput Test for {testDuration.TotalSeconds.ToString("n0")} seconds...[/]");
            var pages = dma.GetPhysMemPages(
                pageCount: 1000,
                minimumContiguousMemoryLength: BytesPerRead);
            var pb = NativeMemory.AlignedAlloc(
                byteCount: BytesPerRead,
                alignment: 0x1000);
            try
            {
                long totalCount = 0;
                long failedCount = 0;
                var testSW = Stopwatch.StartNew();
                while (testSW.Elapsed < testDuration)
                {
                    if (!dma.Vmm.LeechCore.Read(
                        pa: pages[Random.Shared.Next(pages.Length)].PageBase,
                        pb: pb,
                        cb: BytesPerRead))
                    {
                        failedCount++;
                    }
                    totalCount++;
                }
                AnsiConsole.MarkupLine("[black on green][[OK]] Throughput Test[/]");
                return new ThroughputTestResults(totalCount, failedCount, testDuration);
            }
            finally
            {
                NativeMemory.AlignedFree(pb);
            }
        }
    }
}
