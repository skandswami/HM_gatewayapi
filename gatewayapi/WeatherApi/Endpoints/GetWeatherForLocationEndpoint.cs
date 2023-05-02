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
            AllowAnonymous();
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
                await SendAsync(new
                {
                    Message = "Location information not provided."
                },400);
                return;
            }
            try
            {
                var response = await _webApi.GetWeatherForLocationAsync(req.Lat, req.Long, apiKey!, ct);
                
                await SendAsync(response);

            }
            catch (ApiException e)
            {
                if(e.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // copying the bad request content from the internal api
                    await SendAsync(e.Content!, statusCode: 400);
                }
                else
                {
                    // for any other errors, it should be an internal error hence sending 503 Sever Unavailable
                    await SendErrorsAsync(statusCode: 503);
                }
            }
            catch
            {
                // Any internal exception other than from the external api resource
                await SendErrorsAsync(statusCode: 500);
            }
        }
    }
}