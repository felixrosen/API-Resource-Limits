using Orleans.Configuration;

namespace APIResourceLimits.API.Infrastructure;

public static class OrleansExtensions
{
    public static void SetupOrleans(this WebApplicationBuilder builder)
    {
        builder.Host.UseOrleans(siloBuilder =>
        {
            siloBuilder.AddActivityPropagation();

            siloBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<PlacementStrategy, PreferLocalPlacement>();
            });            

            siloBuilder.Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "api-resource-limits-cluster";
                options.ServiceId = "api-resource-limits-cluster";
            }).UseLocalhostClustering();
        });
    }
}
