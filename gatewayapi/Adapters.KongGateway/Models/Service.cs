namespace Adapters.KongGateway.Models
{
    public class Service
    {
        public string? Name { get; set; }
        public string? Protocol { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Path { get; set; }
        public List<string>? Tags { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
