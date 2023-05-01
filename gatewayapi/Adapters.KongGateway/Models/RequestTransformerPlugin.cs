// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace Adapters.KongGateway.Models;

public class RequestTransformerPlugin
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public List<string>? Protocols { get; set; }
    public List<string>? Tags { get; set; }
    public RequestTransformerPluginConfiguration? Config { get; set; }

    public class RequestTransformerPluginConfiguration
    {
        public RequestTransformerPluginConfigurationItem? Add { get; set; }

        public RequestTransformerPluginConfigurationItem? Append { get; set; }

        public RequestTransformerPluginConfigurationItem? Replace { get; set; }

        public RequestTransformerPluginConfigurationItem? Rename { get; set; }

        public RequestTransformerPluginConfigurationItem? Remove { get; set; }
    }

    public class RequestTransformerPluginConfigurationItem
    {
        public List<string>? Headers { get; set; }

        public List<string>? Querystring { get; set; }

        public List<string>? Body { get; set; }

        public string? Uri { get; set; }
    }
}
