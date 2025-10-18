using Spectre.Console;

namespace LoneDMATest.Tests.Results
{
    public enum TestResult
    {
        /// <summary>
        /// This test has FAILED and steps should be taken to remediate.
        /// </summary>
        FAIL,
        /// <summary>
        /// This test has PASSED, and the results are satisfactory.
        /// </summary>
        ACCEPTABLE,
        /// <summary>
        /// This test has PASSED, and results are above satisfactory.
        /// </summary>
        GOOD,
        /// <summary>
        /// This test has PASSED, and results exceed expectations.
        /// </summary>
        EXCELLENT,
        /// <summary>
        /// This test has PASSED, and results are perfect.
        /// </summary>
        PERFECT
    }

    public static class TestResultExtensions
    {
        /// <summary>
        /// Print the test result.
        /// </summary>
        /// <param name="testResult">Test result.</param>
        public static void Print(this TestResult testResult)
        {
            AnsiConsole.Markup("[cyan][[i]] Test Result: [/]");
            string result = testResult.ToString();
            switch (testResult)
            {
                case TestResult.FAIL:
                    AnsiConsole.MarkupLine("[black on red]FAIL[/]");
                    break;
                case TestResult.ACCEPTABLE:
                    AnsiConsole.MarkupLine("[black on yellow]ACCEPTABLE[/]");
                    break;
                case TestResult.GOOD:
                    AnsiConsole.MarkupLine("[black on green]GOOD[/]");
                    break;
                case TestResult.EXCELLENT:
                    AnsiConsole.MarkupLine("[black on green]EXCELLENT[/]");
                    break;
                case TestResult.PERFECT:
                    AnsiConsole.MarkupLine("[black on aqua]PERFECT[/]");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(testResult));
            }
        }
    }
}
