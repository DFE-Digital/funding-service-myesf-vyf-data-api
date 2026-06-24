using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Pds.Core.ApiAuthentication;
using Pds.Core.Telemetry.ApplicationInsights;
using PDS.ViewYourFunding.Data.API.Helpers;
using PDS.ViewYourFunding.Data.Core;
using PDS.ViewYourFunding.Data.Interfaces;
using PDS.ViewYourFunding.Data.Services;
using PDS.ViewYourFunding.Data.Services.ServiceRegistrations;
using PDS.VYF.Data.Services.ServiceRegistrations;
using System;

namespace PDS.ViewYourFunding.Data.API
{
    /// <summary>
    /// The Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationConfiguration>(options =>
            {
                Configuration.Bind(options);
            });

            services.Configure<Authentication>(options =>
            {
                Configuration.Bind(nameof(Authentication), options);
            });

            services.AddAzureADAuthentication(Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    nameof(ToggleAuthorizeRequirement),
                    policy => policy.Requirements.Add(new ToggleAuthorizeRequirement(Convert.ToBoolean(Configuration[nameof(ApplicationConfiguration.EnableOAuthSecurity)]))));
            });

            services.AddSingleton<IAuthorizationHandler, ToggleAuthorizeHandler>();
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup));
            services.AddHttpContextAccessor();

            services.AddPdsApplicationInsightsTelemetry(BuildAppInsightsConfiguration);

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.RegisterAzSearchService();
            services.RegisterFundingSearchService();

            services.AddSingleton<IAzureTableStorageClient>(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IOptions<ApplicationConfiguration>>().Value;

                return new AzureTableStorageClient(configuration.StorageAccount.AccountName, configuration.StorageAccount.AccountKey);
            });

            services.AddTransient<IUserFundingViewRepository, UserFundingViewRepository>();

            services.RegisterInfraServices();
            services.RegisterAppServices();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void BuildAppInsightsConfiguration(PdsApplicationInsightsConfiguration options)
        {
            Configuration.Bind("PdsApplicationInsights", options);
            options.Component = this.GetType().Assembly.GetName().Name;
        }
    }
}