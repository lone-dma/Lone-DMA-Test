using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;
using Spectre.Console;

namespace LoneDMATest.Tests
{
    public sealed class FullTest : ITest
    {
        /// <summary>
        /// Singleton Instance of <see cref="FullTest"/>.
        /// </summary>
        internal static ITest Instance { get; } = new FullTest();

        private FullTest() { }

        public void RunStandalone()
        {
            try
            {
                AnsiConsole.Clear();
                using var dma = new DmaConnection();
                ProcessTest(dma);
                dma.GetMemoryMap();
                var latency = LatencyTest.Instance.Run(dma, TimeSpan.FromSeconds(5));
                var tput = ThroughputTest.Instance.Run(dma, TimeSpan.FromSeconds(5));
                latency.Print();
                tput.Print();
                bool failed = latency.Result is TestResult.FAIL || tput.Result is TestResult.FAIL;
                AnsiConsole.WriteLine();
                AnsiConsole.Markup("[cyan][[i]] Overall Test Result: [/]");
                AnsiConsole.MarkupLine(failed ? "[black on red]FAIL[/]" : "[black on green]PASS[/]");
                AnsiConsole.WriteLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[FAIL] {ex.Message}")}[/]");
            }
        }

        private static void ProcessTest(DmaConnection dma)
        {
            const string processTarget = "smss.exe";
            AnsiConsole.MarkupLine($"[cyan][[i]] Attempting to locate process {Markup.Escape(processTarget)}...[/]");
            if (!dma.Vmm.PidGetFromName(processTarget, out uint pid))
                throw new InvalidOperationException($"Unable to locate process {processTarget}!");
            AnsiConsole.MarkupLine($"[green][[i]] Process {Markup.Escape(processTarget)} running @ PID {pid}[/]");
            AnsiConsole.MarkupLine("[cyan][[i]] Attempting to enumerate process modules...[/]");
            var modules = dma.Vmm.Map_GetModule(pid, false);
            if (modules.Length == 0)
                throw new InvalidOperationException($"No Modules located for {processTarget}!");
            foreach (var module in modules)
                AnsiConsole.MarkupLine($"[green][[i]] {Markup.Escape(module.sText)} @ 0x{module.vaBase.ToString("x")}[/]");
            AnsiConsole.MarkupLine("[black on green][[OK]] Process Lookup[/]");
        }

        public IResult Run(DmaConnection dma, TimeSpan testDuration)
        {
            throw new NotImplementedException();
        }
    }
}
