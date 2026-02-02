using LoneDMATest.DMA;
using LoneDMATest.Misc;
using LoneDMATest.Tests;
using Spectre.Console;
using System.Reflection;
using System.Runtime;
using System.Text;
using Velopack;
using Velopack.Sources;

namespace LoneDMATest
{
    class Program
    {
        private const string _mutexID = "b9812694-f82a-4dee-a9eb-7daccbee7d02";
        internal const string Name = "Lone's DMA Test Tool";
        private static Mutex _mutex;

        static void Main()
        {
            VelopackApp.Build().Run();
            try
            {
                Console.OutputEncoding = Encoding.Unicode;
                _mutex = new Mutex(true, _mutexID, out bool singleton);
                if (!singleton)
                    throw new InvalidOperationException("This Application is already running!");
                _ = Task.Run(CheckForUpdatesAsync); // Run continuations on the threadpool
                PerformanceInterop.SetHighPerformanceMode();
                RunMenuLoop();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[UNHANDLED EXCEPTION] {ex}")}[/]");
                Utilities.ConsolePause();
                throw;
            }
        }

        private static void RunMenuLoop()
        {
            string version = Assembly
                .GetExecutingAssembly()!
                .GetCustomAttribute<AssemblyFileVersionAttribute>()!
                .Version!;

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
                            "Stress Test",
                            "Options",
                            "Exit"
                        }));

                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
                try
                {
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
                        case "Stress Test":
                            StressTest.Instance.RunStandalone();
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
                    Utilities.ConsolePause();
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
            AnsiConsole.MarkupLine($"[gray]https://lone-dma.org/[/]");
            AnsiConsole.Write(new Rule().RuleStyle("grey").Centered());
            AnsiConsole.WriteLine();
        }

        private static async Task CheckForUpdatesAsync()
        {
            try
            {

                var updater = new UpdateManager(
                    source: new GithubSource(
                        repoUrl: "https://github.com/lone-dma/Lone-DMA-Test",
                        accessToken: null,
                        prerelease: false));

                if (!updater.IsInstalled)
                    return;

                var newVersion = await updater.CheckForUpdatesAsync();

                if (newVersion is not null)
                {
                    var prompt = Win32.MessageBox(
                        hWnd: IntPtr.Zero,
                        lpText: $"A new version ({newVersion.TargetFullRelease.Version}) is available.\n\nWould you like to update now?",
                        lpCaption: Program.Name,
                        uType: Win32.MessageBoxFlags.MB_YESNO | Win32.MessageBoxFlags.MB_ICONQUESTION | Win32.MessageBoxFlags.MB_DEFAULT_DESKTOP_ONLY);

                    if (prompt == Win32.MessageBoxResult.IDYES)
                    {
                        await updater.DownloadUpdatesAsync(newVersion);
                        updater.ApplyUpdatesAndRestart(newVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Win32.MessageBox(
                    IntPtr.Zero,
                    $"An unhandled exception occurred while checking for updates: {ex}",
                    Program.Name,
                    uType: Win32.MessageBoxFlags.MB_OK | Win32.MessageBoxFlags.MB_ICONERROR | Win32.MessageBoxFlags.MB_DEFAULT_DESKTOP_ONLY);
            }
        }
    }
}