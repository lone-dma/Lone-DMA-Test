namespace LoneDMATest.DMA
{
    /// <summary>
    /// Represents a page in a Physical Memory Map section.
    /// </summary>
    public readonly struct PMemPageEntry
    {
        public readonly ulong PageBase { get; init; }
        public readonly ulong RemainingBytesInSection { get; init; }
    }
}
