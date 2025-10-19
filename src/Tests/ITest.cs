using LoneDMATest.DMA;
using LoneDMATest.Tests.Results;

namespace LoneDMATest.Tests
{
    /// <summary>
    /// Defines a test interface.
    /// </summary>
    public interface ITest
    {
        /// <summary>
        /// Runs the test in standalone mode and immediately returns results to the end user.
        /// </summary>
        void RunStandalone();
        /// <summary>
        /// Runs the test in embedded mode and returns the results to the caller (another test,etc.)
        /// </summary>
        /// <param name="dma"></param>
        /// <param name="testDuration"></param>
        /// <returns></returns>
        IResult Run(DmaConnection dma, TimeSpan testDuration);
    }
}
