// Todas las configuraciones restantes con sintaxis ToTable corregida

// ScheduleConfiguration.cs - Corregida
using Domain.Entities.EVM;
using Domain.Entities.Projects;

namespace Infrastructure.Data.Configurations.Schedule;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Schedules", "Schedule", t =>
        {
            t.HasCheckConstraint("CK_Schedules_ProjectDates",
                "[ProjectFinish] IS NULL OR [ProjectStart] IS NULL OR [ProjectFinish] >= [ProjectStart]");
            t.HasCheckConstraint("CK_Schedules_DataDate",
                "[DataDate] <= [EffectiveDate]");
            t.HasCheckConstraint("CK_Schedules_Approval",
                "([Status] != 'APPROVED' AND [ApprovedBy] IS NULL AND [ApprovedDate] IS NULL) OR " +
                "([Status] = 'APPROVED' AND [ApprovedBy] IS NOT NULL AND [ApprovedDate] IS NOT NULL)");
        });

        // Rest of configuration...
    }
}

// ActivityConfiguration.cs - Corregida
public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Activities", "Schedule", t =>
        {
            t.HasCheckConstraint("CK_Activities_PlannedDates",
                "[PlannedFinishDate] >= [PlannedStartDate]");
            t.HasCheckConstraint("CK_Activities_ActualDates",
                "[ActualFinishDate] IS NULL OR [ActualStartDate] IS NULL OR [ActualFinishDate] >= [ActualStartDate]");
            t.HasCheckConstraint("CK_Activities_PlannedDuration",
                "[PlannedDuration] > 0");
            t.HasCheckConstraint("CK_Activities_RemainingDuration",
                "[RemainingDuration] IS NULL OR [RemainingDuration] >= 0");
            t.HasCheckConstraint("CK_Activities_ActualDuration",
                "[ActualDuration] IS NULL OR [ActualDuration] >= 0");
            t.HasCheckConstraint("CK_Activities_PercentComplete",
                "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
            t.HasCheckConstraint("CK_Activities_PlannedCost",
                "[PlannedCost] IS NULL OR [PlannedCost] >= 0");
            t.HasCheckConstraint("CK_Activities_ActualCost",
                "[ActualCost] >= 0");
        });

        // Rest of configuration...
    }
}



public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
{
    public void Configure(EntityTypeBuilder<Milestone> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Milestones", "Schedule", t =>
        {
            t.HasCheckConstraint("CK_Milestones_CompletedDate",
                "[CompletedDate] IS NULL OR ([IsCompleted] = 1 AND [CompletedDate] IS NOT NULL)");
            t.HasCheckConstraint("CK_Milestones_ActualDate",
                "[ActualDate] IS NULL OR [ActualDate] >= [PlannedDate] - 365");
        });

        // Rest of configuration...
    }
}


public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Equipment", "Progress", t =>
        {
            t.HasCheckConstraint("CK_Equipment_YearOfManufacture",
                "[YearOfManufacture] IS NULL OR ([YearOfManufacture] >= 1900 AND [YearOfManufacture] <= YEAR(GETDATE()))");
            t.HasCheckConstraint("CK_Equipment_PurchaseValue",
                "[PurchaseValue] IS NULL OR [PurchaseValue] >= 0");
            t.HasCheckConstraint("CK_Equipment_DailyRate",
                "[DailyRate] IS NULL OR [DailyRate] >= 0");
            t.HasCheckConstraint("CK_Equipment_HourlyRate",
                "[HourlyRate] IS NULL OR [HourlyRate] >= 0");
            t.HasCheckConstraint("CK_Equipment_MaintenanceHours",
                "[MaintenanceHours] IS NULL OR [MaintenanceHours] >= 0");
            t.HasCheckConstraint("CK_Equipment_MaintenanceDates",
                "[NextMaintenanceDate] IS NULL OR [LastMaintenanceDate] IS NULL OR [NextMaintenanceDate] > [LastMaintenanceDate]");
        });

        // Rest of configuration...
    }
}

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Resources", "Progress", t =>
        {
            t.HasCheckConstraint("CK_Resources_Dates",
                "[EndDate] >= [StartDate]");
            t.HasCheckConstraint("CK_Resources_PlannedQuantity",
                "[PlannedQuantity] > 0");
            t.HasCheckConstraint("CK_Resources_ActualQuantity",
                "[ActualQuantity] >= 0");
            t.HasCheckConstraint("CK_Resources_PlannedRate",
                "[PlannedRate] >= 0");
            t.HasCheckConstraint("CK_Resources_ActualRate",
                "[ActualRate] IS NULL OR [ActualRate] >= 0");
            t.HasCheckConstraint("CK_Resources_PercentComplete",
                "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
        });

        // Rest of configuration...
    }
}


public class EVMRecordConfiguration : IEntityTypeConfiguration<EVMRecord>
{
    public void Configure(EntityTypeBuilder<EVMRecord> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("EVMRecords", "EVM", t =>
        {
            t.HasCheckConstraint("CK_EVMRecords_BAC",
                "[BAC] > 0");
            t.HasCheckConstraint("CK_EVMRecords_PV",
                "[PV] >= 0");
            t.HasCheckConstraint("CK_EVMRecords_EV",
                "[EV] >= 0");
            t.HasCheckConstraint("CK_EVMRecords_AC",
                "[AC] >= 0");
            t.HasCheckConstraint("CK_EVMRecords_EAC",
                "[EAC] > 0");
            t.HasCheckConstraint("CK_EVMRecords_ReportingPeriod",
                "[ReportingPeriod] LIKE '[0-9][0-9][0-9][0-9]-[0-1][0-9]'");
            t.HasCheckConstraint("CK_EVMRecords_EV_Limit",
                "[EV] <= [BAC]");
        });

        // Rest of configuration...
    }
}


