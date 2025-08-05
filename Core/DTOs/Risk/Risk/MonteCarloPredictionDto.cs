namespace Core.DTOs.Risk.Risk;

public class MonteCarloPredictionDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public DateTime SimulationDate { get; set; }
    public int Iterations { get; set; }
    
    // Cost Predictions
    public MonteCarloResultDto CostPrediction { get; set; } = new();
    
    // Schedule Predictions
    public MonteCarloResultDto SchedulePrediction { get; set; } = new();
    
    // Combined Risk Score
    public MonteCarloResultDto RiskScorePrediction { get; set; } = new();
    
    // Confidence Levels
    public List<ConfidenceLevelDto> CostConfidenceLevels { get; set; } = new();
    public List<ConfidenceLevelDto> ScheduleConfidenceLevels { get; set; } = new();
    
    // Risk Drivers
    public List<RiskDriverDto> TopRiskDrivers { get; set; } = new();
}

public class MonteCarloResultDto
{
    public decimal Mean { get; set; }
    public decimal StandardDeviation { get; set; }
    public decimal Minimum { get; set; }
    public decimal Maximum { get; set; }
    public decimal P10 { get; set; } // 10th percentile
    public decimal P50 { get; set; } // 50th percentile (median)
    public decimal P90 { get; set; } // 90th percentile
    public List<HistogramBin> Distribution { get; set; } = new();
}

public class ConfidenceLevelDto
{
    public int ConfidencePercentage { get; set; }
    public decimal Value { get; set; }
    public string Interpretation { get; set; } = string.Empty;
}

public class RiskDriverDto
{
    public Guid RiskId { get; set; }
    public string RiskCode { get; set; } = string.Empty;
    public string RiskTitle { get; set; } = string.Empty;
    public decimal SensitivityIndex { get; set; }
    public decimal ContributionToVariance { get; set; }
}

public class HistogramBin
{
    public decimal LowerBound { get; set; }
    public decimal UpperBound { get; set; }
    public int Frequency { get; set; }
    public decimal Probability { get; set; }
}