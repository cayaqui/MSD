using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

public class EVMService : IEVMService
{
    private readonly IApiService _apiService;
    
    public EVMService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    // TODO: Implement EVM service methods
}