global using static LoneDMATest.Misc.ConsoleInterop;
using LoneDMATest;
using LoneDMATest.DMA;
using LoneDMATest.Misc;
using LoneDMATest.Tests;
using System.Reflection;
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

        static void Main() => PromptUser();

        private static void PromptUser()
        {
            string version = Assembly.GetExecutingAssembly()!.GetName()!.Version!.ToString();
            while (true)
            {
                Console.Clear();
                ConsoleWriteLine($"=========================\n" +
                    $"{Program.Name}\n" +
                    $"Version {version}\n" +
                    "=========================", ConsoleColor.Cyan);
                Console.WriteLine();
                ConsoleWriteLine("[?] Make a selection:\n\n" +
                    "1. Full Test (Recommended)\n" +
                    "2. Latency Test\n" +
                    "3. Throughput Test\n" +
                    "4. Options\n" +
                    "5. Exit", ConsoleColor.Cyan);
                var key = Console.ReadKey(true).Key;
                ParseKeyInput(key);
            }
        }

        private static void ParseKeyInput(ConsoleKey key)
        {
            try
            {
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    FullTest.RunStandalone();
                }
                else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                {
                    LatencyTest.RunStandalone();
                }
                else if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3)
                {
                    ThroughputTest.RunStandalone();
                }
                else if (key == ConsoleKey.D4 || key == ConsoleKey.NumPad4)
                {
                    Options.ChangeOptions();
                }
                else if (key == ConsoleKey.D5 || key == ConsoleKey.NumPad5)
                {
                    Environment.Exit(0);
                }
                else
                    ConsoleWriteLine("Invalid selection!", ConsoleColor.Black, ConsoleColor.Red);
            }
            finally
            {
                ConsolePause();
            }
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
                ConsoleWriteLine($"[STARTUP FAIL] {ex.Message}", ConsoleColor.Black, ConsoleColor.Red);
                ConsolePause();
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