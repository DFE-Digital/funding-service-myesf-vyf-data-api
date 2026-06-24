namespace PDS.VYF.Data.Services.ServiceRegistrations
{
    using Microsoft.Extensions.DependencyInjection;
    using PDS.VYF.Data.Services.Abstracts.AppServices;
    using PDS.VYF.Data.Services.HostedServices;
    using PDS.VYF.Data.Services.Implementations.AppServices;

    /// <summary>
    /// The class to register App Services.
    /// </summary>
    public static class AppServicesRegistrations
    {
        /// <summary>
        /// Registers the application services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>Services.</returns>
        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.AddHostedService<AzSearchInitializationService>();

            services.AddSingleton<IParentSearchServices, ParentSearchServices>();
            services.AddSingleton<IChildSearchServices, ChildSearchServices>();
            services.AddSingleton<IUserViewCountServices, UserViewCountServices>();
            services.AddSingleton<IInYearOpenerCalcServices, InYearOpenerCalcServices>();
            services.AddSingleton<IComparisonServices, ComparisonServices>();

            return services;
        }
    }
}
