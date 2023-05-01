using System;
namespace Gateway.ApiProvisioner.CLI
{
    public interface IApiProductConfiguration
    {
        string? ProviderName { get; }

        string? ProviderApiName { get; }

        ApiProductSubmitModel GetSubmitModel();
    }
}