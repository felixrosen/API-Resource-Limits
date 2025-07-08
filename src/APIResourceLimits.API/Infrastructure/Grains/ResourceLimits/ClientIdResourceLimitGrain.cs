using APIResourceLimits.API.Infrastructure.Grains.ResourceLimits.Types;

namespace APIResourceLimits.API.Infrastructure.Grains.ResourceLimits;

[Alias("IResourceLimitGrain")]
public interface IClientIdResourceLimitGrain : IGrainWithStringKey
{
    [Alias("RequestSlot")]
    Task<RequestSlotResponse> RequestSlot(RequestSlotRequest request);
    [Alias("ReturnSlot")]
    Task<ReturnSlotResponse> ReturnSlot(ReturnSlotRequest response);
}


public class ClientIdResourceLimitGrain : Grain, IClientIdResourceLimitGrain
{
    private readonly ILogger<ClientIdResourceLimitGrain> _logger;
    private readonly string _clientId;
    private readonly Dictionary<string, int> _resourceLimits;

    private const int _maxResourceLimit = 4;

    public ClientIdResourceLimitGrain(ILogger<ClientIdResourceLimitGrain> logger)
    {
        _logger = logger;
        _clientId = this.GetPrimaryKeyString();
        _resourceLimits = [];
    }

    public override Task OnActivateAsync(CancellationToken ct)
    {
        _logger.LogInformation("Activated for clientId: {ClientId}", _clientId);
        return base.OnActivateAsync(ct);
    }

    public Task<RequestSlotResponse> RequestSlot(RequestSlotRequest request)
    {
        if (_resourceLimits.TryGetValue(request.ResourceName, out var value))
        {
            if (value >= _maxResourceLimit)
            {
                _logger.LogInformation("{ClientId} {Resource} NO SLOTS LEFT", _clientId, request.ResourceName);
                return Task.FromResult(new RequestSlotResponse { Success = false, Message = "No slots left" });
            }

            _resourceLimits[request.ResourceName] = ++value;

            _logger.LogInformation("{ClientId} {Resource} slots left {Slots}", _clientId, request.ResourceName, _maxResourceLimit - value);

            return Task.FromResult(new RequestSlotResponse { Success = true, });
        }
        
        _resourceLimits[request.ResourceName] = 1;

        _logger.LogInformation("{ClientId} {Resource} slots left {Slots}", _clientId, request.ResourceName, _maxResourceLimit - 1);

        return Task.FromResult(new RequestSlotResponse { Success = true, });
    }

    public Task<ReturnSlotResponse> ReturnSlot(ReturnSlotRequest request)
    {
        if (_resourceLimits.TryGetValue(request.ResourceName, out var value))
        {
            if (value <= 0)
                return Task.FromResult(new ReturnSlotResponse { Success = true, });

            _resourceLimits[request.ResourceName] = --value;

            _logger.LogInformation("{ClientId} {Resource} slots left {Slots}", _clientId, request.ResourceName, _maxResourceLimit - value);

            return Task.FromResult(new ReturnSlotResponse { Success = true, });
        }

        _logger.LogInformation("{ClientId} {Resource} slots left {Slots}", _clientId, request.ResourceName, _maxResourceLimit);
        return Task.FromResult(new ReturnSlotResponse { Success = true, });
    }
}
