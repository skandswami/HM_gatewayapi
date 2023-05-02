using FastEndpoints;
using FastEndpoints.Swagger;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using WeatherApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

_ = builder.Services.AddFastEndpoints();
_ = builder.Services
    .AddRefitClient<IGatewayWebApi>()
    // Kong Base Url is where the Api's are hosted through kong, its by default http://localhost:8000
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("KongBaseUrl")!));

_ = builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Gateway Api";
        s.Version = "v1";
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();

app.UseFastEndpoints();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    _ = app.UseOpenApi();
    _ = app.UseSwaggerUi3();
}

app.Run();

