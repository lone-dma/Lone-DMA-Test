using Spectre.Console;

namespace LoneDMATest.Tests.Results
{
    public sealed class StressTestResult : IResult
    {
        private readonly long _count;
        private readonly long _failed;
        private readonly TimeSpan _duration;

        private float PercentFailed =>
            _count == 0 ? 0f : (_failed / (float)_count) * 100f;

        public TestResult Result
        {
            get
            {
                if (_failed > 0)
                    return TestResult.FAIL;
                return TestResult.PERFECT;
            }
        }

        public StressTestResult(long count, long failed, TimeSpan duration)
        {
            _count = count;
            _failed = failed;
            _duration = duration;
        }

        public void Print()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[cyan]== Stress Test Results (16MB Reads * 8 Threads) ==[/]\n" +
                $"[cyan]Duration: {(int)_duration.TotalSeconds} seconds[/]\n" +
                $"[cyan]Total Reads: {_count.ToString("n0")}[/]\n" +
                $"[cyan]Failed Reads: {_failed.ToString("n0")} ({PercentFailed.ToString("n2")}%)\n[/]");
            Result.Print();
        }
    }
}
