using System;
using FastEndpoints;
using WeatherApi.Endpoints.Requests;

namespace WeatherApi.Endpoints
{
	public class GetWeatherForLocationEndpoint : Endpoint<WeatherRequest>
	{
        public override Task HandleAsync(WeatherRequest req, CancellationToken ct)
        {
            return base.HandleAsync(req, ct);
        }
    }
}

