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
            ConsoleWrite("[i] Test Result: ", ConsoleColor.Cyan);
            string result = testResult.ToString();
            switch (testResult)
            {
                case TestResult.FAIL:
                    ConsoleWrite(result, ConsoleColor.Black, ConsoleColor.Red);
                    break;
                case TestResult.ACCEPTABLE:
                    ConsoleWrite(result, ConsoleColor.Black, ConsoleColor.DarkYellow);
                    break;
                case TestResult.GOOD:
                    ConsoleWrite(result, ConsoleColor.Black, ConsoleColor.DarkGreen);
                    break;
                case TestResult.EXCELLENT:
                    ConsoleWrite(result, ConsoleColor.Black, ConsoleColor.Green);
                    break;
                case TestResult.PERFECT:
                    ConsoleWrite(result, ConsoleColor.Black, ConsoleColor.DarkCyan);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(testResult));
            }
            Console.WriteLine();
        }
    }
}
