using System;
namespace ApiProvisioner.CLI
{
    public static class ApiProductsWebApiRoutes
    {
        public const string ApiProducts = "apiproducts";
        public const string ApiProductsGetById = ApiProducts + "/{id}";
        public const string ApiProductsGetByPlanId = ApiProducts + "/getByPlanId/{planId}";

        public const string ApiProductsGetPlans = ApiProducts + "/{id}/plans";

        public const string ApiProductsGetVersion = ApiProducts + "/{id}/versions/{version}";
        public const string ApiProductsGetLatestVersion = ApiProducts + "/{id}/versions/latest";
    }
}

