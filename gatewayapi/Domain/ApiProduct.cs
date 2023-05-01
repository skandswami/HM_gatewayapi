using System;
namespace Domain
{
    public class ApiProduct
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ApiProduct() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public ApiProduct(string providerName, string providerApiName)
        {
            Id = Guid.NewGuid();
            ProviderName = providerName;
            ProviderApiName = providerApiName;
            CreatedAt = DateTimeOffset.UtcNow;
            Name = BuildName(providerName, providerApiName);
        }

        public ApiProduct(Guid id, string providerName, string providerApiName)
            : this(providerName, providerApiName)
        {
            Id = id;
        }

        public Guid Id { get; set; }

        public string Name { get; private set; }

        public string ProviderName { get; set; }

        public string ProviderApiName { get; set; }

        public string? UpstreamProtocol { get; set; }

        public string? UpstreamHost { get; set; }

        public int UpstreamPort { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string? Hash { get; set; }

        public static string BuildName(string providerName, string providerApiName) => $"{providerName.ToLower()}-{providerApiName.ToLower()}";
    }
}

