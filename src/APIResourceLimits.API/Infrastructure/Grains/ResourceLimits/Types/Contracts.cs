namespace APIResourceLimits.API.Infrastructure.Grains.ResourceLimits.Types;

[GenerateSerializer]
[Alias("RequestSlotRequest")]
public class RequestSlotRequest
{
    [Id(0)]
    public required string ResourceName { get; init; }
}

[GenerateSerializer]
[Alias("RequestSlotResponse")]
public class RequestSlotResponse
{
    [Id(0)]
    public required bool Success { get; init; }
    [Id(1)]
    public string? Message { get; init; }
}

[GenerateSerializer]
[Alias("ReturnSlotRequest")]
public class ReturnSlotRequest
{
    [Id(0)]
    public required string ResourceName { get; init; }
}

[GenerateSerializer]
[Alias("ReturnSlotResponse")]
public class ReturnSlotResponse
{
    [Id(0)]
    public required bool Success { get; init; }
}
