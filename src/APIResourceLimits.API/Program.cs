using APIResourceLimits.API.Endpoints;
using APIResourceLimits.API.Infrastructure;
using APIResourceLimits.API.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IClientIdResourceLimitService, ClientIdResourceLimitService>();
builder.SetupOrleans();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            options.RoutePrefix = string.Empty;
        });
    }
}

app.UseHttpsRedirection();

BootstrapEndpoints(app);

app.Run();

static void BootstrapEndpoints(WebApplication app)
{
    var endpoints = System.Reflection.Assembly.GetExecutingAssembly()
                                              .GetTypes()
                                              .Where(type => typeof(IEndpoint).IsAssignableFrom(type) && !type.IsInterface);
    foreach (var type in endpoints)
    {
        var handler = (IEndpoint)Activator.CreateInstance(type)!;
        handler.Bootstrap(app, "/api");
    }
}