public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Notifications", "UI", t =>
        {
            t.HasCheckConstraint("CK_Notifications_ExpiresAt",
                "[ExpiresAt] IS NULL OR [ExpiresAt] > [CreatedAt]");
            t.HasCheckConstraint("CK_Notifications_ReadAt",
                "[ReadAt] IS NULL OR [ReadAt] >= [CreatedAt]");
            t.HasCheckConstraint("CK_Notifications_DismissedAt",
                "[DismissedAt] IS NULL OR [DismissedAt] >= [CreatedAt]");
            t.HasCheckConstraint("CK_Notifications_Color",
                "[Color] IS NULL OR [Color] LIKE '#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]'");
        });

        // Rest of configuration...
    }
}

public class CommitmentConfiguration : IEntityTypeConfiguration<Commitment>
{
    public void Configure(EntityTypeBuilder<Commitment> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Commitments", "Cost", t =>
        {
            t.HasCheckConstraint("CK_Commitments_ContractValue",
                "[ContractValue] > 0");
            t.HasCheckConstraint("CK_Commitments_CommittedValue",
                "[CommittedValue] > 0");
            t.HasCheckConstraint("CK_Commitments_ActualCost",
                "[ActualCost] >= 0");
            t.HasCheckConstraint("CK_Commitments_RetentionPercentage",
                "[RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100");
            t.HasCheckConstraint("CK_Commitments_PercentComplete",
                "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
            t.HasCheckConstraint("CK_Commitments_Dates",
                "[CompletionDate] >= [StartDate] AND [StartDate] >= [SignedDate]");
            t.HasCheckConstraint("CK_Commitments_ActualCompletionDate",
                "[ActualCompletionDate] IS NULL OR [ActualCompletionDate] >= [StartDate]");
        });

        // Rest of configuration...
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("Invoices", "Cost", t =>
        {
            t.HasCheckConstraint("CK_Invoices_GrossAmount",
                "[GrossAmount] > 0");
            t.HasCheckConstraint("CK_Invoices_TaxAmount",
                "[TaxAmount] >= 0");
            t.HasCheckConstraint("CK_Invoices_RetentionAmount",
                "[RetentionAmount] >= 0");
            t.HasCheckConstraint("CK_Invoices_PaidAmount",
                "[PaidAmount] >= 0 AND [PaidAmount] <= [NetAmount]");
            t.HasCheckConstraint("CK_Invoices_DueDate",
                "[DueDate] >= [InvoiceDate]");
            t.HasCheckConstraint("CK_Invoices_PaymentDate",
                "[PaymentDate] IS NULL OR [PaymentDate] >= [InvoiceDate]");
        });

        // Rest of configuration...
    }
}


public class BudgetItemConfiguration : IEntityTypeConfiguration<BudgetItem>
{
    public void Configure(EntityTypeBuilder<BudgetItem> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("BudgetItems", "Cost", t =>
        {
            t.HasCheckConstraint("CK_BudgetItems_Quantity",
                "[Quantity] > 0");
            t.HasCheckConstraint("CK_BudgetItems_UnitRate",
                "[UnitRate] >= 0");
            t.HasCheckConstraint("CK_BudgetItems_LineNumber",
                "[LineNumber] > 0");
        });

        // Rest of configuration...
    }
}

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("InvoiceItems", "Cost", t =>
        {
            t.HasCheckConstraint("CK_InvoiceItems_Quantity",
                "[Quantity] > 0");
            t.HasCheckConstraint("CK_InvoiceItems_UnitRate",
                "[UnitRate] >= 0");
            t.HasCheckConstraint("CK_InvoiceItems_LineNumber",
                "[LineNumber] > 0");
        });

        // Rest of configuration...
    }
}

// PlanningPackageConfiguration.cs - Corregida
using Domain.Entities.Cost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Cost;

public class PlanningPackageConfiguration : IEntityTypeConfiguration<PlanningPackage>
{
    public void Configure(EntityTypeBuilder<PlanningPackage> builder)
    {
        // Table name and schema with check constraints
        builder.ToTable("PlanningPackages", "Cost", t =>
        {
            t.HasCheckConstraint("CK_PlanningPackages_PlannedDates",
                "[PlannedEndDate] > [PlannedStartDate]");
            t.HasCheckConstraint("CK_PlanningPackages_Budget",
                "[Budget] > 0");
            t.HasCheckConstraint("CK_PlanningPackages_Conversion",
                "([IsConverted] = 0 AND [ConversionDate] IS NULL AND [ConvertedToWorkPackageId] IS NULL) OR " +
                "([IsConverted] = 1 AND [ConversionDate] IS NOT NULL)");
            t.HasCheckConstraint("CK_PlanningPackages_Code_Format",
                "[Code] LIKE 'PP-%'");
        });

        // Rest of configuration...
    }
}