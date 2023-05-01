//using System;
//using Adapters.KongGateway.Models;
//using Domain;
//using static Adapters.KongGateway.Models.RequestTransformerPlugin;

//namespace Adapters.KongGateway
//{
//    public class KongGateway : IGateway
//    {

//        private const string _adminGroupName = "admin-group";
//        private const string _tryitoutGroupName = "tryitout-group";

//        private const string _routePathExtension = "/(?<endpointpath>\\S+)";

//        private const string _systemTag = "system";

//        private readonly IKongWebApi _webApi;

//        public KongGateway(IKongWebApi webApi)
//        {
//            _webApi = webApi;
//        }

//        public async Task<ApiKongState> CreateOrUpdateApiProductAsync(ApiProduct apiProduct, CancellationToken cancellationToken)
//        {
//            ApiKongState state = new();

//            Service service = new()
//            {
//                Name = GetServiceName(apiProduct),
//                Enabled = true,
//                Protocol = apiProduct.UpstreamProtocol,
//                Host = apiProduct.UpstreamHost,
//                Port = apiProduct.UpstreamPort,
//                Tags = new List<string>
//            {
//                _systemTag
//            }
//            };

//            await _webApi.CreateOrUpdateServiceAsync(service.Name, service, cancellationToken);

//            if (string.IsNullOrWhiteSpace(service.Protocol))
//                throw new ApiProductInvalidOperationException($"{nameof(service.Protocol)} cannot be null");

//            Plugin aclPlugin = new()
//            {
//                Name = "acl",
//                Protocols = new List<string> { service.Protocol },
//                Tags = new List<string>
//            {
//                _systemTag
//            },
//                Config = new
//                {
//                    allow = new[] { GetServiceAclGroupName(apiProduct), _adminGroupName, _tryitoutGroupName },
//                    hide_groups_header = true,
//                }
//            };

//            ResponseEnvelope<Plugin> currentServicePlugins = await _webApi.GetPluginsForServiceAsync(service.Name, cancellationToken);

//            Plugin? currentAclPlugin = currentServicePlugins.Data.FirstOrDefault(x => x.Name != null && x.Name.ToLower() == aclPlugin.Name.ToLower());

//            await _webApi.CreateOrUpdatePluginsForServiceAsync(service.Name, currentAclPlugin?.Id ?? Guid.NewGuid().ToString(), aclPlugin, cancellationToken);

//            return state;
//        }

//        public async Task<ApiVersionKongState> CreateOrUpdateApiProductVersionAsync(ApiProduct apiProduct, ApiProductVersion apiProductVersion, CancellationToken cancellationToken)
//        {
//            string routePath = GetRoutePath(apiProduct, apiProductVersion);

//            if (string.IsNullOrWhiteSpace(apiProduct.UpstreamProtocol))
//                throw new ApiProductInvalidOperationException($"{nameof(apiProduct.UpstreamProtocol)} cannot be null");

//            // creating/updating normal route to upstream
//            Route route = new()
//            {
//                Name = GetRouteName(apiProduct, apiProductVersion),
//                Methods = new List<string> { "GET", "PUT", "POST", "OPTIONS" },
//                Protocols = new List<string> { apiProduct.UpstreamProtocol },
//                Service = new ServiceRef { Name = GetServiceName(apiProduct) },
//                StripPath = true,
//                Priority = 0,
//                Paths = new List<string> { $"{routePath}{_routePathExtension}" },
//                Tags = new List<string>
//                {
//                    _systemTag
//                },
//            };

//            await _webApi.UpdateOrCreateRouteForServiceAsync(GetServiceName(apiProduct), route.Name, route, cancellationToken);

//            List<Plugin> plugins = HandlePluginConfigs(apiProductVersion);

//            ResponseEnvelope<Plugin> currentRoutePlugins = await _webApi.GetPluginsForRouteAsync(route.Name, cancellationToken);

