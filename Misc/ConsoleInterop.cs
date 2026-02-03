namespace LoneDMATest.Misc
{
    internal static class ConsoleInterop
    {
        /// <summary>
        /// Write to the console followed by a line break.
        /// </summary>
        internal static void ConsoleWriteLine(string text,
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Write to the console.
        /// </summary>
        internal static void ConsoleWrite(string text,
            ConsoleColor foregroundColor = ConsoleColor.Gray,
            ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Pauses the console and waits for user to press a key.
        /// </summary>
        internal static void ConsolePause()
        {
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey(true);
        }
    }
}
