using Spectre.Console;

namespace LoneDMATest.Tests.Results
{
    public sealed class LatencyTestResults : IResult
    {
        private readonly long _count;
        private readonly long _failed;
        private readonly TimeSpan _testDuration;
        private readonly TimeSpan _min;
        private readonly TimeSpan _max;

        private long Success => _count - _failed;

        private double AvgReadSeconds =>
            Success == 0 ? 0.0 : _testDuration.TotalSeconds / Success;

        private int TotalLatency =>
            AvgReadSeconds <= 0 ? 0 : (int)Math.Round(1.0 / AvgReadSeconds);

        private int FastestRead =>
            (int)Math.Round(_min.TotalMicroseconds);

        private int SlowestRead =>
            (int)Math.Round(_max.TotalMicroseconds);

        private int AvgRead =>
            Success == 0 ? 0 : (int)Math.Round(_testDuration.TotalMicroseconds / Success);

        private double PercentFailed =>
            _count == 0 ? 0.0 : (_failed / (double)_count) * 100.0;

        public TestResult Result
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
                else if (TotalLatency >= 4000)
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

        public void Print()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[cyan]== Latency Test Results (4kB Reads) ==[/]\n" +
                $"[cyan]Total Read Latency: {TotalLatency.ToString("n0")}/sec[/]\n" +
                $"[cyan]Total Reads: {_count.ToString("n0")}[/]\n" +
                $"[cyan]Failed Reads: {_failed.ToString("n0")} ({PercentFailed.ToString("n2")}%)\n[/]" +
                $"[cyan]Fastest Read: {FastestRead.ToString("n0")} μs[/]\n" +
                $"[cyan]Slowest Read: {SlowestRead.ToString("n0")} μs[/]\n" +
                $"[cyan]Average Read: {AvgRead.ToString("n0")} μs\n[/]");
            Result.Print();
        }
    }
}
