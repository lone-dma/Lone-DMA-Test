namespace LoneDMATest.Tests.Results
{
    /// <summary>
    /// Defines the interface for test results.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// The result(s) of the test.
        /// </summary>
        TestResult Result { get; }
        /// <summary>
        /// Prints the test result(s).
        /// </summary>
        void Print();
    }
}
