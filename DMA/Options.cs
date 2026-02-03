using LoneDMATest.Tests;

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
            Console.Clear();
            ConsoleWriteLine("[?] Select an FPGA Algorithm:\n" +
                "1. Auto (default)\n" +
                "2. Async Normal\n" +
                "3. Async Tiny\n" +
                "4. Old Normal\n" +
                "5. Old Tiny\n", ConsoleColor.Cyan);
            var fpga = Console.ReadKey(true).Key;
            ParseFpgaAlgo(fpga);
            ConsoleWriteLine("[?] Select Logging Level:\n" +
                "1. None\n" +
                "2. Verbose (default)\n" +
                "3. Very Verbose\n" +
                "4. Very Very Verbose (Not Recommended)\n", ConsoleColor.Cyan);
            var logging = Console.ReadKey(true).Key;
            ParseLogging(logging);
        }

        private static void ParseFpgaAlgo(ConsoleKey key)
        {
            FpgaAlgo algo;
            if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
            {
                algo = FpgaAlgo.Auto;
            }
            else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
            {
                algo = FpgaAlgo.AsyncNormal;
            }
            else if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3)
            {
                algo = FpgaAlgo.AsyncTiny;
            }
            else if (key == ConsoleKey.D4 || key == ConsoleKey.NumPad4)
            {
                algo = FpgaAlgo.OldNormal;
            }
            else if (key == ConsoleKey.D5 || key == ConsoleKey.NumPad5)
            {
                algo = FpgaAlgo.OldTiny;
            }
            else
            {
                ConsoleWriteLine("Invalid FPGA Algo selection!\n", ConsoleColor.Black, ConsoleColor.Red);
                return;
            }
            FpgaAlgo = algo;
            ConsoleWriteLine($"FPGA Algo Set to {algo}\n", ConsoleColor.Black, ConsoleColor.Green);
        }

        private static void ParseLogging(ConsoleKey key)
        {
            FpgaLoggingLevel logLevel;
            if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
            {
                logLevel = FpgaLoggingLevel.None;
            }
            else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
            {
                logLevel = FpgaLoggingLevel.Verbose;
            }
            else if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3)
            {
                logLevel = FpgaLoggingLevel.VeryVerbose;
            }
            else if (key == ConsoleKey.D4 || key == ConsoleKey.NumPad4)
            {
                logLevel = FpgaLoggingLevel.VeryVeryVerbose;
            }
            else
            {
                ConsoleWriteLine("Invalid Logging Level selection!\n", ConsoleColor.Black, ConsoleColor.Red);
                return;
            }
            LoggingLevel = logLevel;
            ConsoleWriteLine($"Logging Level Set to {logLevel}\n", ConsoleColor.Black, ConsoleColor.Green);
        }
    }
}
