using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Risk;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Risk.Risk;
using Core.DTOs.Risk.RiskResponse;
using Core.DTOs.Risk.RiskReview;
using Microsoft.Extensions.Logging;

namespace Application.Services.Risk;

public class RiskService : IRiskService
{
    private readonly IMapper _mapper;
    private readonly ILogger<RiskService> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly Dictionary<Guid, RiskDto> _riskCache = new();
    private readonly Dictionary<Guid, RiskResponseDto> _responseCache = new();
    private readonly Dictionary<Guid, RiskReviewDto> _reviewCache = new();

    public RiskService(
        IMapper mapper,
        ILogger<RiskService> logger,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<RiskDto> CreateAsync(CreateRiskDto createDto, string? createdBy = null, CancellationToken cancellationToken = default)
    {
        var risk = new RiskDto
        {
            Id = Guid.NewGuid(),
            Code = $"RSK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8]}",
            Title = createDto.Title,
            Description = createDto.Description,
            ProjectId = createDto.ProjectId,
            Category = createDto.Category,
            Type = createDto.Type,
            Status = "Open",
            Probability = createDto.Probability,
            Impact = createDto.Impact,
            RiskScore = createDto.Probability * createDto.Impact,
            IdentifiedById = createDto.IdentifiedById,
            IdentifiedByName = createDto.IdentifiedByName ?? _currentUserService.Name ?? "System",
            IdentifiedDate = createDto.IdentifiedDate ?? DateTime.UtcNow,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = createdBy ?? _currentUserService.Name ?? "System",
            ModifiedDate = DateTime.UtcNow,
            ModifiedBy = createdBy ?? _currentUserService.Name ?? "System"
        };

        _riskCache[risk.Id] = risk;
        return risk;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _riskCache.Remove(id);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _riskCache.ContainsKey(id);
    }

    public async Task<IEnumerable<RiskDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _riskCache.Values;
    }

