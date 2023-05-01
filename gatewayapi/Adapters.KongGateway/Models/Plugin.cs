namespace Adapters.KongGateway.Models
{
    public class Plugin
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Protocols { get; set; }
        public List<string>? Tags { get; set; }
        public object? Config { get; set; }
    }
}
