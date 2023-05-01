using System.Text.Json.Serialization;

namespace Adapters.KongGateway.Models
{
    public class Consumer
    {
        [JsonPropertyName("custom_id")]
        public string? CustomId { get; set; }

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        public string? Username { get; set; }

        public List<string>? Tags { get; set; }
    }
}

