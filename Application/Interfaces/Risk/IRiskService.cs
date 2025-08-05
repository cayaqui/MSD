using Core.DTOs.Common;
using Core.DTOs.Risk.Risk;
using Core.DTOs.Risk.RiskResponse;
using Core.DTOs.Risk.RiskReview;

namespace Application.Interfaces.Risk;

public interface IRiskService : IBaseService<RiskDto, CreateRiskDto, UpdateRiskDto>
{
    Task<IEnumerable<RiskDto>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<RiskDto>> GetActiveRisksByProjectAsync(Guid projectId);
    Task<RiskDto?> GetByCodeAsync(string code);
    Task<IEnumerable<RiskDto>> GetByResponseOwnerAsync(Guid userId);
    Task<IEnumerable<RiskDto>> GetRisksRequiringReviewAsync(DateTime reviewDate);
    
    // Risk Assessment
    Task<RiskDto?> UpdateRiskAssessmentAsync(Guid id, int probability, int impact);
    Task<RiskDto?> UpdateResidualRiskAsync(Guid id, int residualProbability, int residualImpact);
    
    // Risk Response
    Task<RiskResponseDto> CreateResponseAsync(CreateRiskResponseDto dto);
    Task<RiskResponseDto?> UpdateResponseAsync(Guid responseId, UpdateRiskResponseDto dto);
    Task<RiskResponseDto?> UpdateResponseStatusAsync(Guid responseId, string status);
    Task<IEnumerable<RiskResponseDto>> GetResponsesByRiskAsync(Guid riskId);
    
    // Risk Review
    Task<RiskReviewDto> CreateReviewAsync(CreateRiskReviewDto dto);
    Task<IEnumerable<RiskReviewDto>> GetReviewsByRiskAsync(Guid riskId);
    
    // Risk Status
    Task<RiskDto?> UpdateStatusAsync(Guid id, string status);
    Task<RiskDto?> CloseRiskAsync(Guid id, string closureReason);
    
    // Risk Matrix and Reporting
    Task<RiskMatrixDto> GetRiskMatrixAsync(Guid projectId);
    Task<RiskRegisterDto> GetRiskRegisterAsync(Guid projectId);
    Task<RiskDashboardDto> GetRiskDashboardAsync(Guid projectId);
    Task<IEnumerable<RiskTrendDto>> GetRiskTrendsAsync(Guid projectId, DateTime startDate, DateTime endDate);
    
    // Risk Analysis
    Task<RiskExposureDto> CalculateRiskExposureAsync(Guid projectId);
    Task<MonteCarloPredictionDto> RunMonteCarloSimulationAsync(Guid projectId, int iterations = 1000);
    Task<RiskHeatMapDto> GenerateRiskHeatMapAsync(Guid projectId);
}