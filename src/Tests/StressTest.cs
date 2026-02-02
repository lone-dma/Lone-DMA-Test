using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using Spectre.Console;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LoneDMATest.Tests
{
    public sealed class StressTest : ITest
    {
        /// <summary>
        /// Singleton Instance of <see cref="StressTest"/>.
        /// </summary>
        internal static ITest Instance { get; } = new StressTest();
        public const uint BytesPerRead = 0x1000000;

        private StressTest() { }

        /// <summary>
        /// Run Stress Test in a standalone manner.
        /// </summary>
        public void RunStandalone()
        {
            try
            {
                AnsiConsole.Clear();
                using var dma = new DmaConnection();
                dma.GetMemoryMap();
                Run(dma, TimeSpan.Zero).Print();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[FAIL] {ex.Message}")}[/]");
            }
        }

        /// <summary>
        /// Run a stress test.
        /// </summary>
        /// <param name="dma">DMA Connection instance.</param>
        /// <returns>Test results.</returns>
        public unsafe IResult Run(DmaConnection dma, TimeSpan testDuration)
        {
            AnsiConsole.MarkupLine("[cyan][[i]] Running Stress Test on 8 Threads...[/]");
            long totalCount = 0;
            long failedCount = 0;
            var pages = dma.GetPhysMemPages(
                pageCount: 1500,
                minimumContiguousMemoryLength: BytesPerRead).ToArray();
            using var cts = new CancellationTokenSource();
            var ct = cts.Token;
            for (int i = 0; i < 8; i++)
            {
                new Thread(() =>
                {
                    var pb = NativeMemory.AlignedAlloc(
                        byteCount: BytesPerRead,
                        alignment: 0x1000);
                    try
                    {
                        while (!ct.IsCancellationRequested)
                        {
                            if (!dma.Vmm.LeechCore.Read(
                                pa: pages[Random.Shared.Next(pages.Length)].PageBase,
                                pb: pb,
                                cb: BytesPerRead))
                            {
                                Interlocked.Increment(ref failedCount);
                            }
                            Interlocked.Increment(ref totalCount);
                        }
                    }
                    finally
                    {
                        NativeMemory.AlignedFree(pb);
                    }
                })
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Highest
                }.Start();
            }
            // Wait for user to press q
            AnsiConsole.MarkupLine("[yellow]Press 'Q' to stop the stress test...[/]");
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    if (key.Key == ConsoleKey.Q)
                    {
                        break;
                    }
                }
                Thread.Sleep(50);
            }
            cts.Cancel();
            return new StressTestResult(totalCount, failedCount);
        }
    }
}
