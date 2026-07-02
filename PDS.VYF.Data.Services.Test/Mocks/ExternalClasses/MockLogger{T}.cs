using Microsoft.Extensions.Logging;
using Moq;

namespace PDS.VYF.Data.Services.Tests.Mocks.ExternalClasses
{
    /// <summary>
    /// The mock class for ILogger.
    /// </summary>
    public class MockLogger<T> : MockBase<ILogger<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockLogger{T}"/> class.
        /// </summary>
        /// <param name="isSetupDefault">if set to <c>true</c> [is setup default].</param>
        public MockLogger(bool isSetupDefault = true)
            : base(isSetupDefault)
        {
        }

        /// <summary>
        /// Mocks the log.
        /// </summary>
        public override void SetupDefault()
        {
            this.Setup(logger => logger.Log(
                                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                                    It.IsAny<EventId>(),
                                    It.IsAny<It.IsAnyType>(),
                                    It.IsAny<Exception>(),
                                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()));

            this.Setup(logger => logger.Log(
                                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                                    It.IsAny<EventId>(),
                                    It.IsAny<It.IsAnyType>(),
                                    It.IsAny<Exception>(),
                                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }

        /// <summary>
        /// Verifies the log.
        /// </summary>
        /// <typeparam name="T">Any Type.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="times">The times.</param>
        /// <returns>The same object for chaining.</returns>
        public MockLogger<T> VerifyLog(LogLevel logLevel, string message, Times times)
        {
            this.Verify(
                        logger => logger.Log(
                                It.Is<LogLevel>(l => l == logLevel),
                                It.IsAny<EventId>(),
                                It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains(message)),
                                It.IsAny<Exception>(),
                                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        times);

            return this;
        }
    }
}
