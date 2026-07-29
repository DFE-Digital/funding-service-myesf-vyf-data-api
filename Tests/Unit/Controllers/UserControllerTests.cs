using FluentAssertions;
using FundingApi.Controllers;
using FundingApi.DTOs;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PDS.ViewYourFunding.Data.API;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Interfaces.Models;
using PDS.ViewYourFunding.Data.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Controllers
{
    /// <summary>
    /// The User controller tests.
    /// </summary>
    [TestClass, TestCategory("Unit")]
    public class UserControllerTests
    {
        /// <summary>
        /// The mapper.
        /// </summary>
        private static IMapper _mapper;

        /// <summary>
        /// The mock user funding view repository.
        /// </summary>
        private readonly Mock<IUserFundingViewRepository> _mockUserFundingViewRepository = new Mock<IUserFundingViewRepository>(MockBehavior.Strict);

        /// <summary>
        /// The mock logger service.
        /// </summary>
        private readonly Mock<ILogger<UserController>> _mockLoggerService = new Mock<ILogger<UserController>>(MockBehavior.Loose);

        #region Initialization

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TypeAdapterConfig config = new TypeAdapterConfig();
            config.Configure();
            _mapper = new Mapper(config);
        }

        #endregion

        #region HasUserVisitedFunding Tests

        [DataRow(true)]
        [DataRow(false)]
        [TestMethod]
        public async Task HasUserVisitedFunding(bool expectedResult)
        {
            var userId = "User Id";
            var fundingId = "Funding Id";

            // Arrange
            _mockUserFundingViewRepository.Setup(o => o.HasUserVisitedFunding(userId, fundingId)).ReturnsAsync(expectedResult);
            var userController = GetUserController();

            // Act
            var result = await userController.HasUserVisitedFunding(userId, fundingId);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task HasUserVisitedFunding_Repo_Throws_Exception()
        {
            var userId = "User Id";
            var fundingId = "Funding Id";

            // Arrange
            _mockUserFundingViewRepository.Setup(o => o.HasUserVisitedFunding(userId, fundingId)).Throws(new System.Exception());
            var userController = GetUserController();

            // Act
            var result = await userController.HasUserVisitedFunding(userId, fundingId);

            // Assert
            Assert.AreEqual(false, result);
        }

        #endregion


        #region AddUserFundingViewDetail Tests

        [TestMethod]
        public async Task AddUserFundingViewDetail_WhenRequestIsNull()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            var result = await userController.AddUserFundingViewDetail(null);

            // Assert
            result.Should().BeNull();
        }


        [TestMethod]
        public async Task AddUserFundingViewDetail_When_Service_Throws_Exception()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            _mockUserFundingViewRepository.Setup(o => o.AddOrReplace(It.IsAny<IUserFundingView>())).Throws(new System.Exception());
            var result = await userController.AddUserFundingViewDetail(new AddUserFundingViewRequest { UserId = "User Id", FundingId = "Funding Id" });

            // Assert
            result.Should().BeNull();
        }


        [TestMethod]
        public async Task AddUserFundingViewDetail_WhenRequestHasNullUserId()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            var result = await userController.AddUserFundingViewDetail(new AddUserFundingViewRequest { FundingId = "Funding Id" });

            // Assert
            result.Should().BeNull();
        }


        public async Task AddUserFundingViewDetail_WhenRequestHasNullFundingId()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            var result = await userController.AddUserFundingViewDetail(new AddUserFundingViewRequest { UserId = "User Id" });

            // Assert
            result.Should().BeNull();
        }


        [TestMethod]
        public async Task AddUserFundingViewDetail_WhenRequestIsValid()
        {
            // Arrange
            var userController = GetUserController();
            var request = new AddUserFundingViewRequest { UserId = "User Id", FundingId = "Funding Id" };

            _mockUserFundingViewRepository.Setup(o => o.AddOrReplace(It.IsAny<UserFundingView>())).Returns(Task.CompletedTask);

            // Act
            var result = await userController.AddUserFundingViewDetail(request);

            // Assert
            result.Should().BeTrue();
        }

        #endregion


        #region GetUserFundingViewCount Tests


        [TestMethod]
        public async Task GetUserFundingViewCount_WhenRequestIsNull()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            var result = await userController.GetUserFundingViewCount(null);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetUserFundingViewCount_When_Repo_Throws_Exception()
        {
            // Arrange
            var userController = GetUserController();
            _mockUserFundingViewRepository.Setup(o => o.GetUserFundingViewCount(It.IsAny<string>(), It.IsAny<List<FundingVersionDetail>>())).Throws(new System.Exception());

            // Act
            var result = await userController.GetUserFundingViewCount(new UserFundingViewCountRequest { UserId = "User Id", FundingVersionDetails = new List<FundingVersionDetail>() });

            // Assert
            result.Should().BeNull();
        }


        [TestMethod]
        public async Task GetUserFundingViewCount_WhenRequestHasNullUserId()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            var result = await userController.GetUserFundingViewCount(new UserFundingViewCountRequest { FundingVersionDetails = new List<FundingVersionDetail>() });

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetUserFundingViewCount_WhenRequestHasNullFundingIds()
        {
            // Arrange
            var userController = GetUserController();

            // Act
            var result = await userController.GetUserFundingViewCount(new UserFundingViewCountRequest { UserId = "User Id" });

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetUserFundingViewCount_WhenRequestIsValid()
        {
            // Arrange
            var userController = GetUserController();
            var request = new UserFundingViewCountRequest { UserId = "User Id", FundingVersionDetails = new List<FundingVersionDetail>() };
            var result = new UserFundingViewCount { UserId = "User Id", UnreadNewFundings = 10, UnreadUpdatedFundings = 20 };

            _mockUserFundingViewRepository.Setup(o => o.GetUserFundingViewCount(request.UserId, request.FundingVersionDetails)).ReturnsAsync(result);

            // Act
            var response = await userController.GetUserFundingViewCount(request);

            // Assert
            response.UserId.Should().Be(result.UserId);
            response.UnreadNewFundings.Should().Be(result.UnreadNewFundings);
            response.UnreadUpdatedFundings.Should().Be(result.UnreadUpdatedFundings);
        }

        #endregion


        #region Helpers

        private UserController GetUserController(bool throwException = false)
        {
            return new UserController(_mockUserFundingViewRepository.Object, _mockLoggerService.Object, _mapper);
        }

        #endregion
    }
}