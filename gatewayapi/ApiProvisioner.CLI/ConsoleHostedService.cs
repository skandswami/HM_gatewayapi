using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Diagnostics;
using ApiProvisioner.CLI;
using ApiProvisioner.CLI.Extensions;
using Refit;
using Adapters.KongGateway;
using Adapters.KongGateway.Models;

namespace Gateway.ApiProvisioner.CLI
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await PerformProvisioningAPIs();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        private async Task PerformProvisioningAPIs()
        {
            using Activity? activity = new ActivitySource("Provisioning-APIs").StartActivity();
            try
            {
                _logger.LogInformation("Starting gateway provisioning");

                string apiProductsApiUrl = Environment.GetEnvironmentVariable("APIPRODUCTSAPI_URL")!;

                _logger.LogInformation($"Provisioning apis on {apiProductsApiUrl}");

                IServiceCollection services = new ServiceCollection();
                services.RegisterAllTypes<IApiProductConfiguration>(new[] { typeof(Program).Assembly });

                ServiceProvider serviceProvider = services.BuildServiceProvider();

                List<ApiProductSubmitModel> apis = new();
                
                apis.AddRange(serviceProvider.GetServices<IApiProductConfiguration>().Where(x => true).Select(x => x.GetSubmitModel()));

                foreach (ApiProductSubmitModel a in apis)
                {
                    using Activity? apiactivity = new ActivitySource($"Provisioning:{a.ProviderName}-{a.ProviderApiName}").StartActivity();
                    string apiProductName = $"{a.ProviderName!.ToLower()}-{a.ProviderApiName!.ToLower()}";

                    try
                    {
                        _logger.LogInformation("Provisioning ApiProduct '{sb_apiProduct}'", apiProductName);

                        await CreateOrUpdateApiProductAsync(a, CancellationToken.None);
                        _logger.LogInformation("ApiProduct '{sb_apiProduct}' provisioned", apiProductName);
                    }
                    catch (Exception e)
                    {
                        //apiActivity?.SetStatus(ActivityStatusCode.Error, e.ToString());
                        _logger.LogInformation(e.ToString());
                    }
                }

                _logger.LogInformation("Done ApiProduct provisioning");
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                throw;
            }
        }

        public async Task CreateOrUpdateApiProductAsync(ApiProductSubmitModel apiProduct, CancellationToken cancellationToken)
        {
            string apiProductsApiUrl = Environment.GetEnvironmentVariable("APIPRODUCTSAPI_URL")!;

            var apiProductsWebApi = RestService.For<IKongWebApi>(new HttpClient
            {
                BaseAddress = new Uri(apiProductsApiUrl),
                Timeout = TimeSpan.FromMinutes(3)
            });

            Service service = new()
            {
                Name = apiProduct.ProviderApiName,
                Enabled = true,
                Protocol = apiProduct.UpstreamProtocol,
                Host = apiProduct.UpstreamHost,
                Port = apiProduct.UpstreamPort,
                Path = apiProduct.Path
            };

            Route route = new()
            {
                Name = apiProduct.RouteName,
                Paths = apiProduct.RouthPaths,
                Service = new ServiceRef { Name = service.Name },
                Methods = new List<string> { },
                Protocols = new List<string> { }
                
            };

            try
            {
                await apiProductsWebApi.CreateOrUpdateServiceAsync(service.Name!, service, cancellationToken);

                await apiProductsWebApi.CreateRouteForServiceAsync(service.Name!, route, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}