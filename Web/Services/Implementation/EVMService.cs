using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class EVMService : IEVMService
{
    private readonly IApiService _apiService;
    
    public EVMService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    // TODO: Implement EVM service methods
}