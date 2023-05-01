using System;
using System.Runtime.InteropServices;
using Gateway.ApiProvisioner.CLI;
using Refit;


namespace ApiProvisioner.CLI
{
    public interface IApiProductsWebApi
    {
        [Post("/" + ApiProductsWebApiRoutes.ApiProducts)]
        Task CreateOrUpdateApiProductAsync([Body] ApiProductSubmitModel submitModel, CancellationToken cancellationToken);

        //[Get("/" + ApiProductsWebApiRoutes.ApiProducts)]
        //Task<List<ApiProductListProjection>> GetAllApiProductsAsync(CancellationToken cancellationToken);

        //[Get("/" + ApiProductsWebApiRoutes.ApiProductsGetPlans)]
        //Task<List<ApiProductPlanListProjection>> GetAllPlansForApiProduct(Guid id, CancellationToken cancellationToken);

        //[Get("/" + ApiProductsWebApiRoutes.ApiProductsGetByPlanId)]
        //Task<ApiProductProjection?> GetApiProductsByPlanId(string planId, CancellationToken cancellationToken);

        //[Get("/" + ApiProductsWebApiRoutes.ApiProductsGetById)]
        //Task<ApiProductProjection?> GetApiProductsById(Guid id, CancellationToken cancellationToken);

        [Get("/" + ApiProductsWebApiRoutes.ApiProductsGetLatestVersion)]
        Task<string?> GetApiProductsLatestVersion(Guid id, CancellationToken cancellationToken);

        [Get("/" + ApiProductsWebApiRoutes.ApiProductsGetVersion)]
        Task<string?> GetApiProductsVersion(Guid id, string version, CancellationToken cancellationToken);
    }
}

