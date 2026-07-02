using Moq;
using PDS.VYF.Data.Services.Abstracts.InfraServices;
using PDS.VYF.Data.Services.Enums;

namespace PDS.VYF.Data.Services.Tests.Mocks.InfraServices
{
    /// <summary>
    /// The Mock class of AzSearchSearchingServices.
    /// </summary>
    /// <seealso cref="PDS.VYF.Data.Services.Tests.Mocks.MockBase&lt;PDS.VYF.Data.Services.Implementations.InfraServices.AzSearchSearchingServices&gt;" />
    public class MockAzSearchSearchingServices : MockBase<IAzSearchSearchingServices>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockAzSearchSearchingServices"/> class.
        /// </summary>
        /// <param name="isSetupDefault">if set to <c>true</c> [is setup default].</param>
        public MockAzSearchSearchingServices(bool isSetupDefault = true)
            : base(isSetupDefault)
        {
        }

        /// <summary>
        /// Setups the default.
        /// </summary>
        public override void SetupDefault()
        {
            this.Setup(a => a.SearchDocumentAsync<List<string>>(It.IsAny<AzSearchIndexTypeEnum>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<List<string>>).Verifiable();
        }

        /// <summary>
        /// Setups the search document asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <param name="returnList">The return list.</param>
        /// <returns>The same object.</returns>
        public MockAzSearchSearchingServices SetupSearchDocumentAsync<T>(AzSearchIndexTypeEnum azSearchIndexTypeEnum, List<T> returnList)
            where T : class
        {
            this.Setup(a => a.SearchDocumentAsync<T>(azSearchIndexTypeEnum, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(returnList.ToAsyncEnumerable()).Verifiable();

            return this;
        }

        /// <summary>
        /// Verifies the search document asynchronous.
        /// </summary>
        /// <typeparam name="T">Any type.</typeparam>
        /// <param name="azSearchIndexTypeEnum">The az search index type enum.</param>
        /// <param name="hasNoColumnsRequested">if set to <c>true</c> [has no columns requested].</param>
        /// <param name="selectFields">The select fields.</param>
        /// <returns>The same object.</returns>
        public MockAzSearchSearchingServices VerifySearchDocumentAsync<T>(AzSearchIndexTypeEnum azSearchIndexTypeEnum, bool hasNoColumnsRequested, List<string> selectFields)
            where T : class
        {
            this.Verify(
                a => a.SearchDocumentAsync<T>(
                                azSearchIndexTypeEnum,
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.Is<string>(b => hasNoColumnsRequested || selectFields.All(c => b != null && b.Contains(c)))),
                Times.Once);

            return this;
        }
    }
}
