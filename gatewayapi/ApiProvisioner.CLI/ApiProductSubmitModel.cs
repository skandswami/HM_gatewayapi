using System;
using FluentValidation;

namespace Gateway.ApiProvisioner.CLI;


    public class ApiProductSubmitModel
    {
        public Guid? Id { get; set; }

        public string? ProviderName { get; set; }

        public string? ProviderApiName { get; set; }

        public string? UpstreamProtocol { get; set; }

        public string? UpstreamHost { get; set; }

        public string? Path { get; set; }

        public string? RouteName { get; set; }

        public List<string>? RouthPaths { get; set; }

    public int UpstreamPort { get; set; }

        public IEnumerable<ApiProductPlan>? PlanConfig { get; set; }

        public IEnumerable<ApiProductVersion>? ApiProductVersions { get; set; }

        public record ApiProductPlan(string? Name, int CallsPerMonth, decimal? PricePerMonth, decimal? PricePerExtraCall, int TrialDays = 0, int TrialCallsPerDay = 0);

        public class ApiProductVersion
        {
            public string? Spec { get; set; }

            public Version? Version { get; set; }

            public string? UpstreamRelativePath { get; set; }

            public GatewayConfig? GatewayConfiguration { get; set; }

            public SpecificationTransformation? SpecTransformation { get; set; }

            public HealthEndpointConfig? HealthEndpoint { get; set; }

            public class GatewayConfig
            {
                public RequestTransformationConfig? RequestTransformation { get; set; }

                public class RequestTransformationConfig
                {
                    public Item? Add { get; set; }

                    public Item? Append { get; set; }

                    public Item? Replace { get; set; }

                    public Item? Rename { get; set; }

                    public Item? Remove { get; set; }

                    public class Item
                    {
                        public List<string>? Headers { get; set; }

                        public List<string>? Querystring { get; set; }

                        public List<string>? Body { get; set; }

                        public string? Uri { get; set; }
                    }
                }
            }

            public class SpecificationTransformation
            {
                public List<string>? PathsToKeep { get; set; }

                public List<string>? Removes { get; set; }

                public Dictionary<string, string>? PathReplaces { get; set; }
            }

            public class HealthEndpointConfig
            {
                public bool IsInvisibleForCustomer { get; set; }

                public string? Method { get; set; }

                public string? UpstreamRelativePath { get; set; }
            }
        }

        public bool ForceProvisioning { get; set; }
    }

    public class ApiProductSubmitModelValidation : AbstractValidator<ApiProductSubmitModel>
    {
        public ApiProductSubmitModelValidation()
        {
            RuleFor(m => m.ProviderName).NotEmpty();
            RuleFor(m => m.ProviderApiName).NotEmpty();
            RuleFor(m => m.UpstreamHost).NotEmpty();
            RuleFor(m => m.UpstreamProtocol).NotEmpty();
            RuleFor(m => m.UpstreamPort).NotEmpty();

            RuleForEach(m => m.PlanConfig).SetValidator(new ApiProductPlanValidator());
            RuleForEach(m => m.ApiProductVersions).SetValidator(new OpenApiSpecValidator());

            RuleFor(m => m.ApiProductVersions)
                .Must(specs => specs.Count() == specs.Select(u => u.Version).Distinct().Count())
                .WithMessage($"The {nameof(ApiProductSubmitModel.ApiProductVersion.Version)} must be unique in the list");
        }

        public class ApiProductPlanValidator : AbstractValidator<ApiProductSubmitModel.ApiProductPlan>
        {
            public ApiProductPlanValidator()
            {
                RuleFor(m => m.Name).NotEmpty();
                RuleFor(m => m.CallsPerMonth).NotNull();
                RuleFor(m => m.PricePerMonth).NotNull();
                RuleFor(m => m.PricePerMonth).NotNull();
            }
        }

        public class OpenApiSpecValidator : AbstractValidator<ApiProductSubmitModel.ApiProductVersion>
        {
            public OpenApiSpecValidator()
            {
                RuleFor(m => m.Spec).NotEmpty();
            }
        }
    }



