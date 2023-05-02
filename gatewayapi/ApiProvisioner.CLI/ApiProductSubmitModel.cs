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
        }
    }



