using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class CostService : ICostService
{
    private readonly IApiService _apiService;
    
    public CostService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    // TODO: Implement cost service methods
}