using System;
using System.Net;
using System.Runtime.InteropServices;
using Adapters.KongGateway;
using Adapters.KongGateway.Models;
using Refit;

namespace Adapters.KongGateway;

public partial interface IKongWebApi
{

    //GET method
    [Get("/services")]
    Task<ResponseEnvelope<Service>> GetListOfServicesAsync(CancellationToken cancellationToken);

    [Get("/certificates/{certificateName}/services")]
    Task<ResponseEnvelope<Service>> GetListOfServicesForCertificateAsync(string certificateName, CancellationToken cancellationToken);

    [Get("/services/{serviceName}")]
    Task<Service> GetServiceAsync(string serviceName, CancellationToken cancellationToken);

    [Get("/certificates/{certificateId}/services/{serviceName}")]
    Task<Service> GetServiceForCertificateAsync(string certificateId, string serviceName, CancellationToken cancellationToken);

    [Get("/routes/{routeName}/service")]
    Task<Service> GetServiceForRouteAsync(string routeName, CancellationToken cancellationToken);

    [Get("/plugins/{pluginId}/service")]
    Task<Service> GetServiceForPluginAsync(string pluginId, CancellationToken cancellationToken);

    async Task<bool> ExistsServiceByNameAsync(string serviceName, CancellationToken cancellationToken)
    {
        try
        {
            await GetServiceAsync(serviceName, cancellationToken);
            return true;
        }
        catch (ApiException ae) when (ae.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    //POST method
    [Post("/services")]
    Task<Service> CreateServiceAsync([Body] Service model, CancellationToken cancellationToken);

    [Post("/certificates/{certificateName}/services")]
    Task<Service> CreateServiceForCertificateAsync(string certificateName, [Body] Service model, CancellationToken cancellationToken);

    //PATCH method
    [Patch("/services/{serviceName}")]
    Task<Service> UpdateServiceAsync(string serviceName, [Body] Service model, CancellationToken cancellationToken);

    [Patch("/certificates/{certificateId}/services/{serviceName}")]
    Task<Service> UpdateServiceForCertificateAsync(string certificateId, string serviceName, [Body] Service model, CancellationToken cancellationToken);

    [Patch("/routes/{routeName}/service")]
    Task<Service> UpdateServiceForRouteAsync(string routeName, [Body] Service model, CancellationToken cancellationToken);

    [Patch("/plugins/{pluginId}/service")]
    Task<Service> UpdateServiceForPluginAsync(string pluginId, [Body] Service model, CancellationToken cancellationToken);


    //PUT method
    [Put("/services/{serviceName}")]
    Task<Service> CreateOrUpdateServiceAsync(string serviceName, [Body] Service model, CancellationToken cancellationToken);

    [Put("/certificates/{certificateId}/services/{serviceName}")]
    Task<Service> CreateOrUpdateServiceAsync(string certificateId, string serviceName, [Body] Service model, CancellationToken cancellationToken);

    [Put("/routes/{routeName}/service")]
    Task<Service> CreateOrUpdateServiceForRouteAsync(string routeName, [Body] Service model, CancellationToken cancellationToken);

    [Put("/plugins/{pluginId}/service")]
    Task<Service> CreateOrUpdateServiceForPluginAsync(string pluginId, [Body] Service model, CancellationToken cancellationToken);


    //DELETE method
    [Delete("/services/{serviceName}")]
    Task DeleteServiceAsync(string serviceName, CancellationToken cancellationToken);

    [Delete("/certificates/{certificateId}/services/{serviceName}")]
    Task DeleteServiceForCertificateAsync(string certificateId, string serviceName, CancellationToken cancellationToken);

    [Post("/services/{serviceName}/routes")]
    Task<Route> CreateRouteForServiceAsync(string serviceName, [Body] Route model, CancellationToken cancellationToken);
}