using System;
namespace WeatherApi.Endpoints.Requests
{
	public class WeatherRequest
	{
        public string Lat { get; set; } = default!;
        public string Long { get; set; } = default!;
    }
}