    public async Task<PagedResult<RiskDto>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 20, string? orderBy = null, bool descending = false, CancellationToken cancellationToken = default)
    {
        var risks = _riskCache.Values.AsQueryable();
        
        // Apply ordering
        risks = orderBy?.ToLower() switch
        {
            "title" => descending ? risks.OrderByDescending(r => r.Title) : risks.OrderBy(r => r.Title),
            "score" => descending ? risks.OrderByDescending(r => r.RiskScore) : risks.OrderBy(r => r.RiskScore),
            "date" => descending ? risks.OrderByDescending(r => r.IdentifiedDate) : risks.OrderBy(r => r.IdentifiedDate),
            _ => descending ? risks.OrderByDescending(r => r.RiskScore) : risks.OrderBy(r => r.RiskScore)
        };

        var totalCount = risks.Count();
        var items = risks.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<RiskDto>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<RiskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _riskCache.TryGetValue(id, out var risk) ? risk : null;
    }

    public async Task<RiskDto?> UpdateAsync(Guid id, UpdateRiskDto updateDto, string? updatedBy = null, CancellationToken cancellationToken = default)
    {
        if (!_riskCache.TryGetValue(id, out var risk))
            return null;

        risk.Title = updateDto.Title ?? risk.Title;
        risk.Description = updateDto.Description ?? risk.Description;
        risk.Category = updateDto.Category ?? risk.Category;
        risk.Type = updateDto.Type ?? risk.Type;
        if (updateDto.Probability.HasValue)
            risk.Probability = updateDto.Probability.Value;
        if (updateDto.Impact.HasValue)  
            risk.Impact = updateDto.Impact.Value;
        if (updateDto.Probability.HasValue || updateDto.Impact.HasValue)
            risk.RiskScore = risk.Probability * risk.Impact;
        risk.ModifiedDate = DateTime.UtcNow;
        risk.ModifiedBy = updatedBy ?? _currentUserService.Name ?? "System";

        return risk;
    }

    public async Task<IEnumerable<RiskDto>> GetByProjectAsync(Guid projectId)
    {
        return _riskCache.Values.Where(r => r.ProjectId == projectId);
    }

    public async Task<IEnumerable<RiskDto>> GetActiveRisksByProjectAsync(Guid projectId)
    {
        return _riskCache.Values.Where(r => r.ProjectId == projectId && r.Status == "Open");
    }

    public async Task<RiskDto?> GetByCodeAsync(string code)
    {
        return _riskCache.Values.FirstOrDefault(r => r.Code == code);
    }

    public async Task<IEnumerable<RiskDto>> GetByResponseOwnerAsync(Guid userId)
    {
        return _riskCache.Values.Where(r => r.ResponseOwnerId == userId);
    }

    public async Task<IEnumerable<RiskDto>> GetRisksRequiringReviewAsync(DateTime reviewDate)
    {
        return _riskCache.Values.Where(r => r.NextReviewDate <= reviewDate && r.Status == "Open");
    }

    public async Task<RiskDto?> UpdateRiskAssessmentAsync(Guid id, int probability, int impact)
    {
        if (!_riskCache.TryGetValue(id, out var risk))
            return null;

        risk.Probability = probability;
        risk.Impact = impact;
        risk.RiskScore = probability * impact;
        risk.ModifiedDate = DateTime.UtcNow;
        risk.ModifiedBy = _currentUserService.Name ?? "System";

        return risk;
    }

    public async Task<RiskDto?> UpdateResidualRiskAsync(Guid id, int residualProbability, int residualImpact)
    {
        if (!_riskCache.TryGetValue(id, out var risk))
            return null;

        risk.ResidualProbability = residualProbability;
        risk.ResidualImpact = residualImpact;
        risk.ResidualRiskScore = residualProbability * residualImpact;
        risk.ModifiedDate = DateTime.UtcNow;
        risk.ModifiedBy = _currentUserService.Name ?? "System";

        return risk;
    }

    public async Task<RiskResponseDto> CreateResponseAsync(CreateRiskResponseDto dto)
    {
        var response = new RiskResponseDto
        {
            Id = Guid.NewGuid(),
            RiskId = dto.RiskId,
            Strategy = dto.Strategy,
            Description = dto.Description,
            Owner = dto.Owner,
            Status = "Planned",
            PlannedStartDate = dto.PlannedStartDate,
            PlannedEndDate = dto.PlannedEndDate,
            Cost = dto.Cost,
            Effectiveness = dto.Effectiveness,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = _currentUserService.Name ?? "System",
            ModifiedDate = DateTime.UtcNow,
            ModifiedBy = _currentUserService.Name ?? "System"
        };

        _responseCache[response.Id] = response;
        return response;
    }

    public async Task<RiskResponseDto?> UpdateResponseAsync(Guid responseId, UpdateRiskResponseDto dto)
    {
        if (!_responseCache.TryGetValue(responseId, out var response))
            return null;

        response.Description = dto.Description ?? response.Description;
        response.Owner = dto.Owner ?? response.Owner;
        response.Status = dto.Status ?? response.Status;
        response.ActualStartDate = dto.ActualStartDate ?? response.ActualStartDate;
        response.ActualEndDate = dto.ActualEndDate ?? response.ActualEndDate;
        response.ActualCost = dto.ActualCost ?? response.ActualCost;
        response.ModifiedDate = DateTime.UtcNow;
        response.ModifiedBy = _currentUserService.Name ?? "System";

        return response;
    }

    public async Task<RiskResponseDto?> UpdateResponseStatusAsync(Guid responseId, string status)
    {
        if (!_responseCache.TryGetValue(responseId, out var response))
            return null;

        response.Status = status;
        response.ModifiedDate = DateTime.UtcNow;
        response.ModifiedBy = _currentUserService.Name ?? "System";

        return response;
    }

    public async Task<IEnumerable<RiskResponseDto>> GetResponsesByRiskAsync(Guid riskId)
    {
        return _responseCache.Values.Where(r => r.RiskId == riskId);
    }

    public async Task<RiskReviewDto> CreateReviewAsync(CreateRiskReviewDto dto)
    {
        var review = new RiskReviewDto
        {
            Id = Guid.NewGuid(),
            RiskId = dto.RiskId,
            ReviewDate = dto.ReviewDate ?? DateTime.UtcNow,
            ReviewedBy = dto.ReviewedBy,
            ReviewComments = dto.ReviewComments,
            PreviousProbability = dto.PreviousProbability,
            PreviousImpact = dto.PreviousImpact,
            NewProbability = dto.NewProbability,
            NewImpact = dto.NewImpact,
            StatusChange = dto.StatusChange,
            NextReviewDate = dto.NextReviewDate,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = _currentUserService.Name ?? "System"
        };

        _reviewCache[review.Id] = review;
        return review;
    }

    public async Task<IEnumerable<RiskReviewDto>> GetReviewsByRiskAsync(Guid riskId)
    {
        return _reviewCache.Values.Where(r => r.RiskId == riskId);
    }

    public async Task<RiskDto?> UpdateStatusAsync(Guid id, string status)
    {
        if (!_riskCache.TryGetValue(id, out var risk))
            return null;

        risk.Status = status;
        risk.ModifiedDate = DateTime.UtcNow;
        risk.ModifiedBy = _currentUserService.Name ?? "System";

        return risk;
    }

    public async Task<RiskDto?> CloseRiskAsync(Guid id, string closureReason)
    {
        if (!_riskCache.TryGetValue(id, out var risk))
            return null;

        risk.Status = "Closed";
        risk.ClosedDate = DateTime.UtcNow;
        risk.ClosureReason = closureReason;
        risk.ModifiedDate = DateTime.UtcNow;
        risk.ModifiedBy = _currentUserService.Name ?? "System";

        return risk;
    }

    public async Task<RiskMatrixDto> GetRiskMatrixAsync(Guid projectId)
    {
        var risks = _riskCache.Values.Where(r => r.ProjectId == projectId && r.Status == "Open");
        
        var matrix = new RiskMatrixDto
        {
            ProjectId = projectId,
            GeneratedDate = DateTime.UtcNow,
            MatrixCells = new List<RiskMatrixCellDto>()
        };

        // Create a 5x5 matrix
        for (int probability = 1; probability <= 5; probability++)
        {
            for (int impact = 1; impact <= 5; impact++)
            {
                var cellRisks = risks.Where(r => r.Probability == probability && r.Impact == impact);
                matrix.MatrixCells.Add(new RiskMatrixCellDto
                {
                    Probability = probability,
                    Impact = impact,
                    RiskCount = cellRisks.Count(),
                    RiskScore = probability * impact,
                    Risks = cellRisks.Select(r => new RiskSummaryDto
                    {
                        Id = r.Id,
                        Code = r.Code,
                        Title = r.Title,
                        RiskScore = r.RiskScore,
                        RiskLevel = GetRiskLevel(r.RiskScore),
                        CostImpact = r.CostImpact,
                        ScheduleImpact = r.ScheduleImpact,
                        ResponseOwner = r.ResponseOwnerName
                    }).ToList()
                });
            }
        }

        return matrix;
    }

    public async Task<RiskRegisterDto> GetRiskRegisterAsync(Guid projectId)
    {
        var risks = _riskCache.Values.Where(r => r.ProjectId == projectId).ToList();
        
        return new RiskRegisterDto
        {
            ProjectId = projectId,
            GeneratedDate = DateTime.UtcNow,
            TotalRisks = risks.Count,
            OpenRisks = risks.Count(r => r.Status == "Open"),
            ClosedRisks = risks.Count(r => r.Status == "Closed"),
            HighRisks = risks.Count(r => r.RiskScore >= 15),
            MediumRisks = risks.Count(r => r.RiskScore >= 5 && r.RiskScore < 15),
            LowRisks = risks.Count(r => r.RiskScore < 5),
            Risks = risks
        };
    }

    public async Task<RiskDashboardDto> GetRiskDashboardAsync(Guid projectId)
    {
        var risks = _riskCache.Values.Where(r => r.ProjectId == projectId).ToList();
        var openRisks = risks.Where(r => r.Status == "Open").ToList();
        
        return new RiskDashboardDto
        {
            ProjectId = projectId,
            GeneratedDate = DateTime.UtcNow,
            
            // Summary metrics
            TotalRisks = risks.Count,
            OpenRisks = openRisks.Count,
            ClosedRisks = risks.Count(r => r.Status == "Closed"),
            MitigatedRisks = risks.Count(r => r.Status == "Mitigated"),
            
            // Risk scores
            TotalRiskScore = openRisks.Sum(r => r.RiskScore),
            AverageRiskScore = openRisks.Any() ? openRisks.Average(r => r.RiskScore) : 0,
            TotalResidualRisk = openRisks.Sum(r => r.ResidualRiskScore ?? 0),
            
            // By category
            RisksByCategory = openRisks.GroupBy(r => r.Category)
                .ToDictionary(g => g.Key, g => g.Count()),
            
            // By status
            RisksByStatus = risks.GroupBy(r => r.Status)
                .ToDictionary(g => g.Key, g => g.Count()),
            
            // Top risks
            TopRisksByScore = openRisks.OrderByDescending(r => r.RiskScore)
                .Take(10)
                .Select(r => new RiskSummaryDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Title = r.Title,
                    RiskScore = r.RiskScore,
                    RiskLevel = GetRiskLevel(r.RiskScore),
                    CostImpact = r.CostImpact,
                    ScheduleImpact = r.ScheduleImpact,
                    ResponseOwner = r.ResponseOwnerName
                })
                .ToList(),
            
            TopRisksByCost = openRisks.Where(r => r.CostImpact.HasValue)
                .OrderByDescending(r => r.CostImpact)
                .Take(10)
                .Select(r => new RiskSummaryDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Title = r.Title,
                    RiskScore = r.RiskScore,
                    RiskLevel = GetRiskLevel(r.RiskScore),
                    CostImpact = r.CostImpact,
                    ScheduleImpact = r.ScheduleImpact,
                    ResponseOwner = r.ResponseOwnerName
                })
                .ToList(),
            
            // Other dashboard properties need to be calculated
            TotalRiskScore = openRisks.Sum(r => r.RiskScore),
            AverageRiskScore = openRisks.Any() ? openRisks.Average(r => r.RiskScore) : 0,
            TotalResidualRisk = openRisks.Sum(r => r.ResidualRiskScore ?? 0),
            ActiveRisks = openRisks.Count,
            MitigatedRisks = risks.Count(r => r.Status == "Mitigated"),
            ResponsesPlanned = 0, // Would calculate from responses
            ResponsesInProgress = 0,
            ResponsesCompleted = 0
        };
    }

    public async Task<IEnumerable<RiskTrendDto>> GetRiskTrendsAsync(Guid projectId, DateTime startDate, DateTime endDate)
    {
        var trends = new List<RiskTrendDto>();
        var currentDate = startDate;
        
        while (currentDate <= endDate)
        {
            var risksAsOfDate = _riskCache.Values
                .Where(r => r.ProjectId == projectId && r.IdentifiedDate <= currentDate)
                .ToList();
            
            trends.Add(new RiskTrendDto
            {
                Date = currentDate,
                TotalRisks = risksAsOfDate.Count,
                OpenRisks = risksAsOfDate.Count(r => r.Status == "Open"),
                ClosedRisks = risksAsOfDate.Count(r => r.Status == "Closed" && r.ClosedDate <= currentDate),
                TotalRiskScore = risksAsOfDate.Where(r => r.Status == "Open").Sum(r => r.RiskScore),
                NewRisks = risksAsOfDate.Count(r => r.IdentifiedDate.Date == currentDate.Date)
            });
            
            currentDate = currentDate.AddDays(7); // Weekly trend
        }
        
        return trends;
    }

    public async Task<RiskExposureDto> CalculateRiskExposureAsync(Guid projectId)
    {
        var risks = _riskCache.Values.Where(r => r.ProjectId == projectId && r.Status == "Open").ToList();
        
        return new RiskExposureDto
        {
            ProjectId = projectId,
            CalculatedDate = DateTime.UtcNow,
            TotalExposure = risks.Sum(r => (r.Probability / 5.0m) * (r.CostImpact ?? 0)),
            ResidualExposure = risks.Sum(r => ((r.ResidualProbability ?? r.Probability) / 5.0m) * (r.CostImpact ?? 0)),
            MitigatedExposure = risks.Sum(r => r.CostImpact ?? 0) - risks.Sum(r => ((r.ResidualProbability ?? r.Probability) / 5.0m) * (r.CostImpact ?? 0)),
            ExposureByCategory = risks.GroupBy(r => r.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(r => (r.Probability / 5.0m) * (r.CostImpact ?? 0))
                ),
            TopExposures = risks.OrderByDescending(r => (r.Probability / 5.0m) * (r.CostImpact ?? 0))
                .Take(10)
                .Select(r => new ExposureDetailDto
                {
                    RiskId = r.Id,
                    RiskCode = r.Code,
                    RiskTitle = r.Title,
                    Exposure = (r.Probability / 5.0m) * (r.CostImpact ?? 0),
                    Probability = r.Probability,
                    CostImpact = r.CostImpact ?? 0
                })
                .ToList()
        };
    }

    public async Task<MonteCarloPredictionDto> RunMonteCarloSimulationAsync(Guid projectId, int iterations = 1000)
    {
        var risks = _riskCache.Values.Where(r => r.ProjectId == projectId && r.Status == "Open").ToList();
        var random = new Random();
        var results = new List<decimal>();
        
        for (int i = 0; i < iterations; i++)
        {
            decimal iterationCost = 0;
            foreach (var risk in risks)
            {
                // Simulate whether risk occurs based on probability
                if (random.NextDouble() < (risk.Probability / 5.0))
                {
                    iterationCost += risk.CostImpact ?? 0;
                }
            }
            results.Add(iterationCost);
        }
        
        results.Sort();
        
        return new MonteCarloPredictionDto
        {
            ProjectId = projectId,
            SimulationDate = DateTime.UtcNow,
            Iterations = iterations,
            MinimumImpact = results.Min(),
            MaximumImpact = results.Max(),
            AverageImpact = results.Average(),
            P50Impact = results[iterations / 2],
            P80Impact = results[(int)(iterations * 0.8)],
            P90Impact = results[(int)(iterations * 0.9)],
            StandardDeviation = CalculateStandardDeviation(results),
            ConfidenceLevel = 0.95m,
            ResultDistribution = CreateDistribution(results)
        };
    }

    public async Task<RiskHeatMapDto> GenerateRiskHeatMapAsync(Guid projectId)
    {
        var risks = _riskCache.Values.Where(r => r.ProjectId == projectId && r.Status == "Open").ToList();
        
        return new RiskHeatMapDto
        {
            ProjectId = projectId,
            GeneratedDate = DateTime.UtcNow,
            Cells = new List<HeatMapCell>(),
            Legend = new HeatMapLegend
            {
                ProbabilityLevels = Enumerable.Range(1, 5).Select(i => new LegendItem
                {
                    Value = i,
                    Label = $"P{i}",
                    Description = GetProbabilityDescription(i)
                }).ToList(),
                ImpactLevels = Enumerable.Range(1, 5).Select(i => new LegendItem
                {
                    Value = i,
                    Label = $"I{i}",
                    Description = GetImpactDescription(i)
                }).ToList()
            },
            Statistics = new HeatMapStatistics
            {
                TotalRisks = risks.Count,
                AverageRiskScore = risks.Any() ? risks.Average(r => r.RiskScore) : 0
            }
        };
        
        // Create cells for each probability/impact combination
        for (int probability = 1; probability <= 5; probability++)
        {
            for (int impact = 1; impact <= 5; impact++)
            {
                var cellRisks = risks.Where(r => r.Probability == probability && r.Impact == impact).ToList();
                var riskScore = probability * impact;
                
                heatMap.Cells.Add(new HeatMapCell
                {
                    Probability = probability,
                    Impact = impact,
                    RiskCount = cellRisks.Count,
                    TotalExposure = cellRisks.Sum(r => r.CostImpact ?? 0),
                    Color = GetCellColor(riskScore),
                    Label = GetRiskLevel(riskScore),
                    Risks = cellRisks.Select(r => new HeatMapRiskItem
                    {
                        Id = r.Id,
                        Code = r.Code,
                        Title = r.Title,
                        Type = r.Type,
                        CostImpact = r.CostImpact,
                        ScheduleImpact = r.ScheduleImpact
                    }).ToList()
                });
            }
        }
        
        return heatMap;
    }

    // Helper methods
    private string GetRiskLevel(int riskScore)
    {
        if (riskScore >= 20) return "Critical";
        if (riskScore >= 15) return "High";
        if (riskScore >= 5) return "Medium";
        return "Low";
    }
    
    private string GetRiskZone(int riskScore)
    {
        if (riskScore >= 15) return "Red";
        if (riskScore >= 5) return "Yellow";
        return "Green";
    }

    private decimal CalculateStandardDeviation(List<decimal> values)
    {
        if (values.Count == 0) return 0;
        
        var avg = values.Average();
        var sum = values.Sum(d => Math.Pow((double)(d - avg), 2));
        return (decimal)Math.Sqrt(sum / values.Count);
    }

    private Dictionary<string, int> CreateDistribution(List<decimal> values)
    {
        if (!values.Any()) return new Dictionary<string, int>();
        
        var min = values.Min();
        var max = values.Max();
        var range = max - min;
        var bucketSize = range / 10; // 10 buckets
        
        var distribution = new Dictionary<string, int>();
        
        for (int i = 0; i < 10; i++)
        {
            var bucketMin = min + (i * bucketSize);
            var bucketMax = min + ((i + 1) * bucketSize);
            var bucketLabel = $"{bucketMin:C0}-{bucketMax:C0}";
            var count = values.Count(v => v >= bucketMin && v < bucketMax);
            distribution[bucketLabel] = count;
        }
        
        return distribution;
    }
    
    private string GetCellColor(int riskScore)
    {
        if (riskScore >= 20) return "#8B0000"; // Dark Red
        if (riskScore >= 15) return "#FF0000"; // Red
        if (riskScore >= 10) return "#FFA500"; // Orange
        if (riskScore >= 5) return "#FFFF00";  // Yellow
        return "#00FF00"; // Green
    }
    
    private string GetProbabilityDescription(int level)
    {
        return level switch
        {
            1 => "Very Low (< 10%)",
            2 => "Low (10-30%)",
            3 => "Medium (30-50%)",
            4 => "High (50-70%)",
            5 => "Very High (> 70%)",
            _ => "Unknown"
        };
    }
    
    private string GetImpactDescription(int level)
    {
        return level switch
        {
            1 => "Negligible",
            2 => "Minor",
            3 => "Moderate",
            4 => "Major",
            5 => "Severe",
            _ => "Unknown"
        };
    }
}