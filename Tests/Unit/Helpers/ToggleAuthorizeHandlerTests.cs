using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDS.ViewYourFunding.Data.API.Helpers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Tests.Unit.Helpers
{
    /// <summary>
    /// The Toggle Authorize Handler Tests.
    /// </summary>
    [TestClass]
    public class ToggleAuthorizeHandlerTests
    {
        /// <summary>
        /// The toggle authorization handler should succeed when security is disabled.
        /// </summary>
        /// <returns>The expected requirement result.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ToggleAuthorizationHandler_SecurityDisabled_Should_Succeed()
        {
            // Arrange
            var requirements = new[]
            {
                new ToggleAuthorizeRequirement(false)
            };

            var user = GetUser(false);
            var context = new AuthorizationHandlerContext(requirements, user, null);
            var toggleAuthorizeHandler = new ToggleAuthorizeHandler();

            // Act
            await toggleAuthorizeHandler.HandleAsync(context);

            // Assert
            context.HasSucceeded.Should().BeTrue();
        }

        /// <summary>
        /// The toggle authorization handler should fail when security is enabled but the user is not authenticated.
        /// </summary>
        /// <returns>The expected requirement result.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ToggleAuthorizationHandler_UserNotAuthenticated_Should_Fail()
        {
            // Arrange
            var requirements = new[]
            {
                new ToggleAuthorizeRequirement(true)
            };

            var user = GetUser(false);
            var context = new AuthorizationHandlerContext(requirements, user, null);
            var toggleAuthorizeHandler = new ToggleAuthorizeHandler();

            // Act
            await toggleAuthorizeHandler.HandleAsync(context);

            // Assert
            context.HasFailed.Should().BeTrue();
        }

        /// <summary>
        /// The toggle authorization handler should succeed when security is enabled and user is authenticated.
        /// </summary>
        /// <returns>The expected requirement result.</returns>
        [TestMethod, TestCategory("Unit")]
        public async Task ToggleAuthorizationHandler_UserAuthenticated_Should_Succeed()
        {
            // Arrange
            var requirements = new[]
            {
                new ToggleAuthorizeRequirement(true)
            };

            var user = GetUser(true);
            var context = new AuthorizationHandlerContext(requirements, user, null);
            var toggleAuthorizeHandler = new ToggleAuthorizeHandler();

            // Act
            await toggleAuthorizeHandler.HandleAsync(context);

            // Assert
            context.HasSucceeded.Should().BeTrue();
        }

        /// <summary>
        /// Gets the authenticated user.
        /// </summary>
        /// <param name="authenticated">The authenticated flag.</param>
        /// <returns>ClaimsPrincipal.</returns>
        private static ClaimsPrincipal GetUser(bool authenticated)
        {
            if (authenticated)
            {
                var user = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[]
                        {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, nameof(ClaimsIdentity.DefaultNameClaimType)),
                        },
                        "Basic"));
                return user;
            }

            return new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
