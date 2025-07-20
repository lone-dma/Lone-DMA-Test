namespace LoneDMATest.DMA
{
    /// <summary>
    /// Represents a page in a Physical Memory Map section.
    /// </summary>
    public readonly struct PMemPageEntry
    {
        public ulong PageBase { get; init; }
        public ulong RemainingBytesInSection { get; init; }
    }
}
