using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using Spectre.Console;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LoneDMATest.Tests
{
    public sealed class LatencyTest : ITest
    {
        /// <summary>
        /// Singleton Instance of <see cref="LatencyTest"/>.
        /// </summary>
        internal static ITest Instance { get; } = new LatencyTest();
        public const uint BytesPerRead = 0x1000;

        private LatencyTest() { }

        /// <summary>
        /// Run Latency Test in a standalone manner.
        /// </summary>
        public void RunStandalone()
        {
            try
            {
                AnsiConsole.Clear();
                var ts = TimeSpan.FromSeconds(30);
                using var dma = new DmaConnection();
                dma.GetMemoryMap();
                Run(dma, ts).Print();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[FAIL] {ex.Message}")}[/]");
            }
        }

        /// <summary>
        /// Run a Latency test.
        /// </summary>
        /// <param name="dma">DMA Connection instance.</param>
        /// <param name="nSeconds">Number of seconds to run test.</param>
        /// <returns>Test results.</returns>
        public IResult Run(DmaConnection dma, TimeSpan testDuration)
        {
            AnsiConsole.MarkupLine($"[cyan][[i]] Running Latency Test for {testDuration.TotalSeconds.ToString("n0")} seconds...[/]");
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
                AnsiConsole.MarkupLine("[black on green][[OK]] Latency Test[/]");
                return new LatencyTestResults(totalCount, failedCount, testDuration, minReadSpeed, maxReadSpeed);
            }
            finally
            {
                h.Free();
            }
        }
    }
}
