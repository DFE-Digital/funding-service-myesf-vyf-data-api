namespace PDS.VYF.Data.Services.HostedServices
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using PDS.ViewYourFunding.Data.Core;
    using PDS.VYF.Data.Services.Abstracts.InfraServices;
    using PDS.VYF.Data.Services.Enums;
    using PDS.VYF.Data.Services.Extensions;

    /// <summary>
    /// The Azure Search initialization service.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    public class AzSearchInitializationService : IHostedService
    {
        private readonly IAzSearchManagingServices azSearchManagingServices;
        private readonly IConfiguration configuration;
        private readonly IOptions<ApplicationConfiguration> applicationConfigurationOption;
        private readonly ApplicationConfiguration applicationConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzSearchInitializationService"/> class.
        /// </summary>
        /// <param name="azSearchManagingServices">The az search managing services.</param>
        /// <param name="configuration">The configuration.</param>
        public AzSearchInitializationService(
                        IAzSearchManagingServices azSearchManagingServices,
                        IConfiguration configuration,
                        IOptions<ApplicationConfiguration> applicationConfigurationOption)
        {
            this.azSearchManagingServices = azSearchManagingServices;
            this.configuration = configuration;
            this.applicationConfigurationOption = applicationConfigurationOption;
            this.applicationConfiguration = applicationConfigurationOption.Value;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            AzSearchIndexTypeExtension.AzResourceBaseName = this.applicationConfiguration.Repositories.AzureSearch.AzResourceBaseName;

            var searchIndexTypes = Enum
                .GetNames<AzSearchIndexTypeEnum>()
                .Select(a => Enum.Parse<AzSearchIndexTypeEnum>(a));

            foreach (var searchIndexType in searchIndexTypes)
            {
                await this.azSearchManagingServices.CreateOrReplaceAzSearchResources(searchIndexType, cancellationToken);
            }
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>The Task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}
