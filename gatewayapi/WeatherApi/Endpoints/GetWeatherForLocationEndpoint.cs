using System;
using FastEndpoints;
using Refit;
using WeatherApi.Endpoints.Requests;

namespace WeatherApi.Endpoints
{
	public class GetWeatherForLocationEndpoint : Endpoint<WeatherRequest>
	{
        private readonly IGatewayWebApi _webApi;

        public GetWeatherForLocationEndpoint(IGatewayWebApi webApi)
        {
            _webApi = webApi;
        }
        public override void Configure()
        {
            Get("/weather");
        }
        public override async Task HandleAsync(WeatherRequest req, CancellationToken ct)
        {
            // Ideally should come from a vault.
            var apiKey = Environment.GetEnvironmentVariable("WeatherVendorApiKey");
            if (apiKey is null)
            {
                // Sending that the Server is unavailable, indicating the api works, but cannot process the request.
                await SendErrorsAsync(statusCode: 503);
            }
            if (req.Long is null|| req.Lat is null)
            {
                await SendErrorsAsync();
            }
            try
            {
                var response = await _webApi.GetWeatherForLocationAsync(req.Lat, req.Long, apiKey,ct);
                
                await SendAsync(response);

            }
            catch (ApiException e)
            {
                // copying the bad request content from the internal api
                if(e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    await SendAsync(e.Content!, statusCode: 400);
                }
                // for any other errors, it should be an internal error hence sending 503 Sever Unavailable
                else
                {
                    await SendErrorsAsync(statusCode: 503);
                }
            }
            catch (Exception ex)
            {
                await SendErrorsAsync(statusCode: 503);
            }
        }
    }
}