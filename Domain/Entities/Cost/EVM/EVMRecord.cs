using Core.Enums.Cost;
using Domain.Common;
using Domain.Entities.Cost.Control;
using System;

namespace Domain.Entities.Cost.EVM;

/// <summary>
/// Earned Value Management Record
/// Tracks EVM metrics over time for Control Accounts
/// </summary>
public class EVMRecord : BaseEntity, IAuditable
{
    // Foreign Keys
    public Guid ControlAccountId { get; private set; }

    // Period Information
    public DateTime DataDate { get; private set; }
    public EVMPeriodType PeriodType { get; private set; }
    public int PeriodNumber { get; private set; }
    public int Year { get; private set; }

    // Core EVM Values (all in base currency)
    public decimal PV { get; private set; } // Planned Value (BCWS)
    public decimal EV { get; private set; } // Earned Value (BCWP)
    public decimal AC { get; private set; } // Actual Cost (ACWP)
    public decimal BAC { get; private set; } // Budget at Completion

    // Variances
    public decimal CV => EV - AC; // Cost Variance
    public decimal SV => EV - PV; // Schedule Variance
    public decimal VAC => BAC - EAC; // Variance at Completion

    // Performance Indices
    public decimal CPI => AC > 0 ? EV / AC : 0; // Cost Performance Index
    public decimal SPI => PV > 0 ? EV / PV : 0; // Schedule Performance Index

    // Forecasts
    public decimal EAC { get; private set; } // Estimate at Completion
    public decimal ETC { get; private set; } // Estimate to Complete
    public decimal TCPI => BAC - AC > 0 ? (BAC - EV) / (BAC - AC) : 0; // To-Complete Performance Index

    // Additional Metrics
    public decimal? PlannedPercentComplete { get; private set; }
    public decimal? ActualPercentComplete { get; private set; }
    public EVMStatus Status { get; private set; }

    // Analysis
    public string? Comments { get; private set; }
    public bool IsBaseline { get; private set; }
    public bool IsApproved { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? ApprovedBy { get; private set; }

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
        decimal bac)
    {
        ControlAccountId = controlAccountId;
        DataDate = dataDate;
        PeriodType = periodType;
        Year = dataDate.Year;
        PeriodNumber = CalculatePeriodNumber(dataDate, periodType);

        PV = pv;
        EV = ev;
        AC = ac;
        BAC = bac;

        CalculateForecasts();
        DetermineStatus();
    }

    // Domain Methods
    public void UpdateValues(decimal pv, decimal ev, decimal ac, string updatedBy)
    {
        if (IsApproved)
            throw new InvalidOperationException("Cannot update approved EVM records");

        PV = pv;
        EV = ev;
        AC = ac;

        CalculateForecasts();
        DetermineStatus();

        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePercentComplete(decimal planned, decimal actual, string updatedBy)
    {
        PlannedPercentComplete = planned;
        ActualPercentComplete = actual;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddComments(string comments, string updatedBy)
    {
        Comments = comments;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string approvedBy)
    {
        if (IsApproved)
            throw new InvalidOperationException("Record is already approved");

        IsApproved = true;
        ApprovedDate = DateTime.UtcNow;
        ApprovedBy = approvedBy;
        UpdatedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsBaseline(string updatedBy)
    {
        IsBaseline = true;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    // Private Methods
    private void CalculateForecasts()
    {
        // Calculate EAC using different methods
        if (CPI > 0)
        {
            // Method 1: EAC = BAC / CPI (assuming current performance continues)
            EAC = BAC / CPI;

            // Could also use: EAC = AC + (BAC - EV) / CPI
            // Or: EAC = AC + (BAC - EV) / (CPI * SPI) for considering both cost and schedule
        }
        else
        {
            EAC = BAC; // Default to original budget if no performance data
        }

        // Calculate ETC
        ETC = EAC - AC;
    }

    private void DetermineStatus()
    {
        // Determine overall status based on variances and indices
        if (CV < 0 && SV < 0)
            Status = EVMStatus.Critical;
        else if (CV < 0 || SV < 0)
            Status = EVMStatus.AtRisk;
        else if (CPI >= 0.95m && SPI >= 0.95m)
            Status = EVMStatus.OnTrack;
        else
            Status = EVMStatus.AtRisk;
    }

    private static int CalculatePeriodNumber(DateTime date, EVMPeriodType periodType)
    {
        return periodType switch
        {
            EVMPeriodType.Daily => date.DayOfYear,
            EVMPeriodType.Weekly => (date.DayOfYear - 1) / 7 + 1,
            EVMPeriodType.Monthly => date.Month,
            EVMPeriodType.Quarterly => (date.Month - 1) / 3 + 1,
            EVMPeriodType.Biannual => (date.Month - 1) / 6 + 1,
            EVMPeriodType.Yearly => 1,
            _ => date.Month
        };
    }

    // Analysis Methods
    public bool IsCostOverrun() => CV < 0;
    public bool IsBehindSchedule() => SV < 0;
    public decimal CostVariancePercentage() => EV > 0 ? CV / EV * 100 : 0;
    public decimal ScheduleVariancePercentage() => PV > 0 ? SV / PV * 100 : 0;
}