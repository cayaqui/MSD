using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cost");

            migrationBuilder.EnsureSchema(
                name: "Projects");

            migrationBuilder.EnsureSchema(
                name: "Configuration");

            migrationBuilder.EnsureSchema(
                name: "Change");

            migrationBuilder.EnsureSchema(
                name: "Contracts");

            migrationBuilder.EnsureSchema(
                name: "Risks");

            migrationBuilder.EnsureSchema(
                name: "Documents");

            migrationBuilder.EnsureSchema(
                name: "Organization");

            migrationBuilder.EnsureSchema(
                name: "UI");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.CreateTable(
                name: "CBSTemplates",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IndustryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CostType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    CodingScheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Numeric"),
                    Delimiter = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "."),
                    IncludesIndirectCosts = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IncludesContingency = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CBSTemplates", x => x.Id);
                    table.CheckConstraint("CK_CBSTemplates_CodingScheme", "[CodingScheme] IN ('Numeric', 'Alphabetic', 'Alphanumeric')");
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Logo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LogoContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    FiscalYearStart = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contractors",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Classification = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContactTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BankRoutingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    IsPrequalified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PrequalificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrequalificationNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PerformanceRating = table.Column<decimal>(type: "decimal(18,2)", precision: 3, scale: 2, nullable: true),
                    HasInsurance = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    InsuranceExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InsuranceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    InsuranceCompany = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Certifications = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SpecialtyAreas = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contractors", x => x.Id);
                    table.CheckConstraint("CK_Contractors_CreditLimit", "[CreditLimit] >= 0");
                    table.CheckConstraint("CK_Contractors_InsuranceAmount", "[InsuranceAmount] >= 0 OR [InsuranceAmount] IS NULL");
                    table.CheckConstraint("CK_Contractors_PerformanceRating", "[PerformanceRating] >= 0 AND [PerformanceRating] <= 5 OR [PerformanceRating] IS NULL");
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SymbolNative = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NumericCode = table.Column<int>(type: "int", nullable: false),
                    DecimalDigits = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    Rounding = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 4, nullable: true),
                    PluralName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DecimalSeparator = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true, defaultValue: "."),
                    ThousandsSeparator = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true, defaultValue: ","),
                    PositivePattern = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NegativePattern = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsBaseCurrency = table.Column<bool>(type: "bit", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 6, nullable: false, defaultValue: 1m),
                    ExchangeRateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExchangeRateSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsEnabledForProjects = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsEnabledForCommitments = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.UniqueConstraint("AK_Currencies_Code", x => x.Code);
                    table.CheckConstraint("CK_Currencies_DecimalDigits", "[DecimalDigits] >= 0 AND [DecimalDigits] <= 4");
                    table.CheckConstraint("CK_Currencies_ExchangeRate", "[ExchangeRate] > 0");
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ColorHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEngineering = table.Column<bool>(type: "bit", nullable: false),
                    IsManagement = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTypes",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RequiresWBS = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresCBS = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresOBS = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresSchedule = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresBudget = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresRiskManagement = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresDocumentControl = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresChangeManagement = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RequiresQualityControl = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiresHSE = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DefaultDurationUnit = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DefaultCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    DefaultProgressMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Physical"),
                    DefaultContingencyPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 10m),
                    DefaultReportingPeriod = table.Column<int>(type: "int", nullable: false, defaultValue: 7),
                    ApprovalLevelsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowStagesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WBSTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CBSTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OBSTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RiskRegisterTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTypes", x => x.Id);
                    table.CheckConstraint("CK_ProjectTypes_DefaultContingencyPercentage", "[DefaultContingencyPercentage] >= 0 AND [DefaultContingencyPercentage] <= 100");
                    table.CheckConstraint("CK_ProjectTypes_DefaultDurationUnit", "[DefaultDurationUnit] >= 1 AND [DefaultDurationUnit] <= 4");
                    table.CheckConstraint("CK_ProjectTypes_DefaultReportingPeriod", "[DefaultReportingPeriod] >= 1");
                });

            migrationBuilder.CreateTable(
                name: "SystemParameters",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "String"),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ValidationRule = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DefaultValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    MinValue = table.Column<int>(type: "int", nullable: true),
                    MaxValue = table.Column<int>(type: "int", nullable: true),
                    AllowedValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemParameters", x => x.Id);
                    table.CheckConstraint("CK_SystemParameters_DataType", "[DataType] IN ('String', 'Number', 'Boolean', 'Date', 'Json')");
                    table.CheckConstraint("CK_SystemParameters_MinMaxValue", "[MinValue] IS NULL OR [MaxValue] IS NULL OR [MinValue] <= [MaxValue]");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntraId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    GivenName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    OfficeLocation = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MobilePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BusinessPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PreferredLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "en"),
                    PhotoUrl = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoginCount = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WBSTemplates",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IndustryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodingScheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Numeric"),
                    Delimiter = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, defaultValue: "."),
                    CodeLength = table.Column<int>(type: "int", nullable: false, defaultValue: 3),
                    AutoGenerateCodes = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UsageCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBSTemplates", x => x.Id);
                    table.CheckConstraint("CK_WBSTemplates_CodeLength", "[CodeLength] >= 1 AND [CodeLength] <= 10");
                    table.CheckConstraint("CK_WBSTemplates_CodingScheme", "[CodingScheme] IN ('Numeric', 'Alphabetic', 'Alphanumeric')");
                });

            migrationBuilder.CreateTable(
                name: "WorkPackageProgress",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProgressPeriod = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PreviousProgress = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    CurrentProgress = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    MeasurementMethod = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    PreviousActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ForecastCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaysDelayed = table.Column<int>(type: "int", nullable: true),
                    EarnedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PlannedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Issues = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Risks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PhotoReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of photo IDs"),
                    DocumentReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of document IDs"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPackageProgress", x => x.Id);
                    table.CheckConstraint("CK_WorkPackageProgress_Costs", "[PreviousActualCost] >= 0 AND [CurrentActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastCost] >= 0");
                    table.CheckConstraint("CK_WorkPackageProgress_DaysDelayed", "[DaysDelayed] IS NULL OR [DaysDelayed] >= 0");
                    table.CheckConstraint("CK_WorkPackageProgress_EVM", "([EarnedValue] IS NULL OR [EarnedValue] >= 0) AND ([PlannedValue] IS NULL OR [PlannedValue] >= 0)");
                    table.CheckConstraint("CK_WorkPackageProgress_Progress", "[CurrentProgress] >= 0 AND [CurrentProgress] <= 100 AND [PreviousProgress] >= 0 AND [PreviousProgress] <= 100 AND [CurrentProgress] >= [PreviousProgress]");
                });

            migrationBuilder.CreateTable(
                name: "CBSTemplateElements",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CBSTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    HierarchyPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CostType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    IsControlAccount = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CBSTemplateElements", x => x.Id);
                    table.CheckConstraint("CK_CBSTemplateElements_Level", "[Level] >= 1");
                    table.CheckConstraint("CK_CBSTemplateElements_UnitRate", "[UnitRate] IS NULL OR [UnitRate] >= 0");
                    table.ForeignKey(
                        name: "FK_CBSTemplateElements_CBSTemplateElements_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Configuration",
                        principalTable: "CBSTemplateElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CBSTemplateElements_CBSTemplates_CBSTemplateId",
                        column: x => x.CBSTemplateId,
                        principalSchema: "Configuration",
                        principalTable: "CBSTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ManagerEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WBSTemplateElements",
                schema: "Configuration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WBSTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    HierarchyPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ElementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Phase"),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DefaultBudgetPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    DefaultDurationDays = table.Column<int>(type: "int", nullable: true),
                    DefaultDiscipline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBSTemplateElements", x => x.Id);
                    table.CheckConstraint("CK_WBSTemplateElements_DefaultBudgetPercentage", "[DefaultBudgetPercentage] IS NULL OR ([DefaultBudgetPercentage] >= 0 AND [DefaultBudgetPercentage] <= 100)");
                    table.CheckConstraint("CK_WBSTemplateElements_DefaultDurationDays", "[DefaultDurationDays] IS NULL OR [DefaultDurationDays] >= 0");
                    table.CheckConstraint("CK_WBSTemplateElements_Level", "[Level] >= 1");
                    table.CheckConstraint("CK_WBSTemplateElements_SequenceNumber", "[SequenceNumber] >= 0");
                    table.ForeignKey(
                        name: "FK_WBSTemplateElements_WBSTemplateElements_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Configuration",
                        principalTable: "WBSTemplateElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WBSTemplateElements_WBSTemplates_WBSTemplateId",
                        column: x => x.WBSTemplateId,
                        principalSchema: "Configuration",
                        principalTable: "WBSTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WBSCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCharter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessCase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Objectives = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deliverables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuccessCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Assumptions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Constraints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ContingencyBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ProjectManagerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProjectManagerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Client = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    PlannedProgress = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EarnedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChangeOrderCount = table.Column<int>(type: "int", nullable: true),
                    LastProgressUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastProgressUpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", nullable: true),
                    ProjectTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.CheckConstraint("CK_Projects_ActualDates", "[ActualEndDate] IS NULL OR [ActualStartDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]");
                    table.CheckConstraint("CK_Projects_ApprovedBudget", "[ApprovedBudget] IS NULL OR [ApprovedBudget] >= 0");
                    table.CheckConstraint("CK_Projects_PlannedDates", "[PlannedEndDate] > [PlannedStartDate]");
                    table.CheckConstraint("CK_Projects_ProgressPercentage", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.CheckConstraint("CK_Projects_TotalBudget", "[TotalBudget] >= 0");
                    table.ForeignKey(
                        name: "FK_Projects_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalSchema: "Organization",
                        principalTable: "Currencies",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Operations_OperationId",
                        column: x => x.OperationId,
                        principalSchema: "Organization",
                        principalTable: "Operations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectTypes_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalSchema: "Configuration",
                        principalTable: "ProjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsBaseline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 6, nullable: false, defaultValue: 1.0m),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ContingencyAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ContingencyPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    ManagementReserve = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ManagementReservePercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ParentBudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastRevisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.CheckConstraint("CK_Budgets_ContingencyPercentage", "[ContingencyPercentage] >= 0 AND [ContingencyPercentage] <= 100");
                    table.CheckConstraint("CK_Budgets_ExchangeRate", "[ExchangeRate] > 0");
                    table.CheckConstraint("CK_Budgets_ManagementReservePercentage", "[ManagementReservePercentage] >= 0 AND [ManagementReservePercentage] <= 100");
                    table.CheckConstraint("CK_Budgets_TotalAmount", "[TotalAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_Budgets_Budgets_ParentBudgetId",
                        column: x => x.ParentBudgetId,
                        principalSchema: "Cost",
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Budgets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CBS",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Category = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    CostType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    IsControlPoint = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsLeafNode = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CostAccountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountingCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EstimateClass = table.Column<int>(type: "int", nullable: true),
                    EstimateAccuracyLow = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    EstimateAccuracyHigh = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    OriginalBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ApprovedChanges = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ForecastCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    AllocationBasis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsTimePhased = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TimePhasedData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CBS", x => x.Id);
                    table.CheckConstraint("CK_CBS_AllocationPercentage", "[AllocationPercentage] IS NULL OR ([AllocationPercentage] >= 0 AND [AllocationPercentage] <= 100)");
                    table.CheckConstraint("CK_CBS_Amounts", "[OriginalBudget] >= 0 AND [ApprovedChanges] >= 0 AND [CommittedCost] >= 0 AND [ActualCost] >= 0 AND [ForecastCost] >= 0");
                    table.CheckConstraint("CK_CBS_EstimateAccuracy", "([EstimateAccuracyLow] IS NULL AND [EstimateAccuracyHigh] IS NULL) OR ([EstimateAccuracyLow] <= 0 AND [EstimateAccuracyHigh] >= 0)");
                    table.CheckConstraint("CK_CBS_Level", "[Level] >= 0");
                    table.CheckConstraint("CK_CBS_SequenceNumber", "[SequenceNumber] > 0");
                    table.ForeignKey(
                        name: "FK_CBS_CBS_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Cost",
                        principalTable: "CBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CBS_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractorReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OriginalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 6, nullable: false, defaultValue: 1.0m),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PercentageComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    AmountInvoiced = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RetentionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RetentionPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    PaymentTerms = table.Column<int>(type: "int", nullable: false),
                    PaymentDays = table.Column<int>(type: "int", nullable: false),
                    PaymentSchedule = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RequiresPerformanceBond = table.Column<bool>(type: "bit", nullable: false),
                    PerformanceBondAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PerformanceBondExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequiresPaymentBond = table.Column<bool>(type: "bit", nullable: false),
                    PaymentBondAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaymentBondExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InsuranceRequirements = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RiskLevel = table.Column<int>(type: "int", nullable: false),
                    ContractDocumentUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LastDocumentUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Exclusions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SpecialConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PenaltyClauses = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TerminationClauses = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.CheckConstraint("CK_Contracts_CurrentValue", "[CurrentValue] >= 0");
                    table.CheckConstraint("CK_Contracts_Dates", "[CurrentEndDate] >= [StartDate]");
                    table.CheckConstraint("CK_Contracts_ExchangeRate", "[ExchangeRate] > 0");
                    table.CheckConstraint("CK_Contracts_PaymentDays", "[PaymentDays] >= 0");
                    table.CheckConstraint("CK_Contracts_PercentageComplete", "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                    table.CheckConstraint("CK_Contracts_RetentionPercentage", "[RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_Contracts_Companies_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EquipmentCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    YearOfManufacture = table.Column<int>(type: "int", nullable: true),
                    IsOwned = table.Column<bool>(type: "bit", nullable: false),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PurchaseValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CurrentLocation = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaintenanceHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.Id);
                    table.CheckConstraint("CK_Equipment_DailyRate", "[DailyRate] IS NULL OR [DailyRate] >= 0");
                    table.CheckConstraint("CK_Equipment_HourlyRate", "[HourlyRate] IS NULL OR [HourlyRate] >= 0");
                    table.CheckConstraint("CK_Equipment_MaintenanceHours", "[MaintenanceHours] IS NULL OR [MaintenanceHours] >= 0");
                    table.CheckConstraint("CK_Equipment_PurchaseValue", "[PurchaseValue] IS NULL OR [PurchaseValue] >= 0");
                    table.CheckConstraint("CK_Equipment_YearOfManufacture", "[YearOfManufacture] IS NULL OR ([YearOfManufacture] >= 1900 AND [YearOfManufacture] <= 2100)");
                    table.ForeignKey(
                        name: "FK_Equipment_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipment_Projects_CurrentProjectId",
                        column: x => x.CurrentProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyFrom = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CurrencyTo = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", precision: 12, scale: 6, nullable: false),
                    UFValue = table.Column<decimal>(type: "decimal(18,2)", precision: 12, scale: 6, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsOfficial = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                    table.CheckConstraint("CK_ExchangeRates_Currencies", "[CurrencyFrom] != [CurrencyTo]");
                    table.CheckConstraint("CK_ExchangeRates_Rate", "[Rate] > 0");
                    table.CheckConstraint("CK_ExchangeRates_UFValue", "[UFValue] IS NULL OR [UFValue] > 0");
                    table.ForeignKey(
                        name: "FK_ExchangeRates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "UI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MetadataJson = table.Column<string>(type: "ntext", nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.CheckConstraint("CK_Notification_ExpiresAt", "[ExpiresAt] IS NULL OR [ExpiresAt] >= [CreatedAt]");
                    table.CheckConstraint("CK_Notification_ReadDate", "[ReadAt] IS NULL OR [ReadAt] >= [CreatedAt]");
                    table.ForeignKey(
                        name: "FK_Notifications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OBSNodes",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NodeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Department"),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    HierarchyPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalFTE = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: true),
                    AvailableFTE = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OBSNodes", x => x.Id);
                    table.CheckConstraint("CK_OBSNodes_AvailableFTE", "[AvailableFTE] IS NULL OR ([AvailableFTE] >= 0 AND ([TotalFTE] IS NULL OR [AvailableFTE] <= [TotalFTE]))");
                    table.CheckConstraint("CK_OBSNodes_FTE", "[TotalFTE] IS NULL OR [TotalFTE] >= 0");
                    table.CheckConstraint("CK_OBSNodes_NodeType", "[NodeType] IN ('Company', 'Division', 'Department', 'Team', 'Role')");
                    table.ForeignKey(
                        name: "FK_OBSNodes_OBSNodes_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Organization",
                        principalTable: "OBSNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OBSNodes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OBSNodes_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phases",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    PhaseType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaselineStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaselineEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WeightFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    KeyDeliverables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequiresGateApproval = table.Column<bool>(type: "bit", nullable: false),
                    GateApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GateApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phases", x => x.Id);
                    table.CheckConstraint("CK_Phases_ActualDates", "[ActualEndDate] IS NULL OR [ActualStartDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]");
                    table.CheckConstraint("CK_Phases_PlannedBudget", "[PlannedBudget] IS NULL OR [PlannedBudget] >= 0");
                    table.CheckConstraint("CK_Phases_PlannedDates", "[PlannedEndDate] > [PlannedStartDate]");
                    table.CheckConstraint("CK_Phases_ProgressPercentage", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_Phases_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTeamMembers",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTeamMembers", x => x.Id);
                    table.CheckConstraint("CK_ProjectTeamMembers_Dates", "[EndDate] IS NULL OR [EndDate] > [StartDate]");
                    table.CheckConstraint("CK_ProjectTeamMembers_Role", "[Role] IN ('PROJECT_MANAGER', 'PROJECT_ENGINEER', 'COST_CONTROLLER', 'PLANNER', 'QA_QC', 'DOCUMENT_CONTROLLER', 'TEAM_MEMBER', 'OBSERVER')");
                    table.ForeignKey(
                        name: "FK_ProjectTeamMembers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTeamMembers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Risks",
                schema: "Risks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Probability = table.Column<int>(type: "int", nullable: false),
                    Impact = table.Column<int>(type: "int", nullable: false),
                    Cause = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Effect = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ScheduleImpact = table.Column<int>(type: "int", nullable: true),
                    QualityImpact = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResponseStrategy = table.Column<int>(type: "int", nullable: true),
                    ResponsePlan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponseOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponseDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ResidualProbability = table.Column<int>(type: "int", nullable: true),
                    ResidualImpact = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IdentifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TriggerIndicators = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Risks", x => x.Id);
                    table.CheckConstraint("CK_Risks_ClosedDate", "[ClosedDate] IS NULL OR [ClosedDate] >= [IdentifiedDate]");
                    table.CheckConstraint("CK_Risks_CostImpact", "[CostImpact] IS NULL OR [CostImpact] >= 0");
                    table.CheckConstraint("CK_Risks_Impact", "[Impact] >= 1 AND [Impact] <= 5");
                    table.CheckConstraint("CK_Risks_Probability", "[Probability] >= 1 AND [Probability] <= 5");
                    table.CheckConstraint("CK_Risks_ResidualImpact", "[ResidualImpact] IS NULL OR ([ResidualImpact] >= 1 AND [ResidualImpact] <= 5)");
                    table.CheckConstraint("CK_Risks_ResidualProbability", "[ResidualProbability] IS NULL OR ([ResidualProbability] >= 1 AND [ResidualProbability] <= 5)");
                    table.CheckConstraint("CK_Risks_ResponseCost", "[ResponseCost] IS NULL OR [ResponseCost] >= 0");
                    table.CheckConstraint("CK_Risks_ScheduleImpact", "[ScheduleImpact] IS NULL OR [ScheduleImpact] >= 0");
                    table.ForeignKey(
                        name: "FK_Risks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Risks_Users_IdentifiedById",
                        column: x => x.IdentifiedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Risks_Users_ResponseOwnerId",
                        column: x => x.ResponseOwnerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleVersions",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsBaseline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalActivities = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CriticalActivities = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalFloat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    DataDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SourceSystem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleVersions", x => x.Id);
                    table.CheckConstraint("CK_ScheduleVersions_CriticalActivities", "[CriticalActivities] >= 0 AND [CriticalActivities] <= [TotalActivities]");
                    table.CheckConstraint("CK_ScheduleVersions_Dates", "[PlannedEndDate] >= [PlannedStartDate]");
                    table.CheckConstraint("CK_ScheduleVersions_TotalActivities", "[TotalActivities] >= 0");
                    table.CheckConstraint("CK_ScheduleVersions_TotalFloat", "[TotalFloat] >= 0");
                    table.ForeignKey(
                        name: "FK_ScheduleVersions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transmittals",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransmittalNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransmittalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromContact = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FromEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FromPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ToCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToContact = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ToEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ToPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResponseRequired = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryMethod = table.Column<int>(type: "int", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeliveryConfirmedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryConfirmedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PreparedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreparedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transmittals", x => x.Id);
                    table.CheckConstraint("CK_Transmittals_ResponseDue", "[ResponseDueDate] IS NULL OR [ResponseDueDate] > [TransmittalDate]");
                    table.ForeignKey(
                        name: "FK_Transmittals_Companies_FromCompanyId",
                        column: x => x.FromCompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transmittals_Companies_ToCompanyId",
                        column: x => x.ToCompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transmittals_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transmittals_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transmittals_Users_PreparedById",
                        column: x => x.PreparedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transmittals_Users_SentById",
                        column: x => x.SentById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProjectPermissions",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PermissionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsGranted = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProjectPermissions", x => x.Id);
                    table.CheckConstraint("CK_UserProjectPermissions_ValidDates", "[ExpiresAt] IS NULL OR [ExpiresAt] > [GrantedAt]");
                    table.ForeignKey(
                        name: "FK_UserProjectPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "Security",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProjectPermissions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProjectPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetRevisions",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PreviousAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NewAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRevisions", x => x.Id);
                    table.CheckConstraint("CK_BudgetRevisions_RevisionNumber", "[RevisionNumber] > 0");
                    table.ForeignKey(
                        name: "FK_BudgetRevisions_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalSchema: "Cost",
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ClaimBasis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ContractualReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FactualBasis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    LegalBasis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClaimedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AssessedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ClaimedTimeExtension = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    AssessedTimeExtension = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ApprovedTimeExtension = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    ClaimantName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RespondentName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsTimeBarred = table.Column<bool>(type: "bit", nullable: false),
                    HasMerit = table.Column<bool>(type: "bit", nullable: false),
                    LiabilityPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    AssessmentComments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Resolution = table.Column<int>(type: "int", nullable: false),
                    ResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionDetails = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SettlementTerms = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    HasNoticeOfClaim = table.Column<bool>(type: "bit", nullable: false),
                    HasDetailedParticulars = table.Column<bool>(type: "bit", nullable: false),
                    HasSupportingDocuments = table.Column<bool>(type: "bit", nullable: false),
                    HasExpertReport = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.CheckConstraint("CK_Claims_Amounts", "[ClaimedAmount] >= 0 AND [AssessedAmount] >= 0 AND [ApprovedAmount] >= 0 AND [PaidAmount] >= 0");
                    table.CheckConstraint("CK_Claims_Dates", "[NotificationDate] >= [EventDate] AND [SubmissionDate] >= [NotificationDate]");
                    table.CheckConstraint("CK_Claims_LiabilityPercentage", "[LiabilityPercentage] >= 0 AND [LiabilityPercentage] <= 100");
                    table.CheckConstraint("CK_Claims_TimeExtensions", "[ClaimedTimeExtension] >= 0 AND [AssessedTimeExtension] >= 0 AND [ApprovedTimeExtension] >= 0");
                    table.ForeignKey(
                        name: "FK_Claims_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Contracts",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContractChangeOrders",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ScopeImpact = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ScheduleImpact = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Estimate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Approve = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Actua = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ScheduleImpactDays = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RevisedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ReviewComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImplementationStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImplementationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PercentageComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    HasSupportingDocuments = table.Column<bool>(type: "bit", nullable: false),
                    Hareakdown = table.Column<bool>(type: "bit", nullable: false),
                    HasScheduleAnalysis = table.Column<bool>(type: "bit", nullable: false),
                    RiskAssessment = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    RiskMitigationPlan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractChangeOrders", x => x.Id);
                    table.CheckConstraint("CK_ContractChangeOrders_Amounts", "[Estimate] >= 0 AND [Approve] >= 0 AND [Actua] >= 0");
                    table.CheckConstraint("CK_ContractChangeOrders_ApprovalDates", "([ApprovalDate] IS NULL AND [RejectionDate] IS NULL) OR ([ApprovalDate] IS NOT NULL AND [RejectionDate] IS NULL) OR ([ApprovalDate] IS NULL AND [RejectionDate] IS NOT NULL)");
                    table.CheckConstraint("CK_ContractChangeOrders_PercentageComplete", "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                    table.ForeignKey(
                        name: "FK_ContractChangeOrders_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Contracts",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractDocuments",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractDocuments", x => x.Id);
                    table.CheckConstraint("CK_ContractDocuments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_ContractDocuments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Contracts",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractMilestones",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MilestoneCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ForecastDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PercentageOfContract = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    CompletionCriteria = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Deliverables = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RequiresClientApproval = table.Column<bool>(type: "bit", nullable: false),
                    IsPaymentMilestone = table.Column<bool>(type: "bit", nullable: false),
                    PercentageComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    ProgressComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LastProgressUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsInvoiced = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VarianceExplanation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractMilestones", x => x.Id);
                    table.CheckConstraint("CK_ContractMilestones_Amounts", "[Amount] >= 0 AND [InvoiceAmount] >= 0 AND [PaymentAmount] >= 0");
                    table.CheckConstraint("CK_ContractMilestones_Dates", "[ActualDate] IS NULL OR [ActualDate] >= [PlannedDate]");
                    table.CheckConstraint("CK_ContractMilestones_PercentageComplete", "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                    table.CheckConstraint("CK_ContractMilestones_PercentageOfContract", "[PercentageOfContract] >= 0 AND [PercentageOfContract] <= 100");
                    table.ForeignKey(
                        name: "FK_ContractMilestones_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Contracts",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Valuations",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValuationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValuationPeriod = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    PeriodStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValuationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreviouslyCompletedWork = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentPeriodWork = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCompletedWork = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PercentageComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    MaterialsOnSite = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaterialsOffSite = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalMaterials = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedVariations = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PendingVariations = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossValuation = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LessRetention = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RetentionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LessPreviousCertificates = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetValuation = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    AdvancePaymentRecovery = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Penalties = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AmountDue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsInvoiced = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ApprovalComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    HasProgressPhotos = table.Column<bool>(type: "bit", nullable: false),
                    HasMeasurementSheets = table.Column<bool>(type: "bit", nullable: false),
                    HasQualityDocuments = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Valuations", x => x.Id);
                    table.CheckConstraint("CK_Valuations_Amounts", "[GrossValuation] >= 0 AND [RetentionAmount] >= 0 AND [NetValuation] >= 0 AND [AmountDue] >= 0");
                    table.CheckConstraint("CK_Valuations_Dates", "[PeriodEndDate] >= [PeriodStartDate]");
                    table.CheckConstraint("CK_Valuations_PercentageComplete", "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                    table.CheckConstraint("CK_Valuations_ValuationPeriod", "[ValuationPeriod] > 0");
                    table.ForeignKey(
                        name: "FK_Valuations_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalSchema: "Contracts",
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OBSNodeMembers",
                schema: "Organization",
                columns: table => new
                {
                    OBSNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OBSNodeMembers", x => new { x.OBSNodeId, x.UserId });
                    table.ForeignKey(
                        name: "FK_OBSNodeMembers_OBSNodes_OBSNodeId",
                        column: x => x.OBSNodeId,
                        principalSchema: "Organization",
                        principalTable: "OBSNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OBSNodeMembers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountCodes",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PhaseCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    PackageCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SpecificCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SIICode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SIIDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresIVA = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EligibleForAcceleratedDepreciation = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CostType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Direct"),
                    CostSubType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ProjectId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCodes", x => x.Id);
                    table.CheckConstraint("CK_AccountCodes_AccountType", "[AccountType] IN ('P', 'C', 'G', 'E')");
                    table.CheckConstraint("CK_AccountCodes_CostType", "[CostType] IN ('Direct', 'Indirect', 'Contingency')");
                    table.ForeignKey(
                        name: "FK_AccountCodes_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCodes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountCodes_Projects_ProjectId1",
                        column: x => x.ProjectId1,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ControlAccounts",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CAMUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BAC = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ContingencyReserve = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ManagementReserve = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    MeasurementMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PercentComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlAccounts", x => x.Id);
                    table.CheckConstraint("CK_ControlAccounts_BAC", "[BAC] >= 0");
                    table.CheckConstraint("CK_ControlAccounts_ContingencyReserve", "[ContingencyReserve] >= 0");
                    table.CheckConstraint("CK_ControlAccounts_ManagementReserve", "[ManagementReserve] >= 0");
                    table.CheckConstraint("CK_ControlAccounts_PercentComplete", "[PercentComplete] >= 0 AND [PercentComplete] <= 100");
                    table.ForeignKey(
                        name: "FK_ControlAccounts_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ControlAccounts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ControlAccounts_Users_CAMUserId",
                        column: x => x.CAMUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MilestoneCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsContractual = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ForecastDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletionPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    CompletionCriteria = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaymentCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    IsPaymentTriggered = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PredecessorMilestones = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SuccessorMilestones = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RequiresApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Deliverables = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                    table.CheckConstraint("CK_Milestones_CompletionPercentage", "[CompletionPercentage] >= 0 AND [CompletionPercentage] <= 100");
                    table.CheckConstraint("CK_Milestones_PaymentAmount", "[PaymentAmount] IS NULL OR [PaymentAmount] > 0");
                    table.ForeignKey(
                        name: "FK_Milestones_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Milestones_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskResponses",
                schema: "Risks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Strategy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Actions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ExpectedProbability = table.Column<int>(type: "int", nullable: false),
                    ExpectedImpact = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EffectivenessScore = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    LessonsLearned = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskResponses", x => x.Id);
                    table.CheckConstraint("CK_RiskResponses_ActualCost", "[ActualCost] IS NULL OR [ActualCost] >= 0");
                    table.CheckConstraint("CK_RiskResponses_CompletedDate", "[CompletedDate] IS NULL OR [CompletedDate] >= [StartDate]");
                    table.CheckConstraint("CK_RiskResponses_EffectivenessScore", "[EffectivenessScore] IS NULL OR ([EffectivenessScore] >= 0 AND [EffectivenessScore] <= 100)");
                    table.CheckConstraint("CK_RiskResponses_EstimatedCost", "[EstimatedCost] >= 0");
                    table.CheckConstraint("CK_RiskResponses_ExpectedImpact", "[ExpectedImpact] >= 1 AND [ExpectedImpact] <= 5");
                    table.CheckConstraint("CK_RiskResponses_ExpectedProbability", "[ExpectedProbability] >= 1 AND [ExpectedProbability] <= 5");
                    table.ForeignKey(
                        name: "FK_RiskResponses_Risks_RiskId",
                        column: x => x.RiskId,
                        principalSchema: "Risks",
                        principalTable: "Risks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskResponses_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RiskReviews",
                schema: "Risks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousProbability = table.Column<int>(type: "int", nullable: false),
                    PreviousImpact = table.Column<int>(type: "int", nullable: false),
                    NewProbability = table.Column<int>(type: "int", nullable: false),
                    NewImpact = table.Column<int>(type: "int", nullable: false),
                    ReviewNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ChangesIdentified = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ActionsRequired = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    StatusChanged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PreviousStatus = table.Column<int>(type: "int", maxLength: 50, nullable: true),
                    NewStatus = table.Column<int>(type: "int", maxLength: 50, nullable: true),
                    ResponseStrategyChanged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UpdatedResponsePlan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskReviews", x => x.Id);
                    table.CheckConstraint("CK_RiskReviews_NewImpact", "[NewImpact] >= 1 AND [NewImpact] <= 5");
                    table.CheckConstraint("CK_RiskReviews_NewProbability", "[NewProbability] >= 1 AND [NewProbability] <= 5");
                    table.CheckConstraint("CK_RiskReviews_NextReviewDate", "[NextReviewDate] IS NULL OR [NextReviewDate] > [ReviewDate]");
                    table.CheckConstraint("CK_RiskReviews_PreviousImpact", "[PreviousImpact] >= 1 AND [PreviousImpact] <= 5");
                    table.CheckConstraint("CK_RiskReviews_PreviousProbability", "[PreviousProbability] >= 1 AND [PreviousProbability] <= 5");
                    table.ForeignKey(
                        name: "FK_RiskReviews_Risks_RiskId",
                        column: x => x.RiskId,
                        principalSchema: "Risks",
                        principalTable: "Risks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RiskReviews_Users_ReviewedById",
                        column: x => x.ReviewedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransmittalAttachments",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransmittalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BlobContainerName = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    BlobStorageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmittalAttachments", x => x.Id);
                    table.CheckConstraint("CK_TransmittalAttachments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_TransmittalAttachments_Transmittals_TransmittalId",
                        column: x => x.TransmittalId,
                        principalSchema: "Documents",
                        principalTable: "Transmittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransmittalRecipients",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransmittalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RequiresAcknowledgment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgmentComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AcknowledgmentToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryError = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmittalRecipients", x => x.Id);
                    table.CheckConstraint("CK_TransmittalRecipients_Recipient", "[UserId] IS NOT NULL OR [Email] IS NOT NULL OR [Name] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_TransmittalRecipients_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmittalRecipients_Transmittals_TransmittalId",
                        column: x => x.TransmittalId,
                        principalSchema: "Documents",
                        principalTable: "Transmittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransmittalRecipients_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimDocuments",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimDocuments", x => x.Id);
                    table.CheckConstraint("CK_ClaimDocuments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_ClaimDocuments_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalSchema: "Contracts",
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimRelations",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimRelations", x => x.Id);
                    table.CheckConstraint("CK_ClaimRelations_NotSelfReferencing", "[ClaimId] <> [RelatedClaimId]");
                    table.ForeignKey(
                        name: "FK_ClaimRelations_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalSchema: "Contracts",
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimRelations_Claims_RelatedClaimId",
                        column: x => x.RelatedClaimId,
                        principalSchema: "Contracts",
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeOrderDocuments",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeOrderDocuments", x => x.Id);
                    table.CheckConstraint("CK_ChangeOrderDocuments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_ChangeOrderDocuments_ContractChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Contracts",
                        principalTable: "ContractChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChangeOrderRelations",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeOrderRelations", x => x.Id);
                    table.CheckConstraint("CK_ChangeOrderRelations_NotSelfReferencing", "[ChangeOrderId] <> [RelatedChangeOrderId]");
                    table.ForeignKey(
                        name: "FK_ChangeOrderRelations_ContractChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Contracts",
                        principalTable: "ContractChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeOrderRelations_ContractChangeOrders_RelatedChangeOrderId",
                        column: x => x.RelatedChangeOrderId,
                        principalSchema: "Contracts",
                        principalTable: "ContractChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClaimChangeOrders",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimChangeOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimChangeOrders_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalSchema: "Contracts",
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimChangeOrders_ContractChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Contracts",
                        principalTable: "ContractChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeOrderMilestones",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MilestoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImpactType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImpactDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeOrderMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeOrderMilestones_ContractChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Contracts",
                        principalTable: "ContractChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeOrderMilestones_ContractMilestones_MilestoneId",
                        column: x => x.MilestoneId,
                        principalSchema: "Contracts",
                        principalTable: "ContractMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MilestoneDependencies",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PredecessorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SuccessorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DependencyType = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false, defaultValue: "FS"),
                    LagDays = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilestoneDependencies", x => x.Id);
                    table.CheckConstraint("CK_MilestoneDependencies_DependencyType", "[DependencyType] IN ('FS', 'SS', 'FF', 'SF')");
                    table.CheckConstraint("CK_MilestoneDependencies_NotSelfReferencing", "[PredecessorId] <> [SuccessorId]");
                    table.ForeignKey(
                        name: "FK_MilestoneDependencies_ContractMilestones_PredecessorId",
                        column: x => x.PredecessorId,
                        principalSchema: "Contracts",
                        principalTable: "ContractMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MilestoneDependencies_ContractMilestones_SuccessorId",
                        column: x => x.SuccessorId,
                        principalSchema: "Contracts",
                        principalTable: "ContractMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MilestoneDocuments",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MilestoneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilestoneDocuments", x => x.Id);
                    table.CheckConstraint("CK_MilestoneDocuments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_MilestoneDocuments_ContractMilestones_MilestoneId",
                        column: x => x.MilestoneId,
                        principalSchema: "Contracts",
                        principalTable: "ContractMilestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValuationDocuments",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValuationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValuationDocuments", x => x.Id);
                    table.CheckConstraint("CK_ValuationDocuments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_ValuationDocuments_Valuations_ValuationId",
                        column: x => x.ValuationId,
                        principalSchema: "Contracts",
                        principalTable: "Valuations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValuationItems",
                schema: "Contracts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValuationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContractQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    ContractAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviousQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    PreviousAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PercentageComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    WorkPackageCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValuationItems", x => x.Id);
                    table.CheckConstraint("CK_ValuationItems_Amounts", "[UnitRate] >= 0 AND [ContractAmount] >= 0 AND [PreviousAmount] >= 0 AND [CurrentAmount] >= 0 AND [TotalAmount] >= 0");
                    table.CheckConstraint("CK_ValuationItems_PercentageComplete", "[PercentageComplete] >= 0 AND [PercentageComplete] <= 100");
                    table.CheckConstraint("CK_ValuationItems_Quantities", "[ContractQuantity] >= 0 AND [PreviousQuantity] >= 0 AND [CurrentQuantity] >= 0 AND [TotalQuantity] >= 0");
                    table.ForeignKey(
                        name: "FK_ValuationItems_Valuations_ValuationId",
                        column: x => x.ValuationId,
                        principalSchema: "Contracts",
                        principalTable: "Valuations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControlAccountAssignments",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlAccountAssignments", x => x.Id);
                    table.CheckConstraint("CK_ControlAccountAssignments_AllocationPercentage", "[AllocationPercentage] IS NULL OR ([AllocationPercentage] >= 0 AND [AllocationPercentage] <= 100)");
                    table.CheckConstraint("CK_ControlAccountAssignments_EndDate", "[EndDate] IS NULL OR [EndDate] >= [AssignedDate]");
                    table.ForeignKey(
                        name: "FK_ControlAccountAssignments_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ControlAccountAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostControlReports",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BudgetedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhysicalProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    EarnedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostVariance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ScheduleVariance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostPerformanceIndex = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 3, nullable: false),
                    EstimateAtCompletion = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExchangeRateUSD = table.Column<decimal>(type: "decimal(18,2)", precision: 12, scale: 6, nullable: true),
                    UFValue = table.Column<decimal>(type: "decimal(18,2)", precision: 12, scale: 6, nullable: true),
                    ImportedMaterialsPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    VarianceAtCompletion = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ToCompletePerformanceIndex = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 3, nullable: true),
                    SchedulePerformanceIndex = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 3, nullable: false),
                    EstimateToComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostControlReports", x => x.Id);
                    table.CheckConstraint("CK_CostControlReports_Amounts", "[BudgetedCost] >= 0 AND [ActualCost] >= 0 AND [EstimateAtCompletion] >= 0 AND [EstimateToComplete] >= 0");
                    table.CheckConstraint("CK_CostControlReports_ImportedMaterials", "[ImportedMaterialsPercentage] IS NULL OR ([ImportedMaterialsPercentage] >= 0 AND [ImportedMaterialsPercentage] <= 100)");
                    table.CheckConstraint("CK_CostControlReports_Indices", "[CostPerformanceIndex] >= 0 AND [SchedulePerformanceIndex] >= 0");
                    table.CheckConstraint("CK_CostControlReports_Progress", "[PhysicalProgressPercentage] >= 0 AND [PhysicalProgressPercentage] <= 100");
                    table.CheckConstraint("CK_CostControlReports_TCPI", "[ToCompletePerformanceIndex] IS NULL OR [ToCompletePerformanceIndex] >= 0");
                    table.ForeignKey(
                        name: "FK_CostControlReports_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostControlReports_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EVMRecords",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodType = table.Column<int>(type: "int", nullable: false),
                    PeriodNumber = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    PV = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EV = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AC = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BAC = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EAC = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ETC = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedPercentComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    ActualPercentComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsBaseline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVMRecords", x => x.Id);
                    table.CheckConstraint("CK_EVMRecords_Percentages", "([PlannedPercentComplete] IS NULL OR ([PlannedPercentComplete] >= 0 AND [PlannedPercentComplete] <= 100)) AND ([ActualPercentComplete] IS NULL OR ([ActualPercentComplete] >= 0 AND [ActualPercentComplete] <= 100))");
                    table.CheckConstraint("CK_EVMRecords_PeriodNumber", "[PeriodNumber] > 0");
                    table.CheckConstraint("CK_EVMRecords_Values", "[PV] >= 0 AND [EV] >= 0 AND [AC] >= 0 AND [BAC] >= 0 AND [EAC] >= 0 AND [ETC] >= 0");
                    table.CheckConstraint("CK_EVMRecords_Year", "[Year] >= 2000 AND [Year] <= 2100");
                    table.ForeignKey(
                        name: "FK_EVMRecords_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanningPackages",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    PlannedConversionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsConverted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ConversionNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ConversionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 99),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningPackages", x => x.Id);
                    table.CheckConstraint("CK_PlanningPackages_ConversionDate", "[PlannedConversionDate] <= [PlannedStartDate]");
                    table.CheckConstraint("CK_PlanningPackages_Dates", "[PlannedEndDate] > [PlannedStartDate]");
                    table.CheckConstraint("CK_PlanningPackages_EstimatedBudget", "[EstimatedBudget] >= 0");
                    table.CheckConstraint("CK_PlanningPackages_EstimatedHours", "[EstimatedHours] >= 0");
                    table.CheckConstraint("CK_PlanningPackages_Priority", "[Priority] >= 1 AND [Priority] <= 99");
                    table.ForeignKey(
                        name: "FK_PlanningPackages_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanningPackages_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanningPackages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimePhasedBudgets",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlannedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CumulativePlannedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedLaborHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false),
                    PlannedLaborCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedMaterialCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedEquipmentCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedSubcontractCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlannedOtherCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsBaseline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevisionNumber = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimePhasedBudgets", x => x.Id);
                    table.CheckConstraint("CK_TimePhasedBudgets_CumulativePlannedValue", "[CumulativePlannedValue] >= 0");
                    table.CheckConstraint("CK_TimePhasedBudgets_Period", "[PeriodEnd] >= [PeriodStart]");
                    table.CheckConstraint("CK_TimePhasedBudgets_PlannedCosts", "[PlannedLaborCost] >= 0 AND [PlannedMaterialCost] >= 0 AND [PlannedEquipmentCost] >= 0 AND [PlannedSubcontractCost] >= 0 AND [PlannedOtherCost] >= 0");
                    table.CheckConstraint("CK_TimePhasedBudgets_PlannedLaborHours", "[PlannedLaborHours] >= 0");
                    table.CheckConstraint("CK_TimePhasedBudgets_PlannedValue", "[PlannedValue] >= 0");
                    table.ForeignKey(
                        name: "FK_TimePhasedBudgets_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WBSElements",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    FullPath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ElementType = table.Column<int>(type: "int", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeliverableDescription = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Assumptions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Constraints = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ExclusionsInclusions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBSElements", x => x.Id);
                    table.CheckConstraint("CK_WBSElements_Level", "[Level] >= 0");
                    table.CheckConstraint("CK_WBSElements_SequenceNumber", "[SequenceNumber] > 0");
                    table.ForeignKey(
                        name: "FK_WBSElements_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WBSElements_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WBSElements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WBSElements_WBSElements_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostItems",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CBSId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CostCenter = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PlannedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ForecastCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", precision: 12, scale: 6, nullable: false, defaultValue: 1.0m),
                    ReferenceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VendorId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostItems", x => x.Id);
                    table.CheckConstraint("CK_CostItems_Costs", "[PlannedCost] >= 0 AND [ActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastCost] >= 0");
                    table.CheckConstraint("CK_CostItems_ExchangeRate", "[ExchangeRate] > 0");
                    table.ForeignKey(
                        name: "FK_CostItems_CBS_CBSId",
                        column: x => x.CBSId,
                        principalSchema: "Cost",
                        principalTable: "CBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostItems_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostItems_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostItems_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WBSCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PackageType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContractType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.CheckConstraint("CK_Packages_ActualDates", "([ActualStartDate] IS NULL AND [ActualEndDate] IS NULL) OR ([ActualStartDate] IS NOT NULL AND ([ActualEndDate] IS NULL OR [ActualEndDate] >= [ActualStartDate]))");
                    table.CheckConstraint("CK_Packages_Assignment", "([PhaseId] IS NULL AND [WBSElementId] IS NOT NULL) OR ([PhaseId] IS NOT NULL AND [WBSElementId] IS NULL) OR ([PhaseId] IS NULL AND [WBSElementId] IS NULL)");
                    table.CheckConstraint("CK_Packages_ContractValue", "[ContractValue] >= 0");
                    table.CheckConstraint("CK_Packages_PlannedDates", "[PlannedEndDate] >= [PlannedStartDate]");
                    table.CheckConstraint("CK_Packages_ProgressPercentage", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_Packages_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RAMAssignments",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OBSNodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponsibilityType = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    AllocatedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: true),
                    AllocatedPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RAMAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RAMAssignments_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RAMAssignments_OBSNodes_OBSNodeId",
                        column: x => x.OBSNodeId,
                        principalSchema: "Organization",
                        principalTable: "OBSNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RAMAssignments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RAMAssignments_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WBSCBSMappings",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CBSId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocationPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AllocationBasis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBSCBSMappings", x => x.Id);
                    table.CheckConstraint("CK_WBSCBSMappings_AllocationPercentage", "[AllocationPercentage] > 0 AND [AllocationPercentage] <= 100");
                    table.CheckConstraint("CK_WBSCBSMappings_EndDate", "[EndDate] IS NULL OR [EndDate] >= [EffectiveDate]");
                    table.ForeignKey(
                        name: "FK_WBSCBSMappings_CBS_CBSId",
                        column: x => x.CBSId,
                        principalSchema: "Cost",
                        principalTable: "CBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WBSCBSMappings_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WBSElementProgress",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgressDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Week = table.Column<int>(type: "int", nullable: false),
                    PreviousProgress = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    CurrentProgress = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    MeasurementMethod = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    PhysicalProgress = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    PhysicalProgressDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PreviousActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CurrentActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ForecastToComplete = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ForecastEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DaysDelayed = table.Column<int>(type: "int", nullable: true),
                    DelayReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EarnedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PlannedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Issues = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Risks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MitigationActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequiresReview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReviewReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReportedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhotoReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of photo IDs"),
                    DocumentReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of document IDs"),
                    MilestoneReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of milestone codes achieved"),
                    LaborHoursUsed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EquipmentHoursUsed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaterialQuantityUsed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    ResourceNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChildrenCount = table.Column<int>(type: "int", nullable: true),
                    CompletedChildrenCount = table.Column<int>(type: "int", nullable: true),
                    IsRollupProgress = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBSElementProgress", x => x.Id);
                    table.CheckConstraint("CK_WBSElementProgress_Children", "([CompletedChildrenCount] IS NULL OR [ChildrenCount] IS NULL OR [CompletedChildrenCount] <= [ChildrenCount])");
                    table.CheckConstraint("CK_WBSElementProgress_Costs", "[PreviousActualCost] >= 0 AND [CurrentActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastToComplete] >= 0");
                    table.CheckConstraint("CK_WBSElementProgress_EVM", "([EarnedValue] IS NULL OR [EarnedValue] >= 0) AND ([PlannedValue] IS NULL OR [PlannedValue] >= 0)");
                    table.CheckConstraint("CK_WBSElementProgress_PhysicalProgress", "[PhysicalProgress] IS NULL OR ([PhysicalProgress] >= 0 AND [PhysicalProgress] <= 100)");
                    table.CheckConstraint("CK_WBSElementProgress_Progress", "[CurrentProgress] >= 0 AND [CurrentProgress] <= 100 AND [PreviousProgress] >= 0 AND [PreviousProgress] <= 100");
                    table.CheckConstraint("CK_WBSElementProgress_Resources", "([LaborHoursUsed] IS NULL OR [LaborHoursUsed] >= 0) AND ([EquipmentHoursUsed] IS NULL OR [EquipmentHoursUsed] >= 0) AND ([MaterialQuantityUsed] IS NULL OR [MaterialQuantityUsed] >= 0)");
                    table.ForeignKey(
                        name: "FK_WBSElementProgress_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkPackageDetails",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaselineStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaselineEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ForecastStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ForecastEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedDuration = table.Column<int>(type: "int", nullable: false),
                    ActualDuration = table.Column<int>(type: "int", nullable: true),
                    RemainingDuration = table.Column<int>(type: "int", nullable: true),
                    TotalFloat = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false),
                    FreeFloat = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false),
                    IsCriticalPath = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CommittedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ForecastCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    PhysicalProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    ProgressMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    WeightFactor = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 4, nullable: true),
                    ResponsibleUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrimaryDisciplineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CPI = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    SPI = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    EarnedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    PlannedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    IsBaselined = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    BaselineDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPackageDetails", x => x.Id);
                    table.CheckConstraint("CK_WorkPackageDetails_Budget", "[Budget] >= 0");
                    table.CheckConstraint("CK_WorkPackageDetails_Costs", "[ActualCost] >= 0 AND [CommittedCost] >= 0 AND [ForecastCost] >= 0");
                    table.CheckConstraint("CK_WorkPackageDetails_Dates", "[PlannedEndDate] > [PlannedStartDate]");
                    table.CheckConstraint("CK_WorkPackageDetails_Duration", "[PlannedDuration] > 0");
                    table.CheckConstraint("CK_WorkPackageDetails_PhysicalProgress", "[PhysicalProgressPercentage] >= 0 AND [PhysicalProgressPercentage] <= 100");
                    table.CheckConstraint("CK_WorkPackageDetails_Progress", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_WorkPackageDetails_Disciplines_PrimaryDisciplineId",
                        column: x => x.PrimaryDisciplineId,
                        principalSchema: "Organization",
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkPackageDetails_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkPackageDetails_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActualCosts",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualCosts", x => x.Id);
                    table.CheckConstraint("CK_ActualCosts_Amount", "[Amount] >= 0");
                    table.ForeignKey(
                        name: "FK_ActualCosts_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActualCosts_CostItems_CostItemId",
                        column: x => x.CostItemId,
                        principalSchema: "Cost",
                        principalTable: "CostItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetItems",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CostType = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountingCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetItems", x => x.Id);
                    table.CheckConstraint("CK_BudgetItems_Amount", "[Amount] >= 0");
                    table.CheckConstraint("CK_BudgetItems_Quantity", "[Quantity] >= 0");
                    table.CheckConstraint("CK_BudgetItems_UnitRate", "[UnitRate] >= 0");
                    table.ForeignKey(
                        name: "FK_BudgetItems_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalSchema: "Cost",
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetItems_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetItems_Packages_PackageId",
                        column: x => x.PackageId,
                        principalSchema: "Organization",
                        principalTable: "Packages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PackageDisciplines",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    IsLeadDiscipline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LeadEngineerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    LastProgressUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageDisciplines", x => x.Id);
                    table.CheckConstraint("CK_PackageDisciplines_ActualCost", "[ActualCost] >= 0");
                    table.CheckConstraint("CK_PackageDisciplines_ActualHours", "[ActualHours] >= 0");
                    table.CheckConstraint("CK_PackageDisciplines_EstimatedCost", "[EstimatedCost] >= 0");
                    table.CheckConstraint("CK_PackageDisciplines_EstimatedHours", "[EstimatedHours] >= 0");
                    table.CheckConstraint("CK_PackageDisciplines_ProgressPercentage", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_PackageDisciplines_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalSchema: "Organization",
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageDisciplines_Packages_PackageId",
                        column: x => x.PackageId,
                        principalSchema: "Organization",
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageDisciplines_Users_LeadEngineerId",
                        column: x => x.LeadEngineerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PlannedStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlannedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    ResourceRate = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: true),
                    PredecessorActivities = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SuccessorActivities = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    WorkPackageDetailsId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScheduleVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.CheckConstraint("CK_Activities_Dates", "[PlannedEndDate] > [PlannedStartDate]");
                    table.CheckConstraint("CK_Activities_DurationDays", "[DurationDays] > 0");
                    table.CheckConstraint("CK_Activities_Hours", "[PlannedHours] >= 0 AND [ActualHours] >= 0");
                    table.CheckConstraint("CK_Activities_Progress", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_Activities_ScheduleVersions_ScheduleVersionId",
                        column: x => x.ScheduleVersionId,
                        principalSchema: "Projects",
                        principalTable: "ScheduleVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Activities_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_WorkPackageDetails_WorkPackageDetailsId",
                        column: x => x.WorkPackageDetailsId,
                        principalSchema: "Projects",
                        principalTable: "WorkPackageDetails",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CostControlReportItems",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostControlReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    BudgetedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhysicalProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    EarnedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostVariance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ScheduleVariance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostPerformanceIndex = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 3, nullable: false),
                    EstimateAtCompletion = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ResponsiblePerson = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CostCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    VarianceExplanation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostControlReportItems", x => x.Id);
                    table.CheckConstraint("CK_CostControlReportItems_Amounts", "[BudgetedCost] >= 0 AND [ActualCost] >= 0 AND [EstimateAtCompletion] >= 0");
                    table.CheckConstraint("CK_CostControlReportItems_CPI", "[CostPerformanceIndex] >= 0");
                    table.CheckConstraint("CK_CostControlReportItems_OneLink", "([WBSElementId] IS NULL AND [WorkPackageId] IS NOT NULL) OR ([WBSElementId] IS NOT NULL AND [WorkPackageId] IS NULL) OR ([WBSElementId] IS NULL AND [WorkPackageId] IS NULL)");
                    table.CheckConstraint("CK_CostControlReportItems_Progress", "[PhysicalProgressPercentage] >= 0 AND [PhysicalProgressPercentage] <= 100");
                    table.CheckConstraint("CK_CostControlReportItems_SequenceNumber", "[SequenceNumber] > 0");
                    table.ForeignKey(
                        name: "FK_CostControlReportItems_CostControlReports_CostControlReportId",
                        column: x => x.CostControlReportId,
                        principalSchema: "Cost",
                        principalTable: "CostControlReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostControlReportItems_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostControlReportItems_WorkPackageDetails_WorkPackageId",
                        column: x => x.WorkPackageId,
                        principalSchema: "Projects",
                        principalTable: "WorkPackageDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CBSBudgetItems",
                schema: "Cost",
                columns: table => new
                {
                    BudgetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CBSId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CBSBudgetItems", x => new { x.BudgetItemId, x.CBSId });
                    table.ForeignKey(
                        name: "FK_CBSBudgetItems_BudgetItems_BudgetItemId",
                        column: x => x.BudgetItemId,
                        principalSchema: "Cost",
                        principalTable: "BudgetItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CBSBudgetItems_CBS_CBSId",
                        column: x => x.CBSId,
                        principalSchema: "Cost",
                        principalTable: "CBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commitments",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommitmentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RevisedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommittedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoicedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    RetentionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentTermsDays = table.Column<int>(type: "int", nullable: true),
                    RetentionPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    AdvancePaymentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContractNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VendorReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccountingReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PerformancePercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    LastInvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Terms = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ScopeOfWork = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Deliverables = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsFixedPrice = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsTimeAndMaterial = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commitments", x => x.Id);
                    table.CheckConstraint("CK_Commitments_Amounts", "[OriginalAmount] >= 0 AND [RevisedAmount] >= 0 AND [CommittedAmount] >= 0 AND [InvoicedAmount] >= 0 AND [PaidAmount] >= 0 AND [RetentionAmount] >= 0");
                    table.CheckConstraint("CK_Commitments_ContractType", "(([IsFixedPrice] = 1 AND [IsTimeAndMaterial] = 0) OR ([IsFixedPrice] = 0 AND [IsTimeAndMaterial] = 1))");
                    table.CheckConstraint("CK_Commitments_Dates", "[EndDate] > [StartDate]");
                    table.CheckConstraint("CK_Commitments_Percentages", "[RetentionPercentage] IS NULL OR ([RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100)");
                    table.CheckConstraint("CK_Commitments_Performance", "[PerformancePercentage] IS NULL OR ([PerformancePercentage] >= 0 AND [PerformancePercentage] <= 100)");
                    table.ForeignKey(
                        name: "FK_Commitments_BudgetItems_BudgetItemId",
                        column: x => x.BudgetItemId,
                        principalSchema: "Cost",
                        principalTable: "BudgetItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commitments_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commitments_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commitments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResourceName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EquipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    ActualQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    PlannedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsOverAllocated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.CheckConstraint("CK_Resources_ActualHours", "[ActualHours] >= 0");
                    table.CheckConstraint("CK_Resources_ActualQuantity", "[ActualQuantity] >= 0");
                    table.CheckConstraint("CK_Resources_Dates", "[EndDate] IS NULL OR [StartDate] IS NULL OR [EndDate] >= [StartDate]");
                    table.CheckConstraint("CK_Resources_PlannedHours", "[PlannedHours] >= 0");
                    table.CheckConstraint("CK_Resources_PlannedQuantity", "[PlannedQuantity] >= 0");
                    table.CheckConstraint("CK_Resources_UnitRate", "[UnitRate] IS NULL OR [UnitRate] >= 0");
                    table.ForeignKey(
                        name: "FK_Resources_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "Projects",
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resources_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Resources_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalSchema: "Projects",
                        principalTable: "Equipment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resources_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeOrders",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Justification = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    EstimatedCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ScheduleImpactDays = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ScopeImpact = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QualityImpact = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RiskImpact = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ContingencyUsed = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentApprovalLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovalSequence = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedById = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestorDepartment = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubmittedById = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedById = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedById = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImplementationStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImplementationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImplementationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AttachmentsJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of document references"),
                    ImpactAnalysisDocument = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeOrders", x => x.Id);
                    table.CheckConstraint("CK_ChangeOrders_ApprovedAmount", "[ApprovedAmount] IS NULL OR [ApprovedAmount] >= 0");
                    table.CheckConstraint("CK_ChangeOrders_ContingencyUsed", "[ContingencyUsed] IS NULL OR [ContingencyUsed] >= 0");
                    table.CheckConstraint("CK_ChangeOrders_EstimatedCostImpact", "[EstimatedCostImpact] >= 0");
                    table.CheckConstraint("CK_ChangeOrders_ImplementationDates", "[ImplementationEndDate] IS NULL OR [ImplementationStartDate] IS NULL OR [ImplementationEndDate] >= [ImplementationStartDate]");
                    table.CheckConstraint("CK_ChangeOrders_ScheduleImpactDays", "[ScheduleImpactDays] >= 0");
                    table.ForeignKey(
                        name: "FK_ChangeOrders_Commitments_CommitmentId",
                        column: x => x.CommitmentId,
                        principalSchema: "Cost",
                        principalTable: "Commitments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeOrders_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeOrders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeOrders_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommitmentItems",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DetailedDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Specifications = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequiredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PromisedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeliveryInstructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    InvoicedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    InvoicedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    DrawingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpecificationReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaterialCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VendorItemCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitmentItems", x => x.Id);
                    table.CheckConstraint("CK_CommitmentItems_Amounts", "[TotalPrice] >= 0 AND [NetAmount] >= 0 AND [LineTotal] >= 0");
                    table.CheckConstraint("CK_CommitmentItems_Percentages", "[DiscountPercentage] IS NULL OR ([DiscountPercentage] >= 0 AND [DiscountPercentage] <= 100)");
                    table.CheckConstraint("CK_CommitmentItems_Progress", "[DeliveredQuantity] >= 0 AND [InvoicedQuantity] >= 0 AND [InvoicedAmount] >= 0 AND [PaidAmount] >= 0");
                    table.CheckConstraint("CK_CommitmentItems_Quantity", "[Quantity] > 0");
                    table.CheckConstraint("CK_CommitmentItems_TaxRate", "[TaxRate] IS NULL OR ([TaxRate] >= 0 AND [TaxRate] <= 100)");
                    table.CheckConstraint("CK_CommitmentItems_UnitPrice", "[UnitPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_CommitmentItems_BudgetItems_BudgetItemId",
                        column: x => x.BudgetItemId,
                        principalSchema: "Cost",
                        principalTable: "BudgetItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommitmentItems_Commitments_CommitmentId",
                        column: x => x.CommitmentId,
                        principalSchema: "Cost",
                        principalTable: "Commitments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitmentRevisions",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    RevisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RevisedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ChangeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ChangePercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ChangeOrderReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitmentRevisions", x => x.Id);
                    table.CheckConstraint("CK_CommitmentRevisions_Amounts", "[PreviousAmount] >= 0 AND [RevisedAmount] >= 0");
                    table.CheckConstraint("CK_CommitmentRevisions_RevisionNumber", "[RevisionNumber] >= 0");
                    table.ForeignKey(
                        name: "FK_CommitmentRevisions_Commitments_CommitmentId",
                        column: x => x.CommitmentId,
                        principalSchema: "Cost",
                        principalTable: "Commitments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommitmentWorkPackages",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoicedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    RetainedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m),
                    LastProgressUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitmentWorkPackages", x => x.Id);
                    table.CheckConstraint("CK_CommitmentWorkPackages_Amounts", "[AllocatedAmount] >= 0 AND [InvoicedAmount] >= 0 AND [RetainedAmount] >= 0 AND [PaidAmount] >= 0");
                    table.CheckConstraint("CK_CommitmentWorkPackages_InvoicedVsAllocated", "[InvoicedAmount] <= [AllocatedAmount]");
                    table.CheckConstraint("CK_CommitmentWorkPackages_PaidVsInvoiced", "[PaidAmount] <= [InvoicedAmount]");
                    table.CheckConstraint("CK_CommitmentWorkPackages_Progress", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_CommitmentWorkPackages_Commitments_CommitmentId",
                        column: x => x.CommitmentId,
                        principalSchema: "Cost",
                        principalTable: "Commitments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommitmentWorkPackages_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeOrderApprovals",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ApprovalLevel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorityLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsWithinAuthority = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Conditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsConditional = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeOrderApprovals", x => x.Id);
                    table.CheckConstraint("CK_ChangeOrderApprovals_AuthorityLimit", "[AuthorityLimit] >= 0");
                    table.ForeignKey(
                        name: "FK_ChangeOrderApprovals_ChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Change",
                        principalTable: "ChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChangeOrderImpacts",
                schema: "Risks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Area = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Severity = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ScheduleImpactDays = table.Column<int>(type: "int", nullable: true),
                    AffectedWBSCodes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of affected WBS codes"),
                    AffectedStakeholders = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of affected stakeholder IDs"),
                    MitigationPlan = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MitigationCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ChangeOrderId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeOrderImpacts", x => x.Id);
                    table.CheckConstraint("CK_ChangeOrderImpacts_CostImpact", "[CostImpact] IS NULL OR [CostImpact] >= 0");
                    table.CheckConstraint("CK_ChangeOrderImpacts_MitigationCost", "[MitigationCost] IS NULL OR [MitigationCost] >= 0");
                    table.CheckConstraint("CK_ChangeOrderImpacts_ScheduleImpact", "[ScheduleImpactDays] IS NULL OR [ScheduleImpactDays] >= 0");
                    table.ForeignKey(
                        name: "FK_ChangeOrderImpacts_ChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Change",
                        principalTable: "ChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeOrderImpacts_ChangeOrders_ChangeOrderId1",
                        column: x => x.ChangeOrderId1,
                        principalSchema: "Change",
                        principalTable: "ChangeOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Trends",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RaisedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Category = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Priority = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    IdentifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImplementationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EstimatedScheduleImpactDays = table.Column<int>(type: "int", nullable: true),
                    QualityImpact = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ScopeImpact = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ImpactLevel = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    MinCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaxCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MostLikelyCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Probability = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: false, defaultValue: 50m),
                    Decision = table.Column<int>(type: "int", maxLength: 50, nullable: true),
                    DecisionRationale = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DecisionByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MitigationStrategy = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsConvertedToChangeOrder = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RootCause = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProposedSolution = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AlternativeSolutions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Assumptions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Risks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiresClientApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ClientApproved = table.Column<bool>(type: "bit", nullable: true),
                    ClientApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trends", x => x.Id);
                    table.CheckConstraint("CK_Trends_EstimatedCostImpact", "[EstimatedCostImpact] IS NULL OR [EstimatedCostImpact] >= 0");
                    table.CheckConstraint("CK_Trends_EstimatedScheduleImpact", "[EstimatedScheduleImpactDays] IS NULL OR [EstimatedScheduleImpactDays] >= 0");
                    table.CheckConstraint("CK_Trends_MinMaxCostImpact", "[MinCostImpact] IS NULL OR [MaxCostImpact] IS NULL OR [MinCostImpact] <= [MaxCostImpact]");
                    table.CheckConstraint("CK_Trends_Probability", "[Probability] >= 0 AND [Probability] <= 100");
                    table.ForeignKey(
                        name: "FK_Trends_ChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Change",
                        principalTable: "ChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trends_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trends_Users_DecisionByUserId",
                        column: x => x.DecisionByUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trends_Users_RaisedByUserId",
                        column: x => x.RaisedByUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trends_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Priority = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrendId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Source = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    WBSElementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ControlAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AffectedPackages = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of package IDs"),
                    AffectedDisciplines = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of discipline IDs"),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequiredByDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImplementationStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImplementationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HasCostImpact = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasScheduleImpact = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasQualityImpact = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasScopeImpact = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    HasSafetyImpact = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    OverallImpactLevel = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    EstimatedCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApprovedCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    CostImpactType = table.Column<int>(type: "int", maxLength: 50, nullable: true),
                    CostBreakdown = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON structure"),
                    EstimatedScheduleImpactDays = table.Column<int>(type: "int", nullable: true),
                    ApprovedScheduleImpactDays = table.Column<int>(type: "int", nullable: true),
                    CurrentCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProposedCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AffectsCriticalPath = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AffectedMilestones = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array"),
                    TechnicalJustification = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProposedSolution = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AlternativeOptions = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RiskAssessment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    QualityImpactDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SafetyImpactDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApprovalRoute = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array of approval steps"),
                    CurrentApprovalLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RequiredApprovalLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RequiresTechnicalReview = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiresClientApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TechnicalReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TechnicalReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TechnicalReviewComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TechnicallyApproved = table.Column<bool>(type: "bit", nullable: true),
                    CostReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CostReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CostReviewComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CostApproved = table.Column<bool>(type: "bit", nullable: true),
                    ScheduleReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScheduleReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ScheduleReviewComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ScheduleApproved = table.Column<bool>(type: "bit", nullable: true),
                    ApprovedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApprovalConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ClientReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientApproved = table.Column<bool>(type: "bit", nullable: true),
                    ClientApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsImplemented = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ActualCostImpact = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ActualScheduleImpactDays = table.Column<int>(type: "int", nullable: true),
                    ImplementationNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LessonsLearned = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsConvertedToChangeOrder = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DrawingReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array"),
                    SpecificationReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array"),
                    StandardReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON array"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.Id);
                    table.CheckConstraint("CK_ChangeRequests_ActualCostImpact", "[ActualCostImpact] IS NULL OR [ActualCostImpact] >= 0");
                    table.CheckConstraint("CK_ChangeRequests_ApprovalLevels", "[CurrentApprovalLevel] >= 0 AND [RequiredApprovalLevel] >= 1");
                    table.CheckConstraint("CK_ChangeRequests_ApprovedCostImpact", "[ApprovedCostImpact] IS NULL OR [ApprovedCostImpact] >= 0");
                    table.CheckConstraint("CK_ChangeRequests_EstimatedCostImpact", "[EstimatedCostImpact] IS NULL OR [EstimatedCostImpact] >= 0");
                    table.CheckConstraint("CK_ChangeRequests_ScheduleImpactDays", "[EstimatedScheduleImpactDays] IS NULL OR [EstimatedScheduleImpactDays] >= 0");
                    table.ForeignKey(
                        name: "FK_ChangeRequests_ChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Change",
                        principalTable: "ChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_ControlAccounts_ControlAccountId",
                        column: x => x.ControlAccountId,
                        principalSchema: "Cost",
                        principalTable: "ControlAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Trends_TrendId",
                        column: x => x.TrendId,
                        principalSchema: "Change",
                        principalTable: "Trends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_CostReviewerId",
                        column: x => x.CostReviewerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_RequestorId",
                        column: x => x.RequestorId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_ScheduleReviewerId",
                        column: x => x.ScheduleReviewerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_TechnicalReviewerId",
                        column: x => x.TechnicalReviewerId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_WBSElements_WBSElementId",
                        column: x => x.WBSElementId,
                        principalSchema: "Projects",
                        principalTable: "WBSElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrendAttachments",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendAttachments", x => x.Id);
                    table.CheckConstraint("CK_TrendAttachments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_TrendAttachments_Trends_TrendId",
                        column: x => x.TrendId,
                        principalSchema: "Change",
                        principalTable: "Trends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrendAttachments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrendComments",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrendId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrendComments_Trends_TrendId",
                        column: x => x.TrendId,
                        principalSchema: "Change",
                        principalTable: "Trends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrendComments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Variations",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangeOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrendId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Category = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    ContractReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContractorReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsContractual = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IssuedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OriginalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NegotiatedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ApprovedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    LaborCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaterialCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    EquipmentCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SubcontractorCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IndirectCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OverheadPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    ProfitPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    TimeExtensionDays = table.Column<int>(type: "int", nullable: true),
                    RevisedCompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCriticalPathImpacted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    QuantityChanges = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON structure"),
                    RateAdjustments = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "JSON structure"),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovalComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RequiresClientApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ClientApproved = table.Column<bool>(type: "bit", nullable: true),
                    ClientApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClientApprovalReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Justification = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ScopeOfWork = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImpactAnalysis = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ContractualBasis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DrawingReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    SpecificationReferences = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", maxLength: 50, nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AdvancePaymentPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    RetentionPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    IsDisputed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisputeReason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisputeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsImplemented = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ImplementedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CertifiedValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PaidValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variations", x => x.Id);
                    table.CheckConstraint("CK_Variations_ApprovalLevel", "[ApprovalLevel] >= 1 AND [ApprovalLevel] <= 5");
                    table.CheckConstraint("CK_Variations_ApprovedValue", "[ApprovedValue] >= 0");
                    table.CheckConstraint("CK_Variations_CostComponents", "([LaborCost] IS NULL OR [LaborCost] >= 0) AND ([MaterialCost] IS NULL OR [MaterialCost] >= 0) AND ([EquipmentCost] IS NULL OR [EquipmentCost] >= 0) AND ([SubcontractorCost] IS NULL OR [SubcontractorCost] >= 0) AND ([IndirectCost] IS NULL OR [IndirectCost] >= 0)");
                    table.CheckConstraint("CK_Variations_Implementation", "([ImplementedValue] IS NULL OR [ImplementedValue] >= 0) AND ([CertifiedValue] IS NULL OR [CertifiedValue] >= 0) AND ([PaidValue] IS NULL OR [PaidValue] >= 0)");
                    table.CheckConstraint("CK_Variations_OriginalValue", "[OriginalValue] >= 0");
                    table.CheckConstraint("CK_Variations_Percentages", "([OverheadPercentage] IS NULL OR ([OverheadPercentage] >= 0 AND [OverheadPercentage] <= 100)) AND ([ProfitPercentage] IS NULL OR ([ProfitPercentage] >= 0 AND [ProfitPercentage] <= 100)) AND ([AdvancePaymentPercentage] IS NULL OR ([AdvancePaymentPercentage] >= 0 AND [AdvancePaymentPercentage] <= 100)) AND ([RetentionPercentage] IS NULL OR ([RetentionPercentage] >= 0 AND [RetentionPercentage] <= 100))");
                    table.ForeignKey(
                        name: "FK_Variations_ChangeOrders_ChangeOrderId",
                        column: x => x.ChangeOrderId,
                        principalSchema: "Change",
                        principalTable: "ChangeOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Variations_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Variations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Variations_Trends_TrendId",
                        column: x => x.TrendId,
                        principalSchema: "Change",
                        principalTable: "Trends",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Variations_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Variations_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Variations_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestApprovals",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalLevel = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestApprovals", x => x.Id);
                    table.CheckConstraint("CK_ChangeRequestApprovals_ApprovalLevel", "[ApprovalLevel] >= 1 AND [ApprovalLevel] <= 10");
                    table.ForeignKey(
                        name: "FK_ChangeRequestApprovals_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalSchema: "Change",
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequestApprovals_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestAttachments",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestAttachments", x => x.Id);
                    table.CheckConstraint("CK_ChangeRequestAttachments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_ChangeRequestAttachments_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalSchema: "Change",
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequestAttachments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequestComments",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangeRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequestComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeRequestComments_ChangeRequests_ChangeRequestId",
                        column: x => x.ChangeRequestId,
                        principalSchema: "Change",
                        principalTable: "ChangeRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeRequestComments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommitmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContractorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VendorInvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Type = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RetentionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsTaxExempt = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PeriodStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceSequence = table.Column<int>(type: "int", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubmittedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ApprovalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SupportingDocuments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HasBackup = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    VariationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.CheckConstraint("CK_Invoices_Amounts", "[GrossAmount] >= 0 AND [TotalAmount] >= 0 AND [NetAmount] >= 0 AND [PaidAmount] >= 0");
                    table.CheckConstraint("CK_Invoices_Dates", "[DueDate] >= [InvoiceDate] AND [PeriodEndDate] >= [PeriodStartDate]");
                    table.CheckConstraint("CK_Invoices_TaxRate", "[TaxRate] IS NULL OR ([TaxRate] >= 0 AND [TaxRate] <= 100)");
                    table.ForeignKey(
                        name: "FK_Invoices_Commitments_CommitmentId",
                        column: x => x.CommitmentId,
                        principalSchema: "Cost",
                        principalTable: "Commitments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Contractors_ContractorId",
                        column: x => x.ContractorId,
                        principalSchema: "Organization",
                        principalTable: "Contractors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Variations_VariationId",
                        column: x => x.VariationId,
                        principalSchema: "Change",
                        principalTable: "Variations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VariationAttachments",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    UploadedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariationAttachments", x => x.Id);
                    table.CheckConstraint("CK_VariationAttachments_FileSize", "[FileSize] > 0");
                    table.ForeignKey(
                        name: "FK_VariationAttachments_Users_UploadedBy",
                        column: x => x.UploadedBy,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VariationAttachments_Variations_VariationId",
                        column: x => x.VariationId,
                        principalSchema: "Change",
                        principalTable: "Variations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariationItems",
                schema: "Change",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VariationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    OriginalQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    RevisedQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariationItems", x => x.Id);
                    table.CheckConstraint("CK_VariationItems_Quantities", "([OriginalQuantity] IS NULL OR [OriginalQuantity] >= 0) AND ([RevisedQuantity] IS NULL OR [RevisedQuantity] >= 0)");
                    table.CheckConstraint("CK_VariationItems_UnitRate", "[UnitRate] >= 0");
                    table.ForeignKey(
                        name: "FK_VariationItems_Variations_VariationId",
                        column: x => x.VariationId,
                        principalSchema: "Change",
                        principalTable: "Variations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                schema: "Cost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommitmentItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemNumber = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CostAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PreviousQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    CumulativeQuantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 4, nullable: true),
                    CompletionPercentage = table.Column<decimal>(type: "decimal(18,2)", precision: 5, scale: 2, nullable: true),
                    WorkOrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeliveryTicket = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.CheckConstraint("CK_InvoiceItems_Amounts", "[Amount] >= 0 AND [NetAmount] >= 0");
                    table.CheckConstraint("CK_InvoiceItems_DiscountAmount", "[DiscountAmount] IS NULL OR [DiscountAmount] >= 0");
                    table.CheckConstraint("CK_InvoiceItems_Progress", "([PreviousQuantity] IS NULL OR [PreviousQuantity] >= 0) AND ([CumulativeQuantity] IS NULL OR [CumulativeQuantity] >= 0) AND ([CompletionPercentage] IS NULL OR ([CompletionPercentage] >= 0 AND [CompletionPercentage] <= 100))");
                    table.CheckConstraint("CK_InvoiceItems_Quantity", "[Quantity] >= 0");
                    table.CheckConstraint("CK_InvoiceItems_UnitPrice", "[UnitPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_InvoiceItems_BudgetItems_BudgetItemId",
                        column: x => x.BudgetItemId,
                        principalSchema: "Cost",
                        principalTable: "BudgetItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_CommitmentItems_CommitmentItemId",
                        column: x => x.CommitmentItemId,
                        principalSchema: "Cost",
                        principalTable: "CommitmentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "Cost",
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentAttachments",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BlobContainerName = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    BlobStorageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentAttachments", x => x.Id);
                    table.CheckConstraint("CK_CommentAttachments_FileSize", "[FileSize] > 0");
                });

            migrationBuilder.CreateTable(
                name: "DocumentComments",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PageNumber = table.Column<int>(type: "int", nullable: true),
                    Reference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Severity = table.Column<int>(type: "int", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ResolvedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Resolution = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentComments", x => x.Id);
                    table.CheckConstraint("CK_DocumentComments_PageNumber", "[PageNumber] IS NULL OR [PageNumber] > 0");
                    table.ForeignKey(
                        name: "FK_DocumentComments_DocumentComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalSchema: "Documents",
                        principalTable: "DocumentComments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DocumentComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentComments_Users_ResolvedById",
                        column: x => x.ResolvedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentDistributions",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Purpose = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    DistributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RecipientCompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientRole = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    DistributedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransmittalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequiresAcknowledgment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AcknowledgedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcknowledgedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AcknowledgmentComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDownloaded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FirstDownloadedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastDownloadedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDistributions", x => x.Id);
                    table.CheckConstraint("CK_DocumentDistributions_DownloadCount", "[DownloadCount] >= 0");
                    table.CheckConstraint("CK_DocumentDistributions_Recipient", "[RecipientUserId] IS NOT NULL OR [RecipientEmail] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_DocumentDistributions_Companies_RecipientCompanyId",
                        column: x => x.RecipientCompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentDistributions_Transmittals_TransmittalId",
                        column: x => x.TransmittalId,
                        principalSchema: "Documents",
                        principalTable: "Transmittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentDistributions_Users_DistributedById",
                        column: x => x.DistributedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentDistributions_Users_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentPermissions",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CanView = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanDownload = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanComment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanDistribute = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanManagePermissions = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GrantedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentPermissions", x => x.Id);
                    table.CheckConstraint("CK_DocumentPermissions_UserOrRole", "[UserId] IS NOT NULL OR [RoleId] IS NOT NULL");
                    table.CheckConstraint("CK_DocumentPermissions_ValidPeriod", "[ValidFrom] IS NULL OR [ValidTo] IS NULL OR [ValidTo] > [ValidFrom]");
                    table.ForeignKey(
                        name: "FK_DocumentPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentRelationships",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelatedDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelationshipType = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstablishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstablishedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRelationships", x => x.Id);
                    table.CheckConstraint("CK_DocumentRelationships_NotSelfReferencing", "[DocumentId] <> [RelatedDocumentId]");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisciplineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentVersion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "A"),
                    CurrentRevisionNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CurrentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Originator = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ClientDocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ContractorDocumentNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Confidentiality = table.Column<int>(type: "int", nullable: false),
                    ReviewStatus = table.Column<int>(type: "int", nullable: false),
                    ReviewDueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewCompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TagsJson = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    Keywords = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetentionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetentionPolicy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastDownloadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastViewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.CheckConstraint("CK_Documents_Statistics", "[DownloadCount] >= 0 AND [ViewCount] >= 0");
                    table.CheckConstraint("CK_Documents_Version", "[CurrentRevisionNumber] >= 0");
                    table.ForeignKey(
                        name: "FK_Documents_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalSchema: "Organization",
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Packages_PackageId",
                        column: x => x.PackageId,
                        principalSchema: "Organization",
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalSchema: "Organization",
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Organization",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Users_LockedById",
                        column: x => x.LockedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Users_ReviewedById",
                        column: x => x.ReviewedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentVersions",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuePurpose = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    VersionComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    BlobContainerName = table.Column<string>(type: "nvarchar(63)", maxLength: 63, nullable: false),
                    BlobName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    BlobStorageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    BlobUploadedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsSuperseded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SupersededById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewStatus = table.Column<int>(type: "int", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewComments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastDownloadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVersions", x => x.Id);
                    table.CheckConstraint("CK_DocumentVersions_DownloadCount", "[DownloadCount] >= 0");
                    table.CheckConstraint("CK_DocumentVersions_FileSize", "[FileSize] > 0");
                    table.CheckConstraint("CK_DocumentVersions_RevisionNumber", "[RevisionNumber] >= 0");
                    table.ForeignKey(
                        name: "FK_DocumentVersions_DocumentVersions_SupersededById",
                        column: x => x.SupersededById,
                        principalSchema: "Documents",
                        principalTable: "DocumentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Documents",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_Users_ReviewedById",
                        column: x => x.ReviewedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_Users_UploadedById",
                        column: x => x.UploadedById,
                        principalSchema: "Security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransmittalDocuments",
                schema: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransmittalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Copies = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsIncluded = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmittalDocuments", x => x.Id);
                    table.CheckConstraint("CK_TransmittalDocuments_Copies", "[Copies] > 0");
                    table.ForeignKey(
                        name: "FK_TransmittalDocuments_DocumentVersions_DocumentVersionId",
                        column: x => x.DocumentVersionId,
                        principalSchema: "Documents",
                        principalTable: "DocumentVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmittalDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "Documents",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransmittalDocuments_Transmittals_TransmittalId",
                        column: x => x.TransmittalId,
                        principalSchema: "Documents",
                        principalTable: "Transmittals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_AccountType",
                schema: "Cost",
                table: "AccountCodes",
                column: "AccountType");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_Category",
                schema: "Cost",
                table: "AccountCodes",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_CostType",
                schema: "Cost",
                table: "AccountCodes",
                column: "CostType");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_IsActive",
                schema: "Cost",
                table: "AccountCodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_PhaseId",
                schema: "Cost",
                table: "AccountCodes",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_ProjectId",
                schema: "Cost",
                table: "AccountCodes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_ProjectId_PhaseId",
                schema: "Cost",
                table: "AccountCodes",
                columns: new[] { "ProjectId", "PhaseId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodes_ProjectId1",
                schema: "Cost",
                table: "AccountCodes",
                column: "ProjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityCode",
                schema: "Projects",
                table: "Activities",
                column: "ActivityCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PlannedEndDate",
                schema: "Projects",
                table: "Activities",
                column: "PlannedEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PlannedStartDate",
                schema: "Projects",
                table: "Activities",
                column: "PlannedStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ScheduleVersionId",
                schema: "Projects",
                table: "Activities",
                column: "ScheduleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Status",
                schema: "Projects",
                table: "Activities",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_Status_PlannedStartDate",
                schema: "Projects",
                table: "Activities",
                columns: new[] { "Status", "PlannedStartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_WBSElementId",
                schema: "Projects",
                table: "Activities",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_WBSElementId_ActivityCode",
                schema: "Projects",
                table: "Activities",
                columns: new[] { "WBSElementId", "ActivityCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Activities_WBSElementId_Status",
                schema: "Projects",
                table: "Activities",
                columns: new[] { "WBSElementId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_WorkPackageDetailsId",
                schema: "Projects",
                table: "Activities",
                column: "WorkPackageDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualCosts_ActualDate",
                schema: "Cost",
                table: "ActualCosts",
                column: "ActualDate");

            migrationBuilder.CreateIndex(
                name: "IX_ActualCosts_ControlAccountId",
                schema: "Cost",
                table: "ActualCosts",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualCosts_CostItemId",
                schema: "Cost",
                table: "ActualCosts",
                column: "CostItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualCosts_CostItemId_ActualDate",
                schema: "Cost",
                table: "ActualCosts",
                columns: new[] { "CostItemId", "ActualDate" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetId",
                schema: "Cost",
                table: "BudgetItems",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetId_ItemCode",
                schema: "Cost",
                table: "BudgetItems",
                columns: new[] { "BudgetId", "ItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_BudgetId_SortOrder",
                schema: "Cost",
                table: "BudgetItems",
                columns: new[] { "BudgetId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_Category",
                schema: "Cost",
                table: "BudgetItems",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_ControlAccountId",
                schema: "Cost",
                table: "BudgetItems",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_ControlAccountId_BudgetId",
                schema: "Cost",
                table: "BudgetItems",
                columns: new[] { "ControlAccountId", "BudgetId" });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_CostType",
                schema: "Cost",
                table: "BudgetItems",
                column: "CostType");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_IsDeleted",
                schema: "Cost",
                table: "BudgetItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_ItemCode",
                schema: "Cost",
                table: "BudgetItems",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetItems_PackageId",
                schema: "Cost",
                table: "BudgetItems",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRevisions_BudgetId",
                schema: "Cost",
                table: "BudgetRevisions",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRevisions_BudgetId_RevisionNumber",
                schema: "Cost",
                table: "BudgetRevisions",
                columns: new[] { "BudgetId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRevisions_IsApproved",
                schema: "Cost",
                table: "BudgetRevisions",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRevisions_RevisionDate",
                schema: "Cost",
                table: "BudgetRevisions",
                column: "RevisionDate");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRevisions_RevisionNumber",
                schema: "Cost",
                table: "BudgetRevisions",
                column: "RevisionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_IsBaseline",
                schema: "Cost",
                table: "Budgets",
                column: "IsBaseline");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_IsDeleted",
                schema: "Cost",
                table: "Budgets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_ParentBudgetId",
                schema: "Cost",
                table: "Budgets",
                column: "ParentBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_ProjectId",
                schema: "Cost",
                table: "Budgets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_ProjectId_IsBaseline",
                schema: "Cost",
                table: "Budgets",
                columns: new[] { "ProjectId", "IsBaseline" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_ProjectId_Status",
                schema: "Cost",
                table: "Budgets",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_ProjectId_Version",
                schema: "Cost",
                table: "Budgets",
                columns: new[] { "ProjectId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Status",
                schema: "Cost",
                table: "Budgets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Type",
                schema: "Cost",
                table: "Budgets",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_Version",
                schema: "Cost",
                table: "Budgets",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_Category",
                schema: "Cost",
                table: "CBS",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_Code",
                schema: "Cost",
                table: "CBS",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CBS_CostType",
                schema: "Cost",
                table: "CBS",
                column: "CostType");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_IsActive",
                schema: "Cost",
                table: "CBS",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_IsControlPoint",
                schema: "Cost",
                table: "CBS",
                column: "IsControlPoint");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_IsDeleted",
                schema: "Cost",
                table: "CBS",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_IsLeafNode",
                schema: "Cost",
                table: "CBS",
                column: "IsLeafNode");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_Level",
                schema: "Cost",
                table: "CBS",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_ParentId",
                schema: "Cost",
                table: "CBS",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_ParentId_SequenceNumber",
                schema: "Cost",
                table: "CBS",
                columns: new[] { "ParentId", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CBS_ProjectId",
                schema: "Cost",
                table: "CBS",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CBS_ProjectId_Code",
                schema: "Cost",
                table: "CBS",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CBS_ProjectId_Level",
                schema: "Cost",
                table: "CBS",
                columns: new[] { "ProjectId", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_CBSBudgetItems_CBSId",
                schema: "Cost",
                table: "CBSBudgetItems",
                column: "CBSId");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_CBSTemplateId",
                schema: "Configuration",
                table: "CBSTemplateElements",
                column: "CBSTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_CBSTemplateId_Code",
                schema: "Configuration",
                table: "CBSTemplateElements",
                columns: new[] { "CBSTemplateId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_CBSTemplateId_IsControlAccount",
                schema: "Configuration",
                table: "CBSTemplateElements",
                columns: new[] { "CBSTemplateId", "IsControlAccount" });

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_CBSTemplateId_Level",
                schema: "Configuration",
                table: "CBSTemplateElements",
                columns: new[] { "CBSTemplateId", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_CostType",
                schema: "Configuration",
                table: "CBSTemplateElements",
                column: "CostType");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_HierarchyPath",
                schema: "Configuration",
                table: "CBSTemplateElements",
                column: "HierarchyPath");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_IsControlAccount",
                schema: "Configuration",
                table: "CBSTemplateElements",
                column: "IsControlAccount");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_Level",
                schema: "Configuration",
                table: "CBSTemplateElements",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplateElements_ParentId",
                schema: "Configuration",
                table: "CBSTemplateElements",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_Code",
                schema: "Configuration",
                table: "CBSTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_CostType",
                schema: "Configuration",
                table: "CBSTemplates",
                column: "CostType");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_IndustryType",
                schema: "Configuration",
                table: "CBSTemplates",
                column: "IndustryType");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_IndustryType_CostType",
                schema: "Configuration",
                table: "CBSTemplates",
                columns: new[] { "IndustryType", "CostType" });

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_IsActive",
                schema: "Configuration",
                table: "CBSTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_IsActive_IsPublic_IsDeleted",
                schema: "Configuration",
                table: "CBSTemplates",
                columns: new[] { "IsActive", "IsPublic", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_IsDeleted",
                schema: "Configuration",
                table: "CBSTemplates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CBSTemplates_IsPublic",
                schema: "Configuration",
                table: "CBSTemplates",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderApprovals_ApprovalLevel",
                schema: "Change",
                table: "ChangeOrderApprovals",
                column: "ApprovalLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderApprovals_ApproverId",
                schema: "Change",
                table: "ChangeOrderApprovals",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderApprovals_ChangeOrderId",
                schema: "Change",
                table: "ChangeOrderApprovals",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderApprovals_ChangeOrderId_ApprovalLevel",
                schema: "Change",
                table: "ChangeOrderApprovals",
                columns: new[] { "ChangeOrderId", "ApprovalLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderApprovals_Decision",
                schema: "Change",
                table: "ChangeOrderApprovals",
                column: "Decision");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderApprovals_DecisionDate",
                schema: "Change",
                table: "ChangeOrderApprovals",
                column: "DecisionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderDocuments_ChangeOrderId",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderDocuments_ChangeOrderId_DocumentType_IsActive",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                columns: new[] { "ChangeOrderId", "DocumentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderDocuments_DocumentId",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderDocuments_DocumentType",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderDocuments_IsActive",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderDocuments_UploadedDate",
                schema: "Contracts",
                table: "ChangeOrderDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderImpacts_Area",
                schema: "Risks",
                table: "ChangeOrderImpacts",
                column: "Area");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderImpacts_ChangeOrderId",
                schema: "Risks",
                table: "ChangeOrderImpacts",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderImpacts_ChangeOrderId_Area",
                schema: "Risks",
                table: "ChangeOrderImpacts",
                columns: new[] { "ChangeOrderId", "Area" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderImpacts_ChangeOrderId1",
                schema: "Risks",
                table: "ChangeOrderImpacts",
                column: "ChangeOrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderImpacts_Severity",
                schema: "Risks",
                table: "ChangeOrderImpacts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderMilestones_ChangeOrderId",
                schema: "Contracts",
                table: "ChangeOrderMilestones",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderMilestones_ChangeOrderId_MilestoneId",
                schema: "Contracts",
                table: "ChangeOrderMilestones",
                columns: new[] { "ChangeOrderId", "MilestoneId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderMilestones_ImpactType",
                schema: "Contracts",
                table: "ChangeOrderMilestones",
                column: "ImpactType");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderMilestones_MilestoneId",
                schema: "Contracts",
                table: "ChangeOrderMilestones",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderRelations_ChangeOrderId",
                schema: "Contracts",
                table: "ChangeOrderRelations",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderRelations_ChangeOrderId_RelatedChangeOrderId",
                schema: "Contracts",
                table: "ChangeOrderRelations",
                columns: new[] { "ChangeOrderId", "RelatedChangeOrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderRelations_RelatedChangeOrderId",
                schema: "Contracts",
                table: "ChangeOrderRelations",
                column: "RelatedChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrderRelations_RelationType",
                schema: "Contracts",
                table: "ChangeOrderRelations",
                column: "RelationType");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_Category",
                schema: "Change",
                table: "ChangeOrders",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_ChangeOrderNumber",
                schema: "Change",
                table: "ChangeOrders",
                column: "ChangeOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_CommitmentId",
                schema: "Change",
                table: "ChangeOrders",
                column: "CommitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_ControlAccountId",
                schema: "Change",
                table: "ChangeOrders",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_IsDeleted",
                schema: "Change",
                table: "ChangeOrders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_Priority",
                schema: "Change",
                table: "ChangeOrders",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_ProjectId",
                schema: "Change",
                table: "ChangeOrders",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_ProjectId_Status",
                schema: "Change",
                table: "ChangeOrders",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_ProjectId_Type",
                schema: "Change",
                table: "ChangeOrders",
                columns: new[] { "ProjectId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_RequestDate",
                schema: "Change",
                table: "ChangeOrders",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_Status",
                schema: "Change",
                table: "ChangeOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_Type",
                schema: "Change",
                table: "ChangeOrders",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeOrders_WBSElementId",
                schema: "Change",
                table: "ChangeOrders",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestApprovals_ApprovalDate",
                schema: "Change",
                table: "ChangeRequestApprovals",
                column: "ApprovalDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestApprovals_ApprovalLevel",
                schema: "Change",
                table: "ChangeRequestApprovals",
                column: "ApprovalLevel");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestApprovals_ApproverId",
                schema: "Change",
                table: "ChangeRequestApprovals",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestApprovals_ChangeRequestId",
                schema: "Change",
                table: "ChangeRequestApprovals",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestApprovals_ChangeRequestId_ApprovalLevel",
                schema: "Change",
                table: "ChangeRequestApprovals",
                columns: new[] { "ChangeRequestId", "ApprovalLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestAttachments_Category",
                schema: "Change",
                table: "ChangeRequestAttachments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestAttachments_ChangeRequestId",
                schema: "Change",
                table: "ChangeRequestAttachments",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestAttachments_UploadedBy",
                schema: "Change",
                table: "ChangeRequestAttachments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestAttachments_UploadedDate",
                schema: "Change",
                table: "ChangeRequestAttachments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestComments_ChangeRequestId",
                schema: "Change",
                table: "ChangeRequestComments",
                column: "ChangeRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestComments_CommentDate",
                schema: "Change",
                table: "ChangeRequestComments",
                column: "CommentDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestComments_IsInternal",
                schema: "Change",
                table: "ChangeRequestComments",
                column: "IsInternal");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequestComments_UserId",
                schema: "Change",
                table: "ChangeRequestComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ApprovedById",
                schema: "Change",
                table: "ChangeRequests",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_Category",
                schema: "Change",
                table: "ChangeRequests",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ChangeOrderId",
                schema: "Change",
                table: "ChangeRequests",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_Code",
                schema: "Change",
                table: "ChangeRequests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ContractorId",
                schema: "Change",
                table: "ChangeRequests",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ControlAccountId",
                schema: "Change",
                table: "ChangeRequests",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_CostReviewerId",
                schema: "Change",
                table: "ChangeRequests",
                column: "CostReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_IsDeleted",
                schema: "Change",
                table: "ChangeRequests",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_Priority",
                schema: "Change",
                table: "ChangeRequests",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ProjectId",
                schema: "Change",
                table: "ChangeRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ProjectId_Status",
                schema: "Change",
                table: "ChangeRequests",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ProjectId_Type",
                schema: "Change",
                table: "ChangeRequests",
                columns: new[] { "ProjectId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_RequestDate",
                schema: "Change",
                table: "ChangeRequests",
                column: "RequestDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_RequestorId",
                schema: "Change",
                table: "ChangeRequests",
                column: "RequestorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_RequiredByDate",
                schema: "Change",
                table: "ChangeRequests",
                column: "RequiredByDate");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ScheduleReviewerId",
                schema: "Change",
                table: "ChangeRequests",
                column: "ScheduleReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_Status",
                schema: "Change",
                table: "ChangeRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_TechnicalReviewerId",
                schema: "Change",
                table: "ChangeRequests",
                column: "TechnicalReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_TrendId",
                schema: "Change",
                table: "ChangeRequests",
                column: "TrendId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_Type",
                schema: "Change",
                table: "ChangeRequests",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_WBSElementId",
                schema: "Change",
                table: "ChangeRequests",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimChangeOrders_ChangeOrderId",
                schema: "Contracts",
                table: "ClaimChangeOrders",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimChangeOrders_ClaimId",
                schema: "Contracts",
                table: "ClaimChangeOrders",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimChangeOrders_ClaimId_ChangeOrderId",
                schema: "Contracts",
                table: "ClaimChangeOrders",
                columns: new[] { "ClaimId", "ChangeOrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClaimChangeOrders_RelationType",
                schema: "Contracts",
                table: "ClaimChangeOrders",
                column: "RelationType");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_ClaimId",
                schema: "Contracts",
                table: "ClaimDocuments",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_ClaimId_DocumentType_IsActive",
                schema: "Contracts",
                table: "ClaimDocuments",
                columns: new[] { "ClaimId", "DocumentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_DocumentId",
                schema: "Contracts",
                table: "ClaimDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_DocumentType",
                schema: "Contracts",
                table: "ClaimDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_IsActive",
                schema: "Contracts",
                table: "ClaimDocuments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDocuments_UploadedDate",
                schema: "Contracts",
                table: "ClaimDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimRelations_ClaimId",
                schema: "Contracts",
                table: "ClaimRelations",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimRelations_ClaimId_RelatedClaimId",
                schema: "Contracts",
                table: "ClaimRelations",
                columns: new[] { "ClaimId", "RelatedClaimId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClaimRelations_RelatedClaimId",
                schema: "Contracts",
                table: "ClaimRelations",
                column: "RelatedClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimRelations_RelationType",
                schema: "Contracts",
                table: "ClaimRelations",
                column: "RelationType");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClaimNumber",
                schema: "Contracts",
                table: "Claims",
                column: "ClaimNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ContractId",
                schema: "Contracts",
                table: "Claims",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ContractId_Status",
                schema: "Contracts",
                table: "Claims",
                columns: new[] { "ContractId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_Direction",
                schema: "Contracts",
                table: "Claims",
                column: "Direction");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_EventDate",
                schema: "Contracts",
                table: "Claims",
                column: "EventDate");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_IsActive",
                schema: "Contracts",
                table: "Claims",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_Priority",
                schema: "Contracts",
                table: "Claims",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ResponseDueDate",
                schema: "Contracts",
                table: "Claims",
                column: "ResponseDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_Status",
                schema: "Contracts",
                table: "Claims",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_SubmissionDate",
                schema: "Contracts",
                table: "Claims",
                column: "SubmissionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_Type",
                schema: "Contracts",
                table: "Claims",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CommentAttachments_BlobName",
                schema: "Documents",
                table: "CommentAttachments",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_CommentAttachments_CommentId",
                schema: "Documents",
                table: "CommentAttachments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentAttachments_UploadedDate",
                schema: "Documents",
                table: "CommentAttachments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_BudgetItemId",
                schema: "Cost",
                table: "CommitmentItems",
                column: "BudgetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_CommitmentId",
                schema: "Cost",
                table: "CommitmentItems",
                column: "CommitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_CommitmentId_ItemNumber",
                schema: "Cost",
                table: "CommitmentItems",
                columns: new[] { "CommitmentId", "ItemNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_CommitmentId_Status",
                schema: "Cost",
                table: "CommitmentItems",
                columns: new[] { "CommitmentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_IsDeleted",
                schema: "Cost",
                table: "CommitmentItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_ItemCode",
                schema: "Cost",
                table: "CommitmentItems",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_ItemNumber",
                schema: "Cost",
                table: "CommitmentItems",
                column: "ItemNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentItems_Status",
                schema: "Cost",
                table: "CommitmentItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentRevisions_CommitmentId",
                schema: "Cost",
                table: "CommitmentRevisions",
                column: "CommitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentRevisions_CommitmentId_RevisionNumber",
                schema: "Cost",
                table: "CommitmentRevisions",
                columns: new[] { "CommitmentId", "RevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentRevisions_RevisionDate",
                schema: "Cost",
                table: "CommitmentRevisions",
                column: "RevisionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentRevisions_RevisionNumber",
                schema: "Cost",
                table: "CommitmentRevisions",
                column: "RevisionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_BudgetItemId",
                schema: "Cost",
                table: "Commitments",
                column: "BudgetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_CommitmentNumber",
                schema: "Cost",
                table: "Commitments",
                column: "CommitmentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_ContractDate",
                schema: "Cost",
                table: "Commitments",
                column: "ContractDate");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_ContractorId",
                schema: "Cost",
                table: "Commitments",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_ContractorId_Status",
                schema: "Cost",
                table: "Commitments",
                columns: new[] { "ContractorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_ControlAccountId",
                schema: "Cost",
                table: "Commitments",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_IsDeleted",
                schema: "Cost",
                table: "Commitments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_ProjectId",
                schema: "Cost",
                table: "Commitments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_ProjectId_Status",
                schema: "Cost",
                table: "Commitments",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_Status",
                schema: "Cost",
                table: "Commitments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Commitments_Type",
                schema: "Cost",
                table: "Commitments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentWorkPackages_CommitmentId",
                schema: "Cost",
                table: "CommitmentWorkPackages",
                column: "CommitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentWorkPackages_CommitmentId_WBSElementId",
                schema: "Cost",
                table: "CommitmentWorkPackages",
                columns: new[] { "CommitmentId", "WBSElementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentWorkPackages_ProgressPercentage",
                schema: "Cost",
                table: "CommitmentWorkPackages",
                column: "ProgressPercentage");

            migrationBuilder.CreateIndex(
                name: "IX_CommitmentWorkPackages_WBSElementId",
                schema: "Cost",
                table: "CommitmentWorkPackages",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Code",
                schema: "Organization",
                table: "Companies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_IsDeleted",
                schema: "Organization",
                table: "Companies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_TaxId",
                schema: "Organization",
                table: "Companies",
                column: "TaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_ApprovalDate",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "ApprovalDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_ChangeOrderNumber",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "ChangeOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_ContractId",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_ContractId_Status",
                schema: "Contracts",
                table: "ContractChangeOrders",
                columns: new[] { "ContractId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_IsActive",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_Priority",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_Status",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_SubmissionDate",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "SubmissionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContractChangeOrders_Type",
                schema: "Contracts",
                table: "ContractChangeOrders",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_ContractId",
                schema: "Contracts",
                table: "ContractDocuments",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_ContractId_DocumentType_IsActive",
                schema: "Contracts",
                table: "ContractDocuments",
                columns: new[] { "ContractId", "DocumentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_DocumentId",
                schema: "Contracts",
                table: "ContractDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_DocumentType",
                schema: "Contracts",
                table: "ContractDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_IsActive",
                schema: "Contracts",
                table: "ContractDocuments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ContractDocuments_UploadedDate",
                schema: "Contracts",
                table: "ContractDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_ContractId",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_ContractId_MilestoneCode",
                schema: "Contracts",
                table: "ContractMilestones",
                columns: new[] { "ContractId", "MilestoneCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_ContractId_SequenceNumber",
                schema: "Contracts",
                table: "ContractMilestones",
                columns: new[] { "ContractId", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_ContractId_Status",
                schema: "Contracts",
                table: "ContractMilestones",
                columns: new[] { "ContractId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_IsActive",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_IsCritical",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "IsCritical");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_IsPaymentMilestone",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "IsPaymentMilestone");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_PlannedDate",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "PlannedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_Status",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ContractMilestones_Type",
                schema: "Contracts",
                table: "ContractMilestones",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Classification",
                schema: "Organization",
                table: "Contractors",
                column: "Classification");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Code",
                schema: "Organization",
                table: "Contractors",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_IsActive",
                schema: "Organization",
                table: "Contractors",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_IsDeleted",
                schema: "Organization",
                table: "Contractors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_IsPrequalified",
                schema: "Organization",
                table: "Contractors",
                column: "IsPrequalified");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Name",
                schema: "Organization",
                table: "Contractors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Status",
                schema: "Organization",
                table: "Contractors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_TaxId",
                schema: "Organization",
                table: "Contractors",
                column: "TaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contractors_Type",
                schema: "Organization",
                table: "Contractors",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Category",
                schema: "Contracts",
                table: "Contracts",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractNumber",
                schema: "Contracts",
                table: "Contracts",
                column: "ContractNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractorId",
                schema: "Contracts",
                table: "Contracts",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractorId_Status",
                schema: "Contracts",
                table: "Contracts",
                columns: new[] { "ContractorId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_IsActive",
                schema: "Contracts",
                table: "Contracts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ProjectId",
                schema: "Contracts",
                table: "Contracts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ProjectId_Status",
                schema: "Contracts",
                table: "Contracts",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Status",
                schema: "Contracts",
                table: "Contracts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Status_CurrentEndDate",
                schema: "Contracts",
                table: "Contracts",
                columns: new[] { "Status", "CurrentEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_Type",
                schema: "Contracts",
                table: "Contracts",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_AssignedDate",
                schema: "Cost",
                table: "ControlAccountAssignments",
                column: "AssignedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_ControlAccountId",
                schema: "Cost",
                table: "ControlAccountAssignments",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_ControlAccountId_UserId_Role",
                schema: "Cost",
                table: "ControlAccountAssignments",
                columns: new[] { "ControlAccountId", "UserId", "Role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_IsActive",
                schema: "Cost",
                table: "ControlAccountAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_Role",
                schema: "Cost",
                table: "ControlAccountAssignments",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_UserId",
                schema: "Cost",
                table: "ControlAccountAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccountAssignments_UserId_IsActive",
                schema: "Cost",
                table: "ControlAccountAssignments",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_CAMUserId",
                schema: "Cost",
                table: "ControlAccounts",
                column: "CAMUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_CAMUserId_Status",
                schema: "Cost",
                table: "ControlAccounts",
                columns: new[] { "CAMUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_Code",
                schema: "Cost",
                table: "ControlAccounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_IsActive",
                schema: "Cost",
                table: "ControlAccounts",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_IsDeleted",
                schema: "Cost",
                table: "ControlAccounts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_PhaseId",
                schema: "Cost",
                table: "ControlAccounts",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_ProjectId",
                schema: "Cost",
                table: "ControlAccounts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_ProjectId_Code",
                schema: "Cost",
                table: "ControlAccounts",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_ProjectId_Status",
                schema: "Cost",
                table: "ControlAccounts",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ControlAccounts_Status",
                schema: "Cost",
                table: "ControlAccounts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReportItems_CostControlReportId",
                schema: "Cost",
                table: "CostControlReportItems",
                column: "CostControlReportId");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReportItems_CostControlReportId_SequenceNumber",
                schema: "Cost",
                table: "CostControlReportItems",
                columns: new[] { "CostControlReportId", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReportItems_SequenceNumber",
                schema: "Cost",
                table: "CostControlReportItems",
                column: "SequenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReportItems_WBSElementId",
                schema: "Cost",
                table: "CostControlReportItems",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReportItems_WorkPackageId",
                schema: "Cost",
                table: "CostControlReportItems",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReports_ControlAccountId",
                schema: "Cost",
                table: "CostControlReports",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReports_ControlAccountId_ReportDate",
                schema: "Cost",
                table: "CostControlReports",
                columns: new[] { "ControlAccountId", "ReportDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReports_PeriodType",
                schema: "Cost",
                table: "CostControlReports",
                column: "PeriodType");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReports_ProjectId",
                schema: "Cost",
                table: "CostControlReports",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReports_ProjectId_ReportDate",
                schema: "Cost",
                table: "CostControlReports",
                columns: new[] { "ProjectId", "ReportDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CostControlReports_ReportDate",
                schema: "Cost",
                table: "CostControlReports",
                column: "ReportDate");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_Category",
                schema: "Cost",
                table: "CostItems",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_CBSId",
                schema: "Cost",
                table: "CostItems",
                column: "CBSId");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_ControlAccountId",
                schema: "Cost",
                table: "CostItems",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_ControlAccountId_Type",
                schema: "Cost",
                table: "CostItems",
                columns: new[] { "ControlAccountId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_IsDeleted",
                schema: "Cost",
                table: "CostItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_ItemCode",
                schema: "Cost",
                table: "CostItems",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_ProjectId",
                schema: "Cost",
                table: "CostItems",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_ProjectId_ItemCode",
                schema: "Cost",
                table: "CostItems",
                columns: new[] { "ProjectId", "ItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_Status",
                schema: "Cost",
                table: "CostItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_Type",
                schema: "Cost",
                table: "CostItems",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_WBSElementId",
                schema: "Cost",
                table: "CostItems",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_CostItems_WBSElementId_Category",
                schema: "Cost",
                table: "CostItems",
                columns: new[] { "WBSElementId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                schema: "Organization",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsActive",
                schema: "Organization",
                table: "Currencies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsBaseCurrency",
                schema: "Organization",
                table: "Currencies",
                column: "IsBaseCurrency");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsEnabledForCommitments_IsActive",
                schema: "Organization",
                table: "Currencies",
                columns: new[] { "IsEnabledForCommitments", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_IsEnabledForProjects_IsActive",
                schema: "Organization",
                table: "Currencies",
                columns: new[] { "IsEnabledForProjects", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_NumericCode",
                schema: "Organization",
                table: "Currencies",
                column: "NumericCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_Code",
                schema: "Organization",
                table: "Disciplines",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_IsDeleted",
                schema: "Organization",
                table: "Disciplines",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_Name",
                schema: "Organization",
                table: "Disciplines",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_AuthorId",
                schema: "Documents",
                table: "DocumentComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_AuthorId_CommentDate",
                schema: "Documents",
                table: "DocumentComments",
                columns: new[] { "AuthorId", "CommentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_CommentDate",
                schema: "Documents",
                table: "DocumentComments",
                column: "CommentDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_DocumentId",
                schema: "Documents",
                table: "DocumentComments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_DocumentId_Status",
                schema: "Documents",
                table: "DocumentComments",
                columns: new[] { "DocumentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_DocumentVersionId",
                schema: "Documents",
                table: "DocumentComments",
                column: "DocumentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_IsDeleted",
                schema: "Documents",
                table: "DocumentComments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_IsResolved",
                schema: "Documents",
                table: "DocumentComments",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_ParentCommentId",
                schema: "Documents",
                table: "DocumentComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_ResolvedById",
                schema: "Documents",
                table: "DocumentComments",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_Status",
                schema: "Documents",
                table: "DocumentComments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComments_Type",
                schema: "Documents",
                table: "DocumentComments",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_DistributedById",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "DistributedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_DistributionDate",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "DistributionDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_DocumentId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_DocumentId_Method",
                schema: "Documents",
                table: "DocumentDistributions",
                columns: new[] { "DocumentId", "Method" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_DocumentVersionId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "DocumentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_ExpiryDate",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_IsAcknowledged",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "IsAcknowledged");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_IsDownloaded",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "IsDownloaded");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_Method",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "Method");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_Purpose",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "Purpose");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_RecipientCompanyId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "RecipientCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_RecipientUserId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_RecipientUserId_DistributionDate",
                schema: "Documents",
                table: "DocumentDistributions",
                columns: new[] { "RecipientUserId", "DistributionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_RequiresAcknowledgment",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "RequiresAcknowledgment");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDistributions_TransmittalId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "TransmittalId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_DocumentId",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_DocumentId_RoleId",
                schema: "Documents",
                table: "DocumentPermissions",
                columns: new[] { "DocumentId", "RoleId" },
                unique: true,
                filter: "[RoleId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_DocumentId_UserId",
                schema: "Documents",
                table: "DocumentPermissions",
                columns: new[] { "DocumentId", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_GrantedById",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "GrantedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_GrantedDate",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "GrantedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_RoleId",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_UserId",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_ValidFrom",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentPermissions_ValidTo",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationships_DocumentId",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationships_EstablishedById",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "EstablishedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationships_EstablishedDate",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "EstablishedDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationships_RelatedDocumentId",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "RelatedDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationships_RelationshipType",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "RelationshipType");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentRelationships_Unique",
                schema: "Documents",
                table: "DocumentRelationships",
                columns: new[] { "DocumentId", "RelatedDocumentId", "RelationshipType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AuthorId",
                schema: "Documents",
                table: "Documents",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Category",
                schema: "Documents",
                table: "Documents",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CurrentVersionId",
                schema: "Documents",
                table: "Documents",
                column: "CurrentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DisciplineId",
                schema: "Documents",
                table: "Documents",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentNumber",
                schema: "Documents",
                table: "Documents",
                column: "DocumentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IsDeleted",
                schema: "Documents",
                table: "Documents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_LockedById",
                schema: "Documents",
                table: "Documents",
                column: "LockedById");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PackageId",
                schema: "Documents",
                table: "Documents",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PhaseId",
                schema: "Documents",
                table: "Documents",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProjectId",
                schema: "Documents",
                table: "Documents",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProjectId_Status",
                schema: "Documents",
                table: "Documents",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReviewDueDate",
                schema: "Documents",
                table: "Documents",
                column: "ReviewDueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReviewedById",
                schema: "Documents",
                table: "Documents",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReviewStatus",
                schema: "Documents",
                table: "Documents",
                column: "ReviewStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Status",
                schema: "Documents",
                table: "Documents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Type",
                schema: "Documents",
                table: "Documents",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Type_Status",
                schema: "Documents",
                table: "Documents",
                columns: new[] { "Type", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_BlobName",
                schema: "Documents",
                table: "DocumentVersions",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentId",
                schema: "Documents",
                table: "DocumentVersions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentId_Version_RevisionNumber",
                schema: "Documents",
                table: "DocumentVersions",
                columns: new[] { "DocumentId", "Version", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_IsCurrent",
                schema: "Documents",
                table: "DocumentVersions",
                column: "IsCurrent");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_IssueDate",
                schema: "Documents",
                table: "DocumentVersions",
                column: "IssueDate");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_IsSuperseded",
                schema: "Documents",
                table: "DocumentVersions",
                column: "IsSuperseded");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_ReviewedById",
                schema: "Documents",
                table: "DocumentVersions",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_ReviewStatus",
                schema: "Documents",
                table: "DocumentVersions",
                column: "ReviewStatus");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_SupersededById",
                schema: "Documents",
                table: "DocumentVersions",
                column: "SupersededById");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_UploadedById",
                schema: "Documents",
                table: "DocumentVersions",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_Category",
                schema: "Projects",
                table: "Equipment",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_ContractorId",
                schema: "Projects",
                table: "Equipment",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_CurrentProjectId",
                schema: "Projects",
                table: "Equipment",
                column: "CurrentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_EquipmentCode",
                schema: "Projects",
                table: "Equipment",
                column: "EquipmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_IsActive",
                schema: "Projects",
                table: "Equipment",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_IsActive_IsAvailable",
                schema: "Projects",
                table: "Equipment",
                columns: new[] { "IsActive", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_IsAvailable",
                schema: "Projects",
                table: "Equipment",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_IsDeleted",
                schema: "Projects",
                table: "Equipment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_ControlAccountId",
                schema: "Cost",
                table: "EVMRecords",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_ControlAccountId_DataDate",
                schema: "Cost",
                table: "EVMRecords",
                columns: new[] { "ControlAccountId", "DataDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_ControlAccountId_IsBaseline",
                schema: "Cost",
                table: "EVMRecords",
                columns: new[] { "ControlAccountId", "IsBaseline" });

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_ControlAccountId_Year_PeriodNumber",
                schema: "Cost",
                table: "EVMRecords",
                columns: new[] { "ControlAccountId", "Year", "PeriodNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_DataDate",
                schema: "Cost",
                table: "EVMRecords",
                column: "DataDate");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_IsApproved",
                schema: "Cost",
                table: "EVMRecords",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_IsBaseline",
                schema: "Cost",
                table: "EVMRecords",
                column: "IsBaseline");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_PeriodType",
                schema: "Cost",
                table: "EVMRecords",
                column: "PeriodType");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_Status",
                schema: "Cost",
                table: "EVMRecords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EVMRecords_Year",
                schema: "Cost",
                table: "EVMRecords",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyFrom",
                schema: "Cost",
                table: "ExchangeRates",
                column: "CurrencyFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyFrom_CurrencyTo_Date",
                schema: "Cost",
                table: "ExchangeRates",
                columns: new[] { "CurrencyFrom", "CurrencyTo", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyTo",
                schema: "Cost",
                table: "ExchangeRates",
                column: "CurrencyTo");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_Date",
                schema: "Cost",
                table: "ExchangeRates",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_IsOfficial",
                schema: "Cost",
                table: "ExchangeRates",
                column: "IsOfficial");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ProjectId",
                schema: "Cost",
                table: "ExchangeRates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_ProjectId_CurrencyFrom_CurrencyTo_Date",
                schema: "Cost",
                table: "ExchangeRates",
                columns: new[] { "ProjectId", "CurrencyFrom", "CurrencyTo", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_BudgetItemId",
                schema: "Cost",
                table: "InvoiceItems",
                column: "BudgetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_CommitmentItemId",
                schema: "Cost",
                table: "InvoiceItems",
                column: "CommitmentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                schema: "Cost",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId_ItemNumber",
                schema: "Cost",
                table: "InvoiceItems",
                columns: new[] { "InvoiceId", "ItemNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ItemCode",
                schema: "Cost",
                table: "InvoiceItems",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CommitmentId",
                schema: "Cost",
                table: "Invoices",
                column: "CommitmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CommitmentId_InvoiceDate",
                schema: "Cost",
                table: "Invoices",
                columns: new[] { "CommitmentId", "InvoiceDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ContractorId",
                schema: "Cost",
                table: "Invoices",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_DueDate",
                schema: "Cost",
                table: "Invoices",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate",
                schema: "Cost",
                table: "Invoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                schema: "Cost",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_IsDeleted",
                schema: "Cost",
                table: "Invoices",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status",
                schema: "Cost",
                table: "Invoices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Type",
                schema: "Cost",
                table: "Invoices",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_VariationId",
                schema: "Cost",
                table: "Invoices",
                column: "VariationId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDependencies_DependencyType",
                schema: "Contracts",
                table: "MilestoneDependencies",
                column: "DependencyType");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDependencies_PredecessorId",
                schema: "Contracts",
                table: "MilestoneDependencies",
                column: "PredecessorId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDependencies_PredecessorId_SuccessorId",
                schema: "Contracts",
                table: "MilestoneDependencies",
                columns: new[] { "PredecessorId", "SuccessorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDependencies_SuccessorId",
                schema: "Contracts",
                table: "MilestoneDependencies",
                column: "SuccessorId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDocuments_DocumentId",
                schema: "Contracts",
                table: "MilestoneDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDocuments_DocumentType",
                schema: "Contracts",
                table: "MilestoneDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDocuments_IsActive",
                schema: "Contracts",
                table: "MilestoneDocuments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDocuments_MilestoneId",
                schema: "Contracts",
                table: "MilestoneDocuments",
                column: "MilestoneId");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDocuments_MilestoneId_DocumentType_IsActive",
                schema: "Contracts",
                table: "MilestoneDocuments",
                columns: new[] { "MilestoneId", "DocumentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneDocuments_UploadedDate",
                schema: "Contracts",
                table: "MilestoneDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_IsCompleted",
                schema: "Projects",
                table: "Milestones",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_IsContractual",
                schema: "Projects",
                table: "Milestones",
                column: "IsContractual");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_IsCritical",
                schema: "Projects",
                table: "Milestones",
                column: "IsCritical");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_IsDeleted",
                schema: "Projects",
                table: "Milestones",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_MilestoneCode",
                schema: "Projects",
                table: "Milestones",
                column: "MilestoneCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_PhaseId",
                schema: "Projects",
                table: "Milestones",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_PlannedDate",
                schema: "Projects",
                table: "Milestones",
                column: "PlannedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_ProjectId",
                schema: "Projects",
                table: "Milestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_ProjectId_MilestoneCode",
                schema: "Projects",
                table: "Milestones",
                columns: new[] { "ProjectId", "MilestoneCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_ProjectId_PlannedDate",
                schema: "Projects",
                table: "Milestones",
                columns: new[] { "ProjectId", "PlannedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_ProjectId_Type",
                schema: "Projects",
                table: "Milestones",
                columns: new[] { "ProjectId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Type",
                schema: "Projects",
                table: "Milestones",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_WorkPackageId",
                schema: "Projects",
                table: "Milestones",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CompanyId",
                schema: "UI",
                table: "Notifications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                schema: "UI",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ExpiresAt",
                schema: "UI",
                table: "Notifications",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsDeleted_Status",
                schema: "UI",
                table: "Notifications",
                columns: new[] { "IsDeleted", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Priority",
                schema: "UI",
                table: "Notifications",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ProjectId",
                schema: "UI",
                table: "Notifications",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                schema: "UI",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                schema: "UI",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_User_Status_Date",
                schema: "UI",
                table: "Notifications",
                columns: new[] { "UserId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "UI",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsImportant",
                schema: "UI",
                table: "Notifications",
                columns: new[] { "UserId", "IsImportant" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Status",
                schema: "UI",
                table: "Notifications",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodeMembers_UserId",
                schema: "Organization",
                table: "OBSNodeMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_Code",
                schema: "Organization",
                table: "OBSNodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_HierarchyPath",
                schema: "Organization",
                table: "OBSNodes",
                column: "HierarchyPath");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_IsActive",
                schema: "Organization",
                table: "OBSNodes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_ManagerId",
                schema: "Organization",
                table: "OBSNodes",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_NodeType",
                schema: "Organization",
                table: "OBSNodes",
                column: "NodeType");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_ParentId",
                schema: "Organization",
                table: "OBSNodes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_ProjectId",
                schema: "Organization",
                table: "OBSNodes",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_ProjectId_Code",
                schema: "Organization",
                table: "OBSNodes",
                columns: new[] { "ProjectId", "Code" },
                unique: true,
                filter: "[ProjectId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OBSNodes_ProjectId_IsActive",
                schema: "Organization",
                table: "OBSNodes",
                columns: new[] { "ProjectId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CompanyId_Code",
                schema: "Organization",
                table: "Operations",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_IsDeleted",
                schema: "Organization",
                table: "Operations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDisciplines_DisciplineId",
                schema: "Organization",
                table: "PackageDisciplines",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDisciplines_IsLeadDiscipline",
                schema: "Organization",
                table: "PackageDisciplines",
                column: "IsLeadDiscipline");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDisciplines_LeadEngineerId",
                schema: "Organization",
                table: "PackageDisciplines",
                column: "LeadEngineerId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDisciplines_PackageId",
                schema: "Organization",
                table: "PackageDisciplines",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageDisciplines_PackageId_DisciplineId",
                schema: "Organization",
                table: "PackageDisciplines",
                columns: new[] { "PackageId", "DisciplineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_Code",
                schema: "Organization",
                table: "Packages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ContractorId",
                schema: "Organization",
                table: "Packages",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_IsDeleted",
                schema: "Organization",
                table: "Packages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PackageType",
                schema: "Organization",
                table: "Packages",
                column: "PackageType");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PhaseId",
                schema: "Organization",
                table: "Packages",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PlannedEndDate",
                schema: "Organization",
                table: "Packages",
                column: "PlannedEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_PlannedStartDate",
                schema: "Organization",
                table: "Packages",
                column: "PlannedStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ProgressPercentage",
                schema: "Organization",
                table: "Packages",
                column: "ProgressPercentage");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_WBSCode",
                schema: "Organization",
                table: "Packages",
                column: "WBSCode");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_WBSElementId",
                schema: "Organization",
                table: "Packages",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                schema: "Security",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IsDeleted",
                schema: "Security",
                table: "Permissions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module_Resource",
                schema: "Security",
                table: "Permissions",
                columns: new[] { "Module", "Resource" });

            migrationBuilder.CreateIndex(
                name: "IX_Phases_IsActive_PlannedStartDate_PlannedEndDate",
                schema: "Organization",
                table: "Phases",
                columns: new[] { "IsActive", "PlannedStartDate", "PlannedEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Phases_IsDeleted",
                schema: "Organization",
                table: "Phases",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_PhaseType",
                schema: "Organization",
                table: "Phases",
                column: "PhaseType");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_ProjectId",
                schema: "Organization",
                table: "Phases",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_ProjectId_Code",
                schema: "Organization",
                table: "Phases",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_Code",
                schema: "Projects",
                table: "PlanningPackages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_ControlAccountId",
                schema: "Projects",
                table: "PlanningPackages",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_ControlAccountId_IsConverted",
                schema: "Projects",
                table: "PlanningPackages",
                columns: new[] { "ControlAccountId", "IsConverted" });

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_IsActive",
                schema: "Projects",
                table: "PlanningPackages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_IsConverted",
                schema: "Projects",
                table: "PlanningPackages",
                column: "IsConverted");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_IsDeleted",
                schema: "Projects",
                table: "PlanningPackages",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_PhaseId",
                schema: "Projects",
                table: "PlanningPackages",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_PlannedConversionDate",
                schema: "Projects",
                table: "PlanningPackages",
                column: "PlannedConversionDate");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_Priority",
                schema: "Projects",
                table: "PlanningPackages",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_ProjectId",
                schema: "Projects",
                table: "PlanningPackages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_ProjectId_Code",
                schema: "Projects",
                table: "PlanningPackages",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_Status",
                schema: "Projects",
                table: "PlanningPackages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PlanningPackages_Status_PlannedConversionDate",
                schema: "Projects",
                table: "PlanningPackages",
                columns: new[] { "Status", "PlannedConversionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CurrencyCode",
                schema: "Organization",
                table: "Projects",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsActive_PlannedStartDate_PlannedEndDate",
                schema: "Organization",
                table: "Projects",
                columns: new[] { "IsActive", "PlannedStartDate", "PlannedEndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsDeleted",
                schema: "Organization",
                table: "Projects",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OperationId",
                schema: "Organization",
                table: "Projects",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OperationId_Code",
                schema: "Organization",
                table: "Projects",
                columns: new[] { "OperationId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectManagerId",
                schema: "Organization",
                table: "Projects",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTypeId",
                schema: "Organization",
                table: "Projects",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status",
                schema: "Organization",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_IsActive_StartDate_EndDate",
                schema: "Security",
                table: "ProjectTeamMembers",
                columns: new[] { "IsActive", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_ProjectId",
                schema: "Security",
                table: "ProjectTeamMembers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_Role",
                schema: "Security",
                table: "ProjectTeamMembers",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_UserId",
                schema: "Security",
                table: "ProjectTeamMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTeamMembers_UserId_ProjectId",
                schema: "Security",
                table: "ProjectTeamMembers",
                columns: new[] { "UserId", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypes_Code",
                schema: "Configuration",
                table: "ProjectTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypes_IsActive",
                schema: "Configuration",
                table: "ProjectTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypes_IsActive_IsDeleted",
                schema: "Configuration",
                table: "ProjectTypes",
                columns: new[] { "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypes_IsDeleted",
                schema: "Configuration",
                table: "ProjectTypes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RAMAssignments_ControlAccountId",
                schema: "Organization",
                table: "RAMAssignments",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RAMAssignments_OBSNodeId",
                schema: "Organization",
                table: "RAMAssignments",
                column: "OBSNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RAMAssignments_ProjectId",
                schema: "Organization",
                table: "RAMAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RAMAssignments_ProjectId_WBSElementId_OBSNodeId_ResponsibilityType",
                schema: "Organization",
                table: "RAMAssignments",
                columns: new[] { "ProjectId", "WBSElementId", "OBSNodeId", "ResponsibilityType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RAMAssignments_ResponsibilityType",
                schema: "Organization",
                table: "RAMAssignments",
                column: "ResponsibilityType");

            migrationBuilder.CreateIndex(
                name: "IX_RAMAssignments_WBSElementId",
                schema: "Organization",
                table: "RAMAssignments",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ActivityId",
                schema: "Projects",
                table: "Resources",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ActivityId_Type",
                schema: "Projects",
                table: "Resources",
                columns: new[] { "ActivityId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ContractorId",
                schema: "Projects",
                table: "Resources",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_EquipmentId",
                schema: "Projects",
                table: "Resources",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_IsActive",
                schema: "Projects",
                table: "Resources",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_IsDeleted",
                schema: "Projects",
                table: "Resources",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_IsOverAllocated",
                schema: "Projects",
                table: "Resources",
                column: "IsOverAllocated");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ResourceCode",
                schema: "Projects",
                table: "Resources",
                column: "ResourceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_Type",
                schema: "Projects",
                table: "Resources",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UserId",
                schema: "Projects",
                table: "Resources",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_UserId_IsActive",
                schema: "Projects",
                table: "Resources",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_RiskResponses_DueDate",
                schema: "Risks",
                table: "RiskResponses",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_RiskResponses_OwnerId",
                schema: "Risks",
                table: "RiskResponses",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskResponses_RiskId",
                schema: "Risks",
                table: "RiskResponses",
                column: "RiskId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskResponses_RiskId_Status",
                schema: "Risks",
                table: "RiskResponses",
                columns: new[] { "RiskId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RiskResponses_Status",
                schema: "Risks",
                table: "RiskResponses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RiskReviews_ReviewDate",
                schema: "Risks",
                table: "RiskReviews",
                column: "ReviewDate");

            migrationBuilder.CreateIndex(
                name: "IX_RiskReviews_ReviewedById",
                schema: "Risks",
                table: "RiskReviews",
                column: "ReviewedById");

            migrationBuilder.CreateIndex(
                name: "IX_RiskReviews_RiskId",
                schema: "Risks",
                table: "RiskReviews",
                column: "RiskId");

            migrationBuilder.CreateIndex(
                name: "IX_RiskReviews_RiskId_ReviewDate",
                schema: "Risks",
                table: "RiskReviews",
                columns: new[] { "RiskId", "ReviewDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Risks_Category",
                schema: "Risks",
                table: "Risks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_Code",
                schema: "Risks",
                table: "Risks",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Risks_IdentifiedById",
                schema: "Risks",
                table: "Risks",
                column: "IdentifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_IdentifiedDate",
                schema: "Risks",
                table: "Risks",
                column: "IdentifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_ProjectId",
                schema: "Risks",
                table: "Risks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_ProjectId_Status",
                schema: "Risks",
                table: "Risks",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Risks_ProjectId_Type",
                schema: "Risks",
                table: "Risks",
                columns: new[] { "ProjectId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Risks_ResponseOwnerId",
                schema: "Risks",
                table: "Risks",
                column: "ResponseOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_Status",
                schema: "Risks",
                table: "Risks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Risks_Type",
                schema: "Risks",
                table: "Risks",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_DataDate",
                schema: "Projects",
                table: "ScheduleVersions",
                column: "DataDate");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_IsBaseline",
                schema: "Projects",
                table: "ScheduleVersions",
                column: "IsBaseline");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_ProjectId",
                schema: "Projects",
                table: "ScheduleVersions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_ProjectId_IsBaseline",
                schema: "Projects",
                table: "ScheduleVersions",
                columns: new[] { "ProjectId", "IsBaseline" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_ProjectId_Version",
                schema: "Projects",
                table: "ScheduleVersions",
                columns: new[] { "ProjectId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_Status",
                schema: "Projects",
                table: "ScheduleVersions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleVersions_Version",
                schema: "Projects",
                table: "ScheduleVersions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_SystemParameters_Category",
                schema: "Configuration",
                table: "SystemParameters",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_SystemParameters_Category_Key",
                schema: "Configuration",
                table: "SystemParameters",
                columns: new[] { "Category", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemParameters_IsPublic",
                schema: "Configuration",
                table: "SystemParameters",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_SystemParameters_IsPublic_Category",
                schema: "Configuration",
                table: "SystemParameters",
                columns: new[] { "IsPublic", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemParameters_IsSystem",
                schema: "Configuration",
                table: "SystemParameters",
                column: "IsSystem");

            migrationBuilder.CreateIndex(
                name: "IX_TimePhasedBudgets_ControlAccountId",
                schema: "Cost",
                table: "TimePhasedBudgets",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TimePhasedBudgets_ControlAccountId_PeriodStart",
                schema: "Cost",
                table: "TimePhasedBudgets",
                columns: new[] { "ControlAccountId", "PeriodStart" });

            migrationBuilder.CreateIndex(
                name: "IX_TimePhasedBudgets_PeriodEnd",
                schema: "Cost",
                table: "TimePhasedBudgets",
                column: "PeriodEnd");

            migrationBuilder.CreateIndex(
                name: "IX_TimePhasedBudgets_PeriodStart",
                schema: "Cost",
                table: "TimePhasedBudgets",
                column: "PeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_TimePhasedBudgets_PeriodType",
                schema: "Cost",
                table: "TimePhasedBudgets",
                column: "PeriodType");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalAttachments_BlobName",
                schema: "Documents",
                table: "TransmittalAttachments",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalAttachments_TransmittalId",
                schema: "Documents",
                table: "TransmittalAttachments",
                column: "TransmittalId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalAttachments_UploadedById",
                schema: "Documents",
                table: "TransmittalAttachments",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalAttachments_UploadedDate",
                schema: "Documents",
                table: "TransmittalAttachments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_AddedById",
                schema: "Documents",
                table: "TransmittalDocuments",
                column: "AddedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_DocumentId",
                schema: "Documents",
                table: "TransmittalDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_DocumentVersionId",
                schema: "Documents",
                table: "TransmittalDocuments",
                column: "DocumentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_IsIncluded",
                schema: "Documents",
                table: "TransmittalDocuments",
                column: "IsIncluded");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_SortOrder",
                schema: "Documents",
                table: "TransmittalDocuments",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_TransmittalId",
                schema: "Documents",
                table: "TransmittalDocuments",
                column: "TransmittalId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalDocuments_TransmittalId_DocumentId_DocumentVersionId",
                schema: "Documents",
                table: "TransmittalDocuments",
                columns: new[] { "TransmittalId", "DocumentId", "DocumentVersionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_CompanyId",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_Email",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_IsAcknowledged",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "IsAcknowledged");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_IsDelivered",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "IsDelivered");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_RequiresAcknowledgment",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "RequiresAcknowledgment");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_TransmittalId",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "TransmittalId");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_TransmittalId_Type",
                schema: "Documents",
                table: "TransmittalRecipients",
                columns: new[] { "TransmittalId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_Type",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_TransmittalRecipients_UserId",
                schema: "Documents",
                table: "TransmittalRecipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_ApprovedById",
                schema: "Documents",
                table: "Transmittals",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_FromCompanyId",
                schema: "Documents",
                table: "Transmittals",
                column: "FromCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_FromCompanyId_TransmittalDate",
                schema: "Documents",
                table: "Transmittals",
                columns: new[] { "FromCompanyId", "TransmittalDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_IsDeleted",
                schema: "Documents",
                table: "Transmittals",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_PreparedById",
                schema: "Documents",
                table: "Transmittals",
                column: "PreparedById");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_Priority",
                schema: "Documents",
                table: "Transmittals",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_ProjectId",
                schema: "Documents",
                table: "Transmittals",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_ProjectId_Status",
                schema: "Documents",
                table: "Transmittals",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_SentById",
                schema: "Documents",
                table: "Transmittals",
                column: "SentById");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_Status",
                schema: "Documents",
                table: "Transmittals",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_ToCompanyId",
                schema: "Documents",
                table: "Transmittals",
                column: "ToCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_TransmittalDate",
                schema: "Documents",
                table: "Transmittals",
                column: "TransmittalDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transmittals_TransmittalNumber",
                schema: "Documents",
                table: "Transmittals",
                column: "TransmittalNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrendAttachments_TrendId",
                schema: "Change",
                table: "TrendAttachments",
                column: "TrendId");

            migrationBuilder.CreateIndex(
                name: "IX_TrendAttachments_UploadedBy",
                schema: "Change",
                table: "TrendAttachments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TrendAttachments_UploadedDate",
                schema: "Change",
                table: "TrendAttachments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_TrendComments_CommentDate",
                schema: "Change",
                table: "TrendComments",
                column: "CommentDate");

            migrationBuilder.CreateIndex(
                name: "IX_TrendComments_TrendId",
                schema: "Change",
                table: "TrendComments",
                column: "TrendId");

            migrationBuilder.CreateIndex(
                name: "IX_TrendComments_UserId",
                schema: "Change",
                table: "TrendComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_Category",
                schema: "Change",
                table: "Trends",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_ChangeOrderId",
                schema: "Change",
                table: "Trends",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_Code",
                schema: "Change",
                table: "Trends",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trends_DecisionByUserId",
                schema: "Change",
                table: "Trends",
                column: "DecisionByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_DueDate",
                schema: "Change",
                table: "Trends",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_IdentifiedDate",
                schema: "Change",
                table: "Trends",
                column: "IdentifiedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_IsDeleted",
                schema: "Change",
                table: "Trends",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_Priority",
                schema: "Change",
                table: "Trends",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_ProjectId",
                schema: "Change",
                table: "Trends",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_ProjectId_Status",
                schema: "Change",
                table: "Trends",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Trends_RaisedByUserId",
                schema: "Change",
                table: "Trends",
                column: "RaisedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_Status",
                schema: "Change",
                table: "Trends",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_Type",
                schema: "Change",
                table: "Trends",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Trends_WBSElementId",
                schema: "Change",
                table: "Trends",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_IsActive_GrantedAt_ExpiresAt",
                schema: "Security",
                table: "UserProjectPermissions",
                columns: new[] { "IsActive", "GrantedAt", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_PermissionId",
                schema: "Security",
                table: "UserProjectPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_ProjectId",
                schema: "Security",
                table: "UserProjectPermissions",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_UserId",
                schema: "Security",
                table: "UserProjectPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectPermissions_UserId_ProjectId_PermissionCode",
                schema: "Security",
                table: "UserProjectPermissions",
                columns: new[] { "UserId", "ProjectId", "PermissionCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Security",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EntraId",
                schema: "Security",
                table: "Users",
                column: "EntraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                schema: "Security",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted_IsActive",
                schema: "Security",
                table: "Users",
                columns: new[] { "IsDeleted", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_LastActivityAt",
                schema: "Security",
                table: "UserSessions",
                column: "LastActivityAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_SessionId",
                schema: "Security",
                table: "UserSessions",
                column: "SessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_StartedAt",
                schema: "Security",
                table: "UserSessions",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                schema: "Security",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_IsActive",
                schema: "Security",
                table: "UserSessions",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_DocumentId",
                schema: "Contracts",
                table: "ValuationDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_DocumentType",
                schema: "Contracts",
                table: "ValuationDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_IsActive",
                schema: "Contracts",
                table: "ValuationDocuments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_UploadedDate",
                schema: "Contracts",
                table: "ValuationDocuments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_ValuationId",
                schema: "Contracts",
                table: "ValuationDocuments",
                column: "ValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationDocuments_ValuationId_DocumentType_IsActive",
                schema: "Contracts",
                table: "ValuationDocuments",
                columns: new[] { "ValuationId", "DocumentType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_ValuationItems_IsActive",
                schema: "Contracts",
                table: "ValuationItems",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationItems_ValuationId",
                schema: "Contracts",
                table: "ValuationItems",
                column: "ValuationId");

            migrationBuilder.CreateIndex(
                name: "IX_ValuationItems_ValuationId_ItemCode",
                schema: "Contracts",
                table: "ValuationItems",
                columns: new[] { "ValuationId", "ItemCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ValuationItems_WorkPackageCode",
                schema: "Contracts",
                table: "ValuationItems",
                column: "WorkPackageCode");

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_ContractId",
                schema: "Contracts",
                table: "Valuations",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_ContractId_Status",
                schema: "Contracts",
                table: "Valuations",
                columns: new[] { "ContractId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_ContractId_ValuationPeriod",
                schema: "Contracts",
                table: "Valuations",
                columns: new[] { "ContractId", "ValuationPeriod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_IsActive",
                schema: "Contracts",
                table: "Valuations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_Status",
                schema: "Contracts",
                table: "Valuations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_SubmissionDate",
                schema: "Contracts",
                table: "Valuations",
                column: "SubmissionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_ValuationDate",
                schema: "Contracts",
                table: "Valuations",
                column: "ValuationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Valuations_ValuationNumber",
                schema: "Contracts",
                table: "Valuations",
                column: "ValuationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariationAttachments_Category",
                schema: "Change",
                table: "VariationAttachments",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_VariationAttachments_UploadedBy",
                schema: "Change",
                table: "VariationAttachments",
                column: "UploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VariationAttachments_UploadedDate",
                schema: "Change",
                table: "VariationAttachments",
                column: "UploadedDate");

            migrationBuilder.CreateIndex(
                name: "IX_VariationAttachments_VariationId",
                schema: "Change",
                table: "VariationAttachments",
                column: "VariationId");

            migrationBuilder.CreateIndex(
                name: "IX_VariationItems_ItemCode",
                schema: "Change",
                table: "VariationItems",
                column: "ItemCode");

            migrationBuilder.CreateIndex(
                name: "IX_VariationItems_VariationId",
                schema: "Change",
                table: "VariationItems",
                column: "VariationId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ApprovedByUserId",
                schema: "Change",
                table: "Variations",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_Category",
                schema: "Change",
                table: "Variations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ChangeOrderId",
                schema: "Change",
                table: "Variations",
                column: "ChangeOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_Code",
                schema: "Change",
                table: "Variations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ContractorId",
                schema: "Change",
                table: "Variations",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_IsDeleted",
                schema: "Change",
                table: "Variations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_IssuedDate",
                schema: "Change",
                table: "Variations",
                column: "IssuedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ProjectId",
                schema: "Change",
                table: "Variations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ProjectId_Status",
                schema: "Change",
                table: "Variations",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Variations_RequestedByUserId",
                schema: "Change",
                table: "Variations",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_ReviewedByUserId",
                schema: "Change",
                table: "Variations",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_Status",
                schema: "Change",
                table: "Variations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_TrendId",
                schema: "Change",
                table: "Variations",
                column: "TrendId");

            migrationBuilder.CreateIndex(
                name: "IX_Variations_Type",
                schema: "Change",
                table: "Variations",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_WBSCBSMappings_AllocationPercentage",
                schema: "Cost",
                table: "WBSCBSMappings",
                column: "AllocationPercentage");

            migrationBuilder.CreateIndex(
                name: "IX_WBSCBSMappings_CBSId",
                schema: "Cost",
                table: "WBSCBSMappings",
                column: "CBSId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSCBSMappings_WBSElementId",
                schema: "Cost",
                table: "WBSCBSMappings",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSCBSMappings_WBSElementId_CBSId",
                schema: "Cost",
                table: "WBSCBSMappings",
                columns: new[] { "WBSElementId", "CBSId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_IsApproved",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_Month",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "Month");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_ProgressDate",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "ProgressDate");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_ReportedBy",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "ReportedBy");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_RequiresReview",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "RequiresReview");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_WBSElementId",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "WBSElementId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_WBSElementId_ProgressDate",
                schema: "Projects",
                table: "WBSElementProgress",
                columns: new[] { "WBSElementId", "ProgressDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_WBSElementId_Year_Month",
                schema: "Projects",
                table: "WBSElementProgress",
                columns: new[] { "WBSElementId", "Year", "Month" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_Week",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "Week");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElementProgress_Year",
                schema: "Projects",
                table: "WBSElementProgress",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_Code",
                schema: "Projects",
                table: "WBSElements",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ControlAccountId",
                schema: "Projects",
                table: "WBSElements",
                column: "ControlAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ElementType",
                schema: "Projects",
                table: "WBSElements",
                column: "ElementType");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_IsActive",
                schema: "Projects",
                table: "WBSElements",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_IsDeleted",
                schema: "Projects",
                table: "WBSElements",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_Level",
                schema: "Projects",
                table: "WBSElements",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ParentId",
                schema: "Projects",
                table: "WBSElements",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_PhaseId",
                schema: "Projects",
                table: "WBSElements",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ProjectId",
                schema: "Projects",
                table: "WBSElements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ProjectId_Code",
                schema: "Projects",
                table: "WBSElements",
                columns: new[] { "ProjectId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ProjectId_ElementType",
                schema: "Projects",
                table: "WBSElements",
                columns: new[] { "ProjectId", "ElementType" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSElements_ProjectId_Level",
                schema: "Projects",
                table: "WBSElements",
                columns: new[] { "ProjectId", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_ElementType",
                schema: "Configuration",
                table: "WBSTemplateElements",
                column: "ElementType");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_HierarchyPath",
                schema: "Configuration",
                table: "WBSTemplateElements",
                column: "HierarchyPath");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_Level",
                schema: "Configuration",
                table: "WBSTemplateElements",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_ParentId",
                schema: "Configuration",
                table: "WBSTemplateElements",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_SequenceNumber",
                schema: "Configuration",
                table: "WBSTemplateElements",
                column: "SequenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_WBSTemplateId",
                schema: "Configuration",
                table: "WBSTemplateElements",
                column: "WBSTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_WBSTemplateId_Code",
                schema: "Configuration",
                table: "WBSTemplateElements",
                columns: new[] { "WBSTemplateId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplateElements_WBSTemplateId_Level",
                schema: "Configuration",
                table: "WBSTemplateElements",
                columns: new[] { "WBSTemplateId", "Level" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_Code",
                schema: "Configuration",
                table: "WBSTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_IndustryType",
                schema: "Configuration",
                table: "WBSTemplates",
                column: "IndustryType");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_IndustryType_ProjectType",
                schema: "Configuration",
                table: "WBSTemplates",
                columns: new[] { "IndustryType", "ProjectType" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_IsActive",
                schema: "Configuration",
                table: "WBSTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_IsActive_IsPublic_IsDeleted",
                schema: "Configuration",
                table: "WBSTemplates",
                columns: new[] { "IsActive", "IsPublic", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_IsDeleted",
                schema: "Configuration",
                table: "WBSTemplates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_IsPublic",
                schema: "Configuration",
                table: "WBSTemplates",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_WBSTemplates_ProjectType",
                schema: "Configuration",
                table: "WBSTemplates",
                column: "ProjectType");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_IsBaselined",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "IsBaselined");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_IsCriticalPath",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "IsCriticalPath");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_PlannedEndDate",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "PlannedEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_PlannedStartDate",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "PlannedStartDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_PrimaryDisciplineId",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "PrimaryDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_ResponsibleUserId",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_ResponsibleUserId_Status",
                schema: "Projects",
                table: "WorkPackageDetails",
                columns: new[] { "ResponsibleUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_Status",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_Status_PlannedStartDate",
                schema: "Projects",
                table: "WorkPackageDetails",
                columns: new[] { "Status", "PlannedStartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageDetails_WBSElementId",
                schema: "Projects",
                table: "WorkPackageDetails",
                column: "WBSElementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_IsApproved",
                schema: "Projects",
                table: "WorkPackageProgress",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_ProgressDate",
                schema: "Projects",
                table: "WorkPackageProgress",
                column: "ProgressDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_ProgressPeriod",
                schema: "Projects",
                table: "WorkPackageProgress",
                column: "ProgressPeriod");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_WorkPackageId",
                schema: "Projects",
                table: "WorkPackageProgress",
                column: "WorkPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_WorkPackageId_ProgressDate",
                schema: "Projects",
                table: "WorkPackageProgress",
                columns: new[] { "WorkPackageId", "ProgressDate" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_WorkPackageId_Year_ProgressPeriod",
                schema: "Projects",
                table: "WorkPackageProgress",
                columns: new[] { "WorkPackageId", "Year", "ProgressPeriod" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkPackageProgress_Year",
                schema: "Projects",
                table: "WorkPackageProgress",
                column: "Year");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentAttachments_DocumentComments_CommentId",
                schema: "Documents",
                table: "CommentAttachments",
                column: "CommentId",
                principalSchema: "Documents",
                principalTable: "DocumentComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentComments_DocumentVersions_DocumentVersionId",
                schema: "Documents",
                table: "DocumentComments",
                column: "DocumentVersionId",
                principalSchema: "Documents",
                principalTable: "DocumentVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentComments_Documents_DocumentId",
                schema: "Documents",
                table: "DocumentComments",
                column: "DocumentId",
                principalSchema: "Documents",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentDistributions_DocumentVersions_DocumentVersionId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "DocumentVersionId",
                principalSchema: "Documents",
                principalTable: "DocumentVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentDistributions_Documents_DocumentId",
                schema: "Documents",
                table: "DocumentDistributions",
                column: "DocumentId",
                principalSchema: "Documents",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentPermissions_Documents_DocumentId",
                schema: "Documents",
                table: "DocumentPermissions",
                column: "DocumentId",
                principalSchema: "Documents",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentRelationships_Documents_DocumentId",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "DocumentId",
                principalSchema: "Documents",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentRelationships_Documents_RelatedDocumentId",
                schema: "Documents",
                table: "DocumentRelationships",
                column: "RelatedDocumentId",
                principalSchema: "Documents",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentVersions_CurrentVersionId",
                schema: "Documents",
                table: "Documents",
                column: "CurrentVersionId",
                principalSchema: "Documents",
                principalTable: "DocumentVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ControlAccounts_Phases_PhaseId",
                schema: "Cost",
                table: "ControlAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Phases_PhaseId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Phases_PhaseId",
                schema: "Organization",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_WBSElements_Phases_PhaseId",
                schema: "Projects",
                table: "WBSElements");

            migrationBuilder.DropForeignKey(
                name: "FK_ControlAccounts_Projects_ProjectId",
                schema: "Cost",
                table: "ControlAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Projects_ProjectId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_WBSElements_Projects_ProjectId",
                schema: "Projects",
                table: "WBSElements");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_WBSElements_WBSElementId",
                schema: "Organization",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Packages_PackageId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_AuthorId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_LockedById",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_ReviewedById",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_Users_ReviewedById",
                schema: "Documents",
                table: "DocumentVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentVersions_Users_UploadedById",
                schema: "Documents",
                table: "DocumentVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentVersions_CurrentVersionId",
                schema: "Documents",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "AccountCodes",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "ActualCosts",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "BudgetRevisions",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "CBSBudgetItems",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "CBSTemplateElements",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "ChangeOrderApprovals",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ChangeOrderDocuments",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ChangeOrderImpacts",
                schema: "Risks");

            migrationBuilder.DropTable(
                name: "ChangeOrderMilestones",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ChangeOrderRelations",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ChangeRequestApprovals",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ChangeRequestAttachments",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ChangeRequestComments",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ClaimChangeOrders",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ClaimDocuments",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ClaimRelations",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "CommentAttachments",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "CommitmentRevisions",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "CommitmentWorkPackages",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "ContractDocuments",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ControlAccountAssignments",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "CostControlReportItems",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "DocumentDistributions",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentPermissions",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "DocumentRelationships",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "EVMRecords",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "ExchangeRates",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "InvoiceItems",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "MilestoneDependencies",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "MilestoneDocuments",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "Milestones",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "UI");

            migrationBuilder.DropTable(
                name: "OBSNodeMembers",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "PackageDisciplines",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "PlanningPackages",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectTeamMembers",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "RAMAssignments",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "RiskResponses",
                schema: "Risks");

            migrationBuilder.DropTable(
                name: "RiskReviews",
                schema: "Risks");

            migrationBuilder.DropTable(
                name: "SystemParameters",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "TimePhasedBudgets",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "TransmittalAttachments",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "TransmittalDocuments",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "TransmittalRecipients",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "TrendAttachments",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "TrendComments",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "UserProjectPermissions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "UserSessions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "ValuationDocuments",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "ValuationItems",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "VariationAttachments",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "VariationItems",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "WBSCBSMappings",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "WBSElementProgress",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "WBSTemplateElements",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "WorkPackageProgress",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "CostItems",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "CBSTemplates",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "ChangeRequests",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ContractChangeOrders",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "Claims",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "DocumentComments",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "CostControlReports",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "CommitmentItems",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "ContractMilestones",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "OBSNodes",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Activities",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "Equipment",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "Risks",
                schema: "Risks");

            migrationBuilder.DropTable(
                name: "Transmittals",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "Valuations",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "WBSTemplates",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "CBS",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "Variations",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ScheduleVersions",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "WorkPackageDetails",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "Contracts",
                schema: "Contracts");

            migrationBuilder.DropTable(
                name: "Trends",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "ChangeOrders",
                schema: "Change");

            migrationBuilder.DropTable(
                name: "Commitments",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "BudgetItems",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "Budgets",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "Phases",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Currencies",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Operations",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "ProjectTypes",
                schema: "Configuration");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "WBSElements",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ControlAccounts",
                schema: "Cost");

            migrationBuilder.DropTable(
                name: "Packages",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Contractors",
                schema: "Organization");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "DocumentVersions",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "Documents");

            migrationBuilder.DropTable(
                name: "Disciplines",
                schema: "Organization");
        }
    }
}
