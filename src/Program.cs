using LoneDMATest.DMA;
using LoneDMATest.Misc;
using LoneDMATest.Tests;
using Spectre.Console;
using System;
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
        private static readonly Mutex _mutex;

        static Program()
        {
            try
            {
                VelopackApp.Build().Run();
                Console.OutputEncoding = Encoding.Unicode;
                _mutex = new Mutex(true, _mutexID, out bool singleton);
                if (!singleton)
                    throw new InvalidOperationException("This Application is already running!");
                PerformanceInterop.SetHighPerformanceMode();
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[STARTUP FAIL] {ex}")}[/]");
                Utilities.ConsolePause();
                throw;
            }
        }

        static void Main()
        {
            CheckForUpdatesAsync().GetAwaiter().GetResult();
            RunMenuLoop();
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

                UpdateInfo newVersion = null;
                await AnsiConsole.Status()
                    .Spinner(Spinner.Known.Dots)
                    .SpinnerStyle(new Style(Color.Aqua))
                    .StartAsync("Checking for updates...", async _ =>
                    {
                        newVersion = await updater.CheckForUpdatesAsync();
                    });

                if (newVersion is not null)
                {
                    AnsiConsole.MarkupLine(
                        $"[green]A new version ([bold]{Markup.Escape(newVersion.TargetFullRelease.Version.ToString())}[/]) is available.[/]");

                    bool doUpdate = AnsiConsole.Confirm("[cyan]Would you like to update now?[/]", defaultValue: false);

                    if (doUpdate)
                    {
                        await AnsiConsole.Status()
                            .Spinner(Spinner.Known.Dots)
                            .SpinnerStyle(new Style(Color.Aqua))
                            .StartAsync("Downloading update...", async _ =>
                            {
                                await updater.DownloadUpdatesAsync(newVersion);
                            });

                        AnsiConsole.MarkupLine("[grey]Applying update and restarting...[/]");
                        updater.ApplyUpdatesAndRestart(newVersion);
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[black on red]{Markup.Escape($"[UPDATE ERROR] {ex}")}[/]");
                Utilities.ConsolePause();
            }
            finally
            {
                AnsiConsole.Clear();
            }
        }
    }
}