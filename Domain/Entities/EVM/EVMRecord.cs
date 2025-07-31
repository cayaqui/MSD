using System;
using Domain.Common;
using Core.Enums.Cost;

namespace Domain.Entities.EVM;

/// <summary>
/// Earned Value Management (EVM) Record for tracking project performance
/// </summary>
public class EVMRecord : BaseEntity, IAuditable
{
    // Basic Information
    public Guid ControlAccountId { get; private set; }
    public DateTime DataDate { get; private set; }
    public EVMPeriodType PeriodType { get; private set; }
    public int PeriodNumber { get; private set; } // Week/Month number
    public int Year { get; private set; }

    // Core EVM Values
    public decimal PV { get; private set; } // Planned Value (BCWS)
    public decimal EV { get; private set; } // Earned Value (BCWP)
    public decimal AC { get; private set; } // Actual Cost (ACWP)
    public decimal BAC { get; private set; } // Budget at Completion

    // Cumulative Values
    public decimal CumulativePV { get; private set; }
    public decimal CumulativeEV { get; private set; }
    public decimal CumulativeAC { get; private set; }

    // Variances
    public decimal CV => EV - AC; // Cost Variance
    public decimal SV => EV - PV; // Schedule Variance
    public decimal CumulativeCV => CumulativeEV - CumulativeAC;
    public decimal CumulativeSV => CumulativeEV - CumulativePV;

    // Performance Indices
    public decimal CPI => AC > 0 ? EV / AC : 0; // Cost Performance Index
    public decimal SPI => PV > 0 ? EV / PV : 0; // Schedule Performance Index
    public decimal CumulativeCPI => CumulativeAC > 0 ? CumulativeEV / CumulativeAC : 0;
    public decimal CumulativeSPI => CumulativePV > 0 ? CumulativeEV / CumulativePV : 0;

    // Forecasting
    public decimal EAC { get; private set; } // Estimate at Completion
    public decimal ETC => EAC - AC; // Estimate to Complete
    public decimal VAC => BAC - EAC; // Variance at Completion
    public decimal TCPI => (BAC - CumulativeEV) / (BAC - CumulativeAC); // To-Complete Performance Index

    // Additional Metrics
    public decimal  PercentComplete { get; private set; }
    public decimal PercentSpent => BAC > 0 ? CumulativeAC / BAC * 100 : 0;
    public DateTime? EstimatedCompletionDate { get; private set; }

    // Status
    public EVMStatus Status { get; private set; }
    public string? Comments { get; private set; }

    // Navigation Properties
    public ControlAccount ControlAccount { get; private set; } = null!;

    // Constructor for EF Core
    private EVMRecord() { }

    public EVMRecord(
        Guid controlAccountId,
        DateTime dataDate,
        EVMPeriodType periodType,
        decimal pv,
        decimal ev,
        decimal ac,
        decimal bac,
        decimal cumulativePV,
        decimal cumulativeEV,
        decimal cumulativeAC)
    {
        ControlAccountId = controlAccountId;
        DataDate = dataDate;
        PeriodType = periodType;
        PV = pv;
        EV = ev;
        AC = ac;
        BAC = bac;
        CumulativePV = cumulativePV;
        CumulativeEV = cumulativeEV;
        CumulativeAC = cumulativeAC;

        Year = dataDate.Year;
        PeriodNumber = CalculatePeriodNumber(dataDate, periodType);

        CalculateEAC();
        DetermineStatus();
        CalculatePercentComplete();
        EstimateCompletionDate();

        CreatedAt = DateTime.UtcNow;

        Validate();
    }

    // Methods
    public void UpdateActuals(decimal ev, decimal ac)
    {
        EV = ev;
        AC = ac;

        CalculateEAC();
        DetermineStatus();
        CalculatePercentComplete();
        EstimateCompletionDate();

        UpdatedAt = DateTime.UtcNow;

        Validate();
    }

    public void UpdateCumulatives(decimal cumulativePV, decimal cumulativeEV, decimal cumulativeAC)
    {
        CumulativePV = cumulativePV;
        CumulativeEV = cumulativeEV;
        CumulativeAC = cumulativeAC;

        CalculateEAC();
        DetermineStatus();
        EstimateCompletionDate();

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddComments(string comments)
    {
        Comments = comments;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEAC(decimal eac, string? justification)
    {
        EAC = eac;
        Comments = $"Manual EAC update: {justification}. {Comments}";
        UpdatedAt = DateTime.UtcNow;

        if (eac < AC)
            throw new ArgumentException("EAC cannot be less than Actual Cost");
    }

    // Private Methods
    private void CalculateEAC()
    {
        // Default EAC calculation using CPI method
        // EAC = BAC / CPI
        if (CumulativeCPI > 0)
        {
            EAC = BAC / CumulativeCPI;
        }
        else
        {
            EAC = BAC;
        }

        // Alternative calculations can be implemented:
        // EAC = AC + (BAC - EV) - for atypical variances
        // EAC = AC + ((BAC - EV) / (CPI * SPI)) - for both cost and schedule factors
    }

    private void DetermineStatus()
    {
        // Determine overall status based on CPI and SPI
        if (CumulativeCPI >= 0.95m && CumulativeSPI >= 0.95m)
        {
            Status = EVMStatus.OnTrack;
        }
        else if (CumulativeCPI >= 0.90m && CumulativeSPI >= 0.90m)
        {
            Status = EVMStatus.AtRisk;
        }
        else if (CumulativeCPI < 0.90m || CumulativeSPI < 0.90m)
        {
            Status = EVMStatus.OffTrack;
        }
        else
        {
            Status = EVMStatus.Critical;
        }
    }

    private void CalculatePercentComplete()
    {
        if (BAC > 0)
        {
            PercentComplete = CumulativeEV / BAC * 100;
        }
        else
        {
            PercentComplete = 0;
        }
    }

    private void EstimateCompletionDate()
    {
        // Simple estimation based on current progress rate
        if (CumulativeSPI > 0 && PercentComplete > 0)
        {
            var remainingWork = 100 - PercentComplete;
            var daysElapsed = (DataDate - CreatedAt).TotalDays;
            if (daysElapsed > 0)
            {
                var currentRate = PercentComplete / (decimal)daysElapsed;
                var daysToComplete = remainingWork / currentRate;
                EstimatedCompletionDate = DataDate.AddDays((double)daysToComplete);
            }
        }
    }

    private int CalculatePeriodNumber(DateTime date, EVMPeriodType periodType)
    {
        return periodType switch
        {
            EVMPeriodType.Weekly => System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                date,
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday),
            EVMPeriodType.Monthly => date.Month,
            EVMPeriodType.Quarterly => (date.Month - 1) / 3 + 1,
            _ => 1
        };
    }

    private void Validate()
    {
        if (PV < 0 || EV < 0 || AC < 0 || BAC < 0)
            throw new ArgumentException("EVM values cannot be negative");

        if (EV > PV)
            throw new ArgumentException("Earned Value cannot exceed Planned Value");

        if (CumulativeEV > BAC)
            throw new ArgumentException("Cumulative Earned Value cannot exceed Budget at Completion");
    }
}