//            foreach (Plugin plugin in plugins)
//            {
//                plugin.Protocols = new List<string> { apiProduct.UpstreamProtocol };
//                plugin.Tags = new List<string>
//                {
//                    _systemTag
//                };

//                Plugin? currentPlugin = currentRoutePlugins.Data.FirstOrDefault(x => x.Name != null && x.Name.ToLower() == plugin.Name.ToLower());

//                await _webApi.CreateOrUpdatePluginsForRouteAsync(route.Name, currentPlugin?.Id ?? Guid.NewGuid().ToString(), plugin, cancellationToken);
//            }

//            // creating/updating healthz-route to upstreams healthzroute
//            if (apiProductVersion.HealthEndpoint is not null && apiProductVersion.HealthEndpoint.UpstreamRelativePath is not null && apiProductVersion.HealthEndpoint.Method is not null)
//            {
//                Route healthzRoute = new()
//                {
//                    Name = $"{GetRouteName(apiProduct, apiProductVersion)}-healthz",
//                    Methods = new List<string> { apiProductVersion.HealthEndpoint.Method },
//                    Protocols = new List<string> { apiProduct.UpstreamProtocol },
//                    Service = new ServiceRef { Name = GetServiceName(apiProduct) },
//                    StripPath = true,
//                    Priority = 100,
//                    Paths = new List<string> { $"{routePath}/hea\\wthz" },
//                    Tags = new List<string>
//                {
//                    _systemTag
//                },
//                };

//                await _webApi.UpdateOrCreateRouteForServiceAsync(GetServiceName(apiProduct), healthzRoute.Name, healthzRoute, cancellationToken);

//                RequestTransformerPluginConfiguration healthzRequestTransformerPluginConfig = GetBaseRequestTransformerPluginConfiguration(apiProductVersion);

//                healthzRequestTransformerPluginConfig.Replace ??= new RequestTransformerPluginConfigurationItem();
//                healthzRequestTransformerPluginConfig.Replace.Uri = apiProductVersion.HealthEndpoint.UpstreamRelativePath;

//                Plugin healthzRequestTransformerPlugin = new()
//                {
//                    Config = healthzRequestTransformerPluginConfig,
//                    Name = "request-transformer"
//                };

//                ResponseEnvelope<Plugin> currentHealthzRoutePlugins = await _webApi.GetPluginsForRouteAsync(healthzRoute.Name, cancellationToken);
//                Plugin? currentHealthzRequestTransformerPlugin = currentHealthzRoutePlugins.Data.FirstOrDefault(x => x.Name != null && x.Name.ToLower() == healthzRequestTransformerPlugin.Name.ToLower());

//                await _webApi.CreateOrUpdatePluginsForRouteAsync(healthzRoute.Name, currentHealthzRequestTransformerPlugin?.Id ?? Guid.NewGuid().ToString(), healthzRequestTransformerPlugin, cancellationToken);
//            }

//            return new ApiVersionKongState(apiProductVersion.Version, routePath);
//        }

//        private List<Plugin> HandlePluginConfigs(ApiProductVersion version)
//        {
//            if (string.IsNullOrWhiteSpace(version.UpstreamRelativePath))
//                throw new ApiProductInvalidOperationException($"{nameof(version.UpstreamRelativePath)} cannot be null");

//            List<Plugin>? plugins = new();

//            RequestTransformerPluginConfiguration requestTransformerPluginConfig = GetBaseRequestTransformerPluginConfiguration(version);

//            requestTransformerPluginConfig.Replace ??= new RequestTransformerPluginConfigurationItem();
//            requestTransformerPluginConfig.Replace.Uri = Combine(version.UpstreamRelativePath, "$(uri_captures[\"endpointpath\"])");

//            requestTransformerPluginConfig.Remove ??= new RequestTransformerPluginConfigurationItem();
//            requestTransformerPluginConfig.Remove.Headers ??= new List<string>();

//            requestTransformerPluginConfig.Remove.Headers.Add("origin");
//            requestTransformerPluginConfig.Remove.Headers.Add("switchboard-key");

//            plugins.Add(new Plugin
//            {
//                Config = requestTransformerPluginConfig,
//                Name = "request-transformer"
//            });

//            return plugins;
//        }

//        private static RequestTransformerPluginConfiguration GetBaseRequestTransformerPluginConfiguration(ApiProductVersion version)
//        {
//            return new()
//            {
//                Add = version.GatewayConfig?.RequestTransformation?.Add is null ? null : new RequestTransformerPluginConfigurationItem
//                {
//                    Body = version.GatewayConfig?.RequestTransformation?.Add?.Body,
//                    Headers = version.GatewayConfig?.RequestTransformation?.Add?.Headers,
//                    Querystring = version.GatewayConfig?.RequestTransformation?.Add?.Querystring,
//                    Uri = version.GatewayConfig?.RequestTransformation?.Add?.Uri
//                },
//                Append = version.GatewayConfig?.RequestTransformation?.Append is null ? null : new RequestTransformerPluginConfigurationItem
//                {
//                    Body = version.GatewayConfig?.RequestTransformation?.Append?.Body,
//                    Headers = version.GatewayConfig?.RequestTransformation?.Append?.Headers,
//                    Querystring = version.GatewayConfig?.RequestTransformation?.Append?.Querystring,
//                    Uri = version.GatewayConfig?.RequestTransformation?.Append?.Uri
//                },
//                Remove = version.GatewayConfig?.RequestTransformation?.Remove is null ? null : new RequestTransformerPluginConfigurationItem
//                {
//                    Body = version.GatewayConfig?.RequestTransformation?.Remove?.Body,
//                    Headers = version.GatewayConfig?.RequestTransformation?.Remove?.Headers,
//                    Querystring = version.GatewayConfig?.RequestTransformation?.Remove?.Querystring,
//                    Uri = version.GatewayConfig?.RequestTransformation?.Remove?.Uri
//                },
//                Rename = version.GatewayConfig?.RequestTransformation?.Rename is null ? null : new RequestTransformerPluginConfigurationItem
//                {
//                    Body = version.GatewayConfig?.RequestTransformation?.Rename?.Body,
//                    Headers = version.GatewayConfig?.RequestTransformation?.Rename?.Headers,
//                    Querystring = version.GatewayConfig?.RequestTransformation?.Rename?.Querystring,
//                    Uri = version.GatewayConfig?.RequestTransformation?.Rename?.Uri
//                },
//                Replace = new RequestTransformerPluginConfigurationItem
//                {
//                    Body = version.GatewayConfig?.RequestTransformation?.Replace?.Body,
//                    Headers = version.GatewayConfig?.RequestTransformation?.Replace?.Headers,
//                    Querystring = version.GatewayConfig?.RequestTransformation?.Replace?.Querystring,
//                    Uri = version.GatewayConfig?.RequestTransformation?.Replace?.Uri
//                }
//            };
//        }

//        private static string GetServiceName(ApiProduct apiProduct)
//        {
//            return apiProduct.Name.ToLower();
//        }

//        private static string GetServiceAclGroupName(ApiProduct apiProduct)
//        {
//            return $"{GetServiceName(apiProduct)}-group";
//        }

//        private static string GetRouteName(ApiProduct apiProduct, ApiProductVersion apiProductVersion)
//        {
//            return $"{GetServiceName(apiProduct)}-route-v{apiProductVersion.Version?.ToString()}";
//        }

//        private static string GetRoutePath(ApiProduct apiProduct, ApiProductVersion apiProductVersion)
//        {
//            return @$"/{GetServiceName(apiProduct).Replace('-', '/')}/v{apiProductVersion.Version?.ToString()}";
//        }

//        private static string Combine(string uri1, string uri2)
//        {
//            uri1 = uri1.TrimEnd('/');
//            uri2 = uri2.TrimStart('/');
//            return $"{uri1}/{uri2}";
//        }
//    }

//    public record PluginConfigs(List<Plugin>? Plugins);
//}

