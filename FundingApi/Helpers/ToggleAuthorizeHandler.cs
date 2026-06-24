using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.API.Helpers
{
    /// <summary>
    /// The Toggle Authorize handler.
    /// </summary>
    /// <seealso cref="ToggleAuthorizeRequirement" />
    public class ToggleAuthorizeHandler : AuthorizationHandler<ToggleAuthorizeRequirement>
    {
        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <returns>The result of requirement evaluation.</returns>
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ToggleAuthorizeRequirement requirement)
        {
            if (requirement.EnableOAuthSecurity)
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                // Call 'Succeed' to mark current requirement as passed
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
