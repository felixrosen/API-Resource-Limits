using System.Globalization;
using APIResourceLimits.API.Infrastructure.Grains.ResourceLimits;
using APIResourceLimits.API.Infrastructure.Grains.ResourceLimits.Types;
using APIResourceLimits.API.Service;
using Microsoft.AspNetCore.Mvc;

namespace APIResourceLimits.API.Endpoints;

public class OpenMeteoEndpoint : IEndpoint
{
    public string Route => "/open-meteo";

    public void Bootstrap(WebApplication app, string baseRoute)
    {
        app.MapGet($"{baseRoute}{Route}", async (double latitude,
                                                 double longitude,
                                                 HttpRequest request,
                                                 IClientIdResourceLimitService clientIdResourceLimitService,
                                                 [FromServices] IClusterClient cluster,
                                                 [FromServices] IHttpClientFactory httpFactory,
                                                 CancellationToken ct) =>
        {
            var clientId = request.Headers["X-Client-Id"].FirstOrDefault();

            if (clientId is not { Length: > 0 })
                return Results.BadRequest("Missing X-Client-Id in headers");

            var grain = cluster.GetGrain<IClientIdResourceLimitGrain>(clientId);
            var requestSlotResponse = await clientIdResourceLimitService.RequestSlot(grain, Route);

            if (requestSlotResponse.Success is false)
                return Results.Problem("Resource limit reached", statusCode: 503);

            var requestUrl = $"https://api.open-meteo.com/v1/forecast?" +
                             $"latitude={"59.29811".ToString(CultureInfo.InvariantCulture)}&" +
                             $"longitude={"18.1620204".ToString(CultureInfo.InvariantCulture)}&" +
                             $"current=temperature_2m";
            
            var responseMessage = await httpFactory.CreateClient().GetAsync(requestUrl, ct);

            var response = await responseMessage.Content.ReadAsStringAsync(ct);

            await Task.Delay(TimeSpan.FromSeconds(10), ct); // Simulate long running

            return Results.Ok(response);
        })
        .WithName("Get-Open-Meteo")
        .WithDisplayName("Get weather from open-meteo");
    }
}
