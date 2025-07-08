using APIResourceLimits.API.Infrastructure.Grains.ResourceLimits;
using APIResourceLimits.API.Infrastructure.Grains.ResourceLimits.Types;

namespace APIResourceLimits.API.Service;

public interface IClientIdResourceLimitService
{
    ValueTask DisposeAsync();
    Task<RequestSlotResponse> RequestSlot(IClientIdResourceLimitGrain grain, string resourceName);
}

public class ClientIdResourceLimitService : IAsyncDisposable, IClientIdResourceLimitService
{
    private IClientIdResourceLimitGrain? _grain;
    private string? _resourceName;

    public async Task<RequestSlotResponse> RequestSlot(IClientIdResourceLimitGrain grain, string resourceName)
    {
        _grain = grain;
        _resourceName = resourceName;

        return await _grain.RequestSlot(new RequestSlotRequest { ResourceName = resourceName });
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_grain is null || _resourceName is null)
            return;

        _ = await _grain.ReturnSlot(new ReturnSlotRequest { ResourceName = _resourceName });
    }
}
