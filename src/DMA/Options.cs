using Spectre.Console;

namespace LoneDMATest.DMA
{
    internal static class Options
    {
        public static string DeviceStr = "fpga";
        public static FpgaLoggingLevel LoggingLevel = FpgaLoggingLevel.Verbose;

        /// <summary>
        /// Change the global options for this testing session.
        /// </summary>
        public static void ChangeOptions()
        {
            AnsiConsole.Clear();

            var deviceStr = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan][[?]] Select Device Connection String[/]")
                    .PageSize(10)
                    .AddChoices(new[]
                    {
                        "default (fpga)",
                        "custom"
                    }));
            switch (deviceStr)
            {
                case "default (fpga)":
                    deviceStr = "fpga";
                    break;
                case "custom":
                    deviceStr = AnsiConsole.Ask<string>("[cyan][[?]] Enter custom Device Connection string:[/]");
                    break;
            }
            DeviceStr = deviceStr;
            AnsiConsole.MarkupLine($"[black on green]Device Connection String set to {Markup.Escape(deviceStr.ToString())}[/]\n");

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
