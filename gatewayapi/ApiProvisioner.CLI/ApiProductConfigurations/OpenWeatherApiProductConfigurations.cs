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
        //const string specFolder = "openapi-mockapi";
        Guid id = Guid.NewGuid();

        ApiProductSubmitModel.ApiProductVersion.GatewayConfig defaultGatewayConfig = new()
        {
            RequestTransformation = new ApiProductSubmitModel.ApiProductVersion.GatewayConfig.RequestTransformationConfig
            {
                Add = new ApiProductSubmitModel.ApiProductVersion.GatewayConfig.RequestTransformationConfig.Item
                {
                    Headers = new List<string>
                    {
                        "x-dummy:dummy-header"
                    }
                }
            }
        };

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
            ApiProductVersions = new List<ApiProductSubmitModel.ApiProductVersion>
        {
            new ApiProductSubmitModel.ApiProductVersion
            {
                UpstreamRelativePath = "data/2.5/weather",
                Version = new(1, 0, 0),
                //Spec = ApiSpecExtensions.Read(specFolder, "1.0.0.json"),
                GatewayConfiguration = defaultGatewayConfig
            }
        }
        };
    }
}