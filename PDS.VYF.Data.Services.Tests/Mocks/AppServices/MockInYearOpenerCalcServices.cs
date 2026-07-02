using Moq;
using PDS.VYF.Data.Services.Abstracts.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;

namespace PDS.VYF.Data.Services.Tests.Mocks.AppServices
{
    /// <summary>
    /// The Mock InYearOpenerCalcServices.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Tests.Mocks.MockBase&lt;PDS.VYF.Data.Services.Abstracts.AppServices.IInYearOpenerCalcServices&gt;" />
    public class MockInYearOpenerCalcServices : MockBase<IInYearOpenerCalcServices>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockInYearOpenerCalcServices"/> class.
        /// </summary>
        /// <example>
        ///   <code>
        /// var mock = new Mock&lt;IFormatProvider&gt;();
        /// </code>
        /// </example>
        public MockInYearOpenerCalcServices()
            : base(false)
        {
        }

        /// <summary>
        /// Setups the default.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Not required. If called thrown not implemented exceptions.</exception>
        public override void SetupDefault()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Setups the is in year opener.
        /// </summary>
        /// <param name="inYearOpenerIds">The in year opener ids.</param>
        /// <returns>The same object.</returns>
        public MockInYearOpenerCalcServices SetupIsInYearOpener(params string[] inYearOpenerIds)
        {
            this
                .Setup(a => a.IsInYearOpener(It.Is<LoggedInChildAzSearchModel>(b => inYearOpenerIds.Any(c => c == b.Id))))
                .Returns(true)
                .Verifiable();

            this
                .Setup(a => a.IsInYearOpener(It.Is<LoggedInChildAzSearchModel>(b => !inYearOpenerIds.Any(c => c == b.Id))))
                .Returns(false)
                .Verifiable();

            return this;
        }

        /// <summary>
        /// Verifies the is in year opener.
        /// </summary>
        /// <param name="methodCallExpected">if set to <c>true</c> [method call expected].</param>
        /// <param name="countTotalRecords">The count total records.</param>
        /// <param name="inYearOpenerIds">The in year opener ids.</param>
        /// <returns>The same object.</returns>
        public MockInYearOpenerCalcServices VerifyIsInYearOpener(bool methodCallExpected, int countTotalRecords, params string[] inYearOpenerIds)
        {
            this
                .Setup(a => a.IsInYearOpener(It.Is<LoggedInChildAzSearchModel>(b => inYearOpenerIds.Any(c => c == b.Id))))
                .Returns(true)
                .Verifiable();

            this
                .Setup(a => a.IsInYearOpener(It.Is<LoggedInChildAzSearchModel>(b => !inYearOpenerIds.Any(c => c == b.Id))))
                .Returns(false)
                .Verifiable();

            if (methodCallExpected)
            {
                int iyoRecords = inYearOpenerIds.Count();

                this
                    .Verify(a => a.IsInYearOpener(It.Is<LoggedInChildAzSearchModel>(b => inYearOpenerIds.Any(c => c == b.Id))), Times.Exactly(iyoRecords));

                this
                    .Verify(a => a.IsInYearOpener(It.Is<LoggedInChildAzSearchModel>(b => !inYearOpenerIds.Any(c => c == b.Id))), Times.Exactly(countTotalRecords - iyoRecords));
            }
            else
            {
                this
                    .Verify(a => a.IsInYearOpener(It.IsAny<LoggedInChildAzSearchModel>()), Times.Never);
            }


            return this;
        }
    }
}
