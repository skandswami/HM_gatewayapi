using System;
using Refit;

namespace WeatherApi
{
	public interface IGatewayWeatherApi
	{
        [Get("/")]
        Task<string> GetWeatherForLocationAsync(string latitude, string longitude, CancellationToken cancellationToken);
    }
}

