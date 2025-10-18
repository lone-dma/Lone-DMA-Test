using LoneDMATest;
using LoneDMATest.DMA;
using LoneDMATest.Misc;
using LoneDMATest.Tests;
using Spectre.Console;
using System.Reflection;
using System.Runtime;
using System.Text;

[assembly: AssemblyVersion("3.0.*")]
[assembly: AssemblyTitle(Program.Name)]
[assembly: AssemblyProduct(Program.Name)]
[assembly: AssemblyCopyright("©2025 Lone")]
[assembly: System.Runtime.Versioning.SupportedOSPlatform("Windows")]

namespace LoneDMATest
{
    internal static class Program
    {
        private const string _mutexID = "b9812694-f82a-4dee-a9eb-7daccbee7d02";
        internal const string Name = "Lone's DMA Test Tool";
        private static readonly Mutex _mutex;

        static Program() => Init(ref _mutex);

        static void Main() => RunMenuLoop();

        private static void RunMenuLoop()
        {
            string version = Assembly.GetExecutingAssembly()!.GetName()!.Version!.ToString();

            while (true)
            {
                AnsiConsole.Clear();
                RenderHeader(version);

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[cyan][[?]] Make a selection[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(Color.Black, Color.Aqua, Decoration.Bold))
                        .AddChoices(new[]
                        {
                            "Full Test (Recommended)",
                            "Latency Test",
                            "Throughput Test",
                            "Options",
                            "Exit"
                        }));

                try
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                    GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
                    switch (choice)
                    {
                        case "Full Test (Recommended)":
                            FullTest.Instance.RunStandalone();
                            break;
                        case "Latency Test":
                            LatencyTest.Instance.RunStandalone();
                            break;
                        case "Throughput Test":
                            ThroughputTest.Instance.RunStandalone();
                            break;
                        case "Options":
                            Options.ChangeOptions();
                            break;
                        case "Exit":
                            Environment.Exit(0);
                            break;
                    }
                }
                finally
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Normal;
                    GCSettings.LatencyMode = GCLatencyMode.Interactive;
                    GC.Collect();
                    AnsiConsole.MarkupLine("[grey]Press any key to return to menu...[/]");
                    Console.ReadKey(true);
                }
            }
        }

        private static void RenderHeader(string version)
        {
            var title = new FigletText(Name)
                .Centered()
                .Color(Color.Aqua);
            AnsiConsole.Write(title);

            AnsiConsole.MarkupLine($"[gray]Version {version}[/]");
            AnsiConsole.Write(new Rule().RuleStyle("grey").Centered());
            AnsiConsole.WriteLine();
        }

        private static void Init(ref Mutex mutex)
        {
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
                mutex = new Mutex(true, _mutexID, out bool singleton);
                if (!singleton)
                    throw new InvalidOperationException("This Application is already running!");
                PerformanceInterop.SetHighPerformanceMode();
                VerifyDependencies();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[STARTUP FAIL] {ex.Message}")}[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                throw;
            }
        }

        /// <summary>
        /// Validates that all startup dependencies are present.
        /// </summary>
        private static void VerifyDependencies()
        {
            var dependencies = new List<string>()
            {
                "vmm.dll",
                "leechcore.dll",
                "FTD3XX.dll",
                "symsrv.dll",
                "dbghelp.dll",
                "vcruntime140.dll",
                "tinylz4.dll"
            };

            foreach (var dep in dependencies)
            {
                if (!File.Exists(dep))
                    throw new FileNotFoundException($"Missing Dependency '{dep}'");
            }
        }
    }
}