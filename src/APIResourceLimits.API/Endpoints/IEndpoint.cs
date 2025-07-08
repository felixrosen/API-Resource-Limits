namespace APIResourceLimits.API.Endpoints;

public interface IEndpoint
{
    void Bootstrap(WebApplication app, string baseRoute);
    string Route { get; }
}

