using Spectre.Console;

namespace LoneDMATest.DMA
{
    internal static class Options
    {
        public static FpgaAlgo FpgaAlgo = FpgaAlgo.Auto;
        public static FpgaLoggingLevel LoggingLevel = FpgaLoggingLevel.Verbose;

        /// <summary>
        /// Change the global options for this testing session.
        /// </summary>
        public static void ChangeOptions()
        {
            AnsiConsole.Clear();

            var algo = AnsiConsole.Prompt(
                new SelectionPrompt<FpgaAlgo>()
                    .Title("[cyan][[?]] Select an FPGA Algorithm[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        FpgaAlgo.Auto,
                        FpgaAlgo.AsyncNormal,
                        FpgaAlgo.AsyncTiny,
                        FpgaAlgo.OldNormal,
                        FpgaAlgo.OldTiny
                    }));
            FpgaAlgo = algo;
            AnsiConsole.MarkupLine($"[black on green]FPGA Algo Set to {Markup.Escape(algo.ToString())}[/]\n");

            var logLevel = AnsiConsole.Prompt(
                new SelectionPrompt<FpgaLoggingLevel>()
                    .Title("[cyan][[?]] Select Logging Level[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        FpgaLoggingLevel.Verbose,
                        FpgaLoggingLevel.VeryVerbose,
                        FpgaLoggingLevel.VeryVeryVerbose,
                        FpgaLoggingLevel.None
                    }));
            LoggingLevel = logLevel;
            AnsiConsole.MarkupLine($"[black on green]Logging Level Set to {Markup.Escape(logLevel.ToString())}[/]\n");
        }
    }
}
