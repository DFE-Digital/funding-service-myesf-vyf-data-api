using Azure;
using Azure.Data.Tables;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes.Models;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.VYF.Data.Services.Abstracts.AppServices;
using PDS.VYF.Data.Services.Abstracts.InfraServices;
using PDS.VYF.Data.Services.Implementations.AppServices;
using PDS.VYF.Data.Services.Models.AzSearchModels;
using PDS.VYF.Data.Services.Models.RequestModels;
using PDS.VYF.Data.Services.Models.ResponseModels;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PDS.VYF.Data.Services.Tests.Implementations.AppServices
{
    /// <summary>
    /// The User View Count Services Tests.
    /// </summary>
    [TestClass]
    public class UserViewCountServicesTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<IAzTableServices> mockAzTableServices;
        private readonly string fakeUserId = "aFakeUserId@education.gov.uk";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewCountServicesTests"/> class.
        /// </summary>
        public UserViewCountServicesTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockAzTableServices = this.mockRepository.Create<IAzTableServices>();
        }

        /// <summary>
        /// Gets the user view count state under test expected behavior.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task GetUserViewCount_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var userViewCountServices = this.CreateUserViewCountServices();

            var userViewCountRequest = new UserViewCountRequestModel()
            {
                ChildStatements = new List<ChildStatementModel>
                {
                    new () { ChildId = "GAG-AC-2425-12345678-1_0", StatementType = "new" },
                    new () { ChildId = "GAG-AC-2122-10086755-1_0", StatementType = "new" },
                    new () { ChildId = "GAG-AC-2122-10086745-1_0", StatementType = "new" },
                    new () { ChildId = "GAG-AC-2122-10086755-5_0", StatementType = "Updated" },
                    new () { ChildId = "GAG-AC-2425-1234535-1_0", StatementType = "new" }
                },
                UserId = fakeUserId
            };

            var expectedResult = new UserViewCountResponse()
            {
                NewCount = 0,
                UpdatedCount = 1
            };

            var azTableUserFundingView = new Dictionary<string, bool>()
            {
                { "GAG-AC-2122-10086745-1_0", true },
            };

            var ids = mockAzTableServices.Setup(s => s.GetViewChildrenIdAsync(fakeUserId)).ReturnsAsync(azTableUserFundingView);

            // Act
            var result = await userViewCountServices.GetUserViewCount(userViewCountRequest);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Determines whether [has user visited true].
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task HasUserVisited_True()
        {
            var userViewCountServices = this.CreateUserViewCountServices();

            var boolvalue = mockAzTableServices.Setup(s => s.IsUserViewedProviderFundingId(fakeUserId, "GAG-AC-2425-12345678-1_0")).ReturnsAsync(true);

            // Act
            var result = await userViewCountServices.HasUserVisited(fakeUserId, "GAG-AC-2425-12345678-1_0");

            // Assert
            result.Should().BeTrue();
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Determines whether [has user visited false].
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task HasUserVisited_False()
        {
            var userViewCountServices = this.CreateUserViewCountServices();

            var boolvalue = mockAzTableServices.Setup(s => s.IsUserViewedProviderFundingId(fakeUserId, "GAG-AC-2425-12345678-1_0")).ReturnsAsync(false);

            // Act
            var result = await userViewCountServices.HasUserVisited(fakeUserId, "GAG-AC-2425-12345678-1_0");

            // Assert
            result.Should().BeFalse();
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Adds the user visited information if entity not available true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task AddUserVisitedInfo_IfEntityAlreadyNotAvailable_true()
        {
            var userViewCountServices = this.CreateUserViewCountServices();

            var entity = new TableEntity(fakeUserId, "GAG-AC-2425-12345678-1_0")
                {
                    { "ViewedAt", DateTime.UtcNow }
                };

            mockAzTableServices
                    .Setup(s => s.IsUserViewedProviderFundingId(fakeUserId, "GAG-AC-2425-12345678-1_0")).ReturnsAsync(false);

            mockAzTableServices.Setup(x => x.UpsertEntityAsync(
                                                    It.Is<TableEntity>(a => a.PartitionKey == fakeUserId
                                                                                && a.RowKey == "GAG-AC-2425-12345678-1_0")))
                                .ReturnsAsync(true);

            // Act
            var expectedResult = await userViewCountServices.AddUserVisitedInfo(fakeUserId, "GAG-AC-2425-12345678-1_0");

            // Assert
            expectedResult.Should().BeTrue();
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Adds the user visited information if entity available true.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        public async Task AddUserVisitedInfo_IfEntityAlreadyAvailable_true()
        {
            var userViewCountServices = this.CreateUserViewCountServices();

            var entity = new TableEntity(fakeUserId, "GAG-AC-2425-12345678-1_0")
                {
                    { "ViewedAt", DateTime.UtcNow }
                };

            mockAzTableServices.Setup(s => s.IsUserViewedProviderFundingId(fakeUserId, "GAG-AC-2425-12345678-1_0")).ReturnsAsync(true);

            // Act
            var expectedResult = await userViewCountServices.AddUserVisitedInfo(fakeUserId, "GAG-AC-2425-12345678-1_0");

            // Assert
            expectedResult.Should().BeTrue();
            mockRepository.VerifyAll();
        }

        private UserViewCountServices CreateUserViewCountServices()
        {
            return new UserViewCountServices(
                this.mockAzTableServices.Object);
        }
    }
}
