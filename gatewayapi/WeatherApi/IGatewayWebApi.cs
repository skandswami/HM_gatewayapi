using System;
using Refit;
namespace WeatherApi;

public interface IGatewayWebApi
{
    [Get("/weather?lat={lat}&lon={lon}&appid={apikey}")]
    Task<string> GetWeatherForLocationAsync(string lat, string lon, string apikey, CancellationToken ct);
}

