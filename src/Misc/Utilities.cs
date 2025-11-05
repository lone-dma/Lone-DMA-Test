using Spectre.Console;

namespace LoneDMATest.Misc
{
    internal static class Utilities
    {
        public static void ConsolePause()
        {
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(intercept: true);
        }
    }
}
