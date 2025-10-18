using Spectre.Console;
using System.Diagnostics;
using System.Text;
using VmmSharpEx;
using VmmSharpEx.Options;

namespace LoneDMATest.DMA
{
    public sealed class DmaConnection : IDisposable
    {
        private static readonly string _vmmVersion;
        private static readonly string _leechcoreVersion;
        private readonly Vmm _vmm;
        private PMemPageEntry[] _paPages;

        /// <summary>
        /// Vmm Handle for this connection instance.
        /// </summary>
        public Vmm Vmm => _vmm;

        static DmaConnection()
        {
            _vmmVersion = FileVersionInfo.GetVersionInfo("vmm.dll").FileVersion;
            _leechcoreVersion = FileVersionInfo.GetVersionInfo("leechcore.dll").FileVersion;
        }

        public DmaConnection()
        {
            var algo = Options.FpgaAlgo;
            string[] args = new string[] {
                "-device",
                algo is FpgaAlgo.Auto ?
                    "fpga" : $"fpga://algo={(int)algo}",
                "-norefresh",
                "-waitinitialize"};
            var loggingLevel = Options.LoggingLevel;
            if (loggingLevel is not FpgaLoggingLevel.None)
            {
                string verbosity = loggingLevel is FpgaLoggingLevel.Verbose ?
                    "-v" : loggingLevel is FpgaLoggingLevel.VeryVerbose ?
                    "-vv" : loggingLevel is FpgaLoggingLevel.VeryVeryVerbose ?
                    "-vvv" : throw new ArgumentOutOfRangeException(nameof(loggingLevel));
                string[] loggingArgs = { "-printf", verbosity };
                args = args.Concat(loggingArgs).ToArray();
            }
            bool mmap = File.Exists("mmap.txt");
            if (mmap)
            {
                string[] mapArgs = { "-memmap", "mmap.txt" };
                args = args.Concat(mapArgs).ToArray();
            }
            AnsiConsole.MarkupLine($"[cyan][[i]] Vmm Version: {Markup.Escape(_vmmVersion)}[/]\n[cyan][[i]] Leechcore Version: {Markup.Escape(_leechcoreVersion)}[/]");
            _vmm = new Vmm(args)
            {
                EnableMemoryWriting = false
            };
            if (mmap)
            {
                _vmm.Log("WARNING: Memory Map Loaded", Vmm.LogLevel.Warning);
            }
            if (_vmm.LeechCore.GetOption(LcOption.FPGA_ALGO_TINY) is ulong tiny && tiny != 0)
            {
                _vmm.Log("WARNING: TINY PCIe TLP algo auto-selected!", Vmm.LogLevel.Warning);
            }
            AnsiConsole.MarkupLine("[black on green][[OK]] DMA Initialization[/]");
        }

        /// <summary>
        /// Set the memory map for this connection.
        /// </summary>
        public void GetMemoryMap()
        {
            AnsiConsole.MarkupLine("[cyan][[i]] Retrieving Physical Memory Map...[/]");
            var map = _vmm.Map_GetPhysMem();
            if (map.Length == 0)
                throw new InvalidOperationException("Failed to retrieve Physical Memory Map!");
            // Set the physical memory pages.
            var paList = new List<PMemPageEntry>();
            foreach (var pMapEntry in map)
            {
                for (ulong p = pMapEntry.pa, cbToEnd = pMapEntry.cb;
                    cbToEnd > 0x1000;
                    p += 0x1000, cbToEnd -= 0x1000)
                {
                    paList.Add(new()
                    {
                        PageBase = p,
                        RemainingBytesInSection = cbToEnd
                    });
                }
            }
            var pages = paList.ToArray();
            Random.Shared.Shuffle(pages);
            _paPages = pages;
            // Display map to user.
            var sb = new StringBuilder();
            int leftLength = map.Max(x => x.pa).ToString("x").Length;
            for (int i = 0; i < map.Length; i++)
            {
                sb.AppendFormat($"{{0,{-leftLength}}}", map[i].pa.ToString("x"))
                    .Append($" - {(map[i].pa + map[i].cb - 1).ToString("x")}")
                    .AppendLine();
            }
            AnsiConsole.MarkupLine($"[green]{Markup.Escape(sb.ToString())}[/]");
            AnsiConsole.MarkupLine("[black on green][[OK]] Memory Map[/]");
        }

        /// <summary>
        /// Get the physical memory pages.
        /// </summary>
        /// <param name="pageCount">Number of pages to parse.</param>
        /// <returns>Array of valid physical page addresses.</returns>
        public ReadOnlySpan<PMemPageEntry> GetPhysMemPages(int pageCount = 100000, uint minimumContiguousMemoryLength = 0x1000)
        {
            ArgumentNullException.ThrowIfNull(_paPages, "No Physical Memory Pages to parse from. Please call GetMemoryMap() first.");
            return _paPages
                .Where(page => page.RemainingBytesInSection >=  minimumContiguousMemoryLength)
                .Take(pageCount)
                .ToArray();
        }

        #region IDisposable
        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                return;
            _vmm.Dispose();
            _disposed = true;
        }
        #endregion
    }
}
