using Newtonsoft.Json;

namespace Adapters.KongGateway.Models
{
    public class Route
    {
        public string? Name { get; set; }
        public List<string>? Protocols { get; set; }
        public List<string>? Methods { get; set; }
        public List<string>? Hosts { get; set; }
        public List<string>? Paths { get; set; }
        
        [JsonProperty("strip_path")]
        public bool StripPath { get; set; } = true;
        public List<string>? Tags { get; set; }
        public ServiceRef? Service { get; set; }

        [JsonProperty("regex_priority")]
        public int Priority { get; set; }
    }
}



