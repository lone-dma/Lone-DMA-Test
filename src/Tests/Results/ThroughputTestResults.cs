using Spectre.Console;

namespace LoneDMATest.Tests.Results
{
    public sealed class ThroughputTestResults : IResult
    {
        private readonly long _count;
        private readonly long _failed;
        private readonly TimeSpan _testDuration;

        private long Success => _count - _failed;
        private float PercentFailed =>
            _count == 0 ? 0f : (_failed / (float)_count) * 100f;

        public TestResult Result
        {
            get
            {
                TestResult result;
                if (PercentFailed > 0f)
                    result = TestResult.FAIL;
                else if (Throughput >= 600f)
                    result = TestResult.PERFECT;
                else if (Throughput >= 200f)
                    result = TestResult.EXCELLENT;
                else if (Throughput >= 150f)
                    result = TestResult.GOOD;
                else if (Throughput >= 100f)
                    result = TestResult.ACCEPTABLE;
                else
                    result = TestResult.FAIL;
                return result;
            }
        }

        /// <summary>
        /// Throughput in MB.
        /// </summary>
        public float Throughput
        {
            get
            {
                ulong bytesRead = (ulong)Success * ThroughputTest.BytesPerRead;

                double mbPerSec = bytesRead / 1024d / 1024d / _testDuration.TotalSeconds;
                return (float)mbPerSec;
            }
        }


        public ThroughputTestResults(long count, long failed, TimeSpan testDuration)
        {
            _count = count;
            _failed = failed;
            _testDuration = testDuration;
        }

        public void Print()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[cyan]== Throughput Test Results (16MB Reads) ==[/]\n" +
                $"[cyan]Total Read Throughput: {Throughput.ToString("n2")} MB/s[/]\n" +
                $"[cyan]Total Reads: {_count.ToString("n0")}[/]\n" +
                $"[cyan]Failed Reads: {_failed.ToString("n0")} ({PercentFailed.ToString("n2")}%)\n[/]");
            if (Throughput < 45f)
            {
                AnsiConsole.MarkupLine("[black on yellow][[WARNING]] Throughput indicates USB 2.0 Connection. Check port/cable/connection for issues.[/]");
            }
            Result.Print();
        }
    }
}
