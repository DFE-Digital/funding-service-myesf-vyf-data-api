using Microsoft.AspNetCore.Authorization;

namespace PDS.ViewYourFunding.Data.API.Helpers
{
    /// <summary>
    /// The toggle authorize requirement.
    /// </summary>
    public class ToggleAuthorizeRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Gets or sets a value indicating whether to enable oauth security.
        /// </summary>
        /// <value>
        /// Set to true if OAuth security is enabled.
        /// </value>
        public bool EnableOAuthSecurity { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleAuthorizeRequirement"/> class.
        /// </summary>
        /// <param name="enableOAuthSecurity">Set to true if OAuth security is enabled.</param>
        public ToggleAuthorizeRequirement(bool enableOAuthSecurity)
        {
            EnableOAuthSecurity = enableOAuthSecurity;
        }
    }
}
