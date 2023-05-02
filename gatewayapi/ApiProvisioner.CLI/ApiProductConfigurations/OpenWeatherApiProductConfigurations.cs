using System;
using ApiProvisioner.CLI.Extensions;
using Gateway.ApiProvisioner.CLI;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiProvisioner.CLI.ApiProductConfigurations;

public class OpenWeatherApiProductConfigurations : IApiProductConfiguration
{
    public string? ProviderName => GetSubmitModel().ProviderName;

    public string? ProviderApiName => GetSubmitModel().ProviderApiName;

    public ApiProductSubmitModel GetSubmitModel()
    {
        Guid id = Guid.NewGuid();

        return new ApiProductSubmitModel
        {
            Id = id,
            ProviderName = "Weather",
            ProviderApiName = "OpenWeatherMaps",
            UpstreamProtocol = "https",
            UpstreamHost = "api.openweathermap.org",
            UpstreamPort = 80,
            Path = @"/data/2.5/weather",
            RouteName = "weather",
            RouthPaths = new List<string> { "/weather" },
        };
    }
}