namespace LoneDMATest.Tests.Results
{
    public readonly struct LatencyTestResults
    {
        private readonly long _count;
        private readonly long _failed;
        private readonly TimeSpan _testDuration;
        private readonly TimeSpan _min;
        private readonly TimeSpan _max;

        private readonly double AvgReadSeconds => _testDuration.TotalSeconds / (double)Success;
        public readonly long Success => _count - _failed;
        public readonly int TotalLatency => (int)Math.Round(1f / AvgReadSeconds);
        public readonly int FastestRead => (int)Math.Round(_min.TotalMicroseconds);
        public readonly int SlowestRead => (int)Math.Round(_max.TotalMicroseconds);
        private readonly int AvgRead => (int)Math.Round(_testDuration.TotalMicroseconds / (double)Success);
        public readonly double PercentFailed => ((double)_failed / (double)_count) * 100f;
        public readonly TestResult Result
        {
            get
            {
                TestResult result;
                if (PercentFailed >= 1f)
                    result = TestResult.FAIL;
                else if (TotalLatency >= 18000)
                    result = TestResult.PERFECT;
                else if (TotalLatency >= 6000)
                    result = TestResult.EXCELLENT;
                else if (TotalLatency >= 4500)
                    result = TestResult.GOOD;
                else if (TotalLatency >= 3000)
                    result = TestResult.ACCEPTABLE;
                else
                    result = TestResult.FAIL;
                return result;
            }
        }

        public LatencyTestResults(long count, long failed, TimeSpan testDuration, TimeSpan min, TimeSpan max)
        {
            _count = count;
            _failed = failed;
            _testDuration = testDuration;
            _min = min;
            _max = max;
        }

        public readonly void Print()
        {
            Console.WriteLine();
            ConsoleWriteLine($"== Latency Test Results (4kB Reads) ==\n" +
                $"Total Read Latency: {TotalLatency.ToString("n0")}/sec\n" +
                $"Total Reads: {_count.ToString("n0")}\n" +
                $"Failed Reads: {_failed.ToString("n0")} ({PercentFailed.ToString("n2")}%)\n" +
                $"Fastest Read: {FastestRead.ToString("n0")} μs\n" +
                $"Slowest Read: {SlowestRead.ToString("n0")} μs\n" +
                $"Average Read: {AvgRead.ToString("n0")} μs\n", ConsoleColor.Cyan);
            Result.Print();
        }
    }
}
