using Web.Services.Interfaces.Cost;

namespace Web.Services.Implementation.Cost;

public class CostService : ICostService
{
    private readonly IApiService _apiService;
    
    public CostService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    // TODO: Implement cost service methods
}