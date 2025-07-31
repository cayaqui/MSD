using Application.Interfaces.Setup;
using Carter;
using Carter.ModelBinding;
using Core.DTOs.Companies;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Modules;

public class CompaniesModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/companies")
            .WithTags("Companies")
            .RequireAuthorization();

        // Get all companies
        group.MapGet("", GetCompanies)
            .WithName("GetCompanies")
            .WithSummary("Get all companies")
            .Produces<IEnumerable<CompanyDto>>(200);

        // Get company by ID
        group.MapGet("{id:guid}", GetCompanyById)
            .WithName("GetCompanyById")
            .WithSummary("Get company by ID")
            .Produces<CompanyDto>(200)
            .Produces(404);

        // Get company with operations
        group.MapGet("{id:guid}/with-operations", GetCompanyWithOperations)
            .WithName("GetCompanyWithOperations")
            .WithSummary("Get company with operations summary")
            .Produces<CompanyWithOperationsDto>(200)
            .Produces(404);

        // Create company
        group.MapPost("", CreateCompany)
            .WithName("CreateCompany")
            .WithSummary("Create a new company")
            .Produces<CompanyDto>(201)
            .Produces<ValidationProblemDetails>(400);

        // Update company
        group.MapPut("{id:guid}", UpdateCompany)
            .WithName("UpdateCompany")
            .WithSummary("Update company")
            .Produces<CompanyDto>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(404);

        // Delete company
        group.MapDelete("{id:guid}", DeleteCompany)
            .WithName("DeleteCompany")
            .WithSummary("Delete company")
            .Produces(204)
            .Produces(404);

        // Check if code is unique
        group.MapGet("check-code/{code}", CheckCompanyCode)
            .WithName("CheckCompanyCode")
            .WithSummary("Check if company code is unique")
            .Produces<bool>(200);

        // Logo endpoints
        group.MapPost("{id:guid}/logo", UploadCompanyLogo)
            .WithName("UploadCompanyLogo")
            .WithSummary("Upload company logo")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .DisableAntiforgery();

        group.MapGet("{id:guid}/logo", GetCompanyLogo)
            .WithName("GetCompanyLogo")
            .WithSummary("Get company logo")
            .Produces(200)
            .Produces(404);

        group.MapDelete("{id:guid}/logo", DeleteCompanyLogo)
            .WithName("DeleteCompanyLogo")
            .WithSummary("Delete company logo")
            .Produces(204)
            .Produces(404);
    }

    private static async Task<Ok<IEnumerable<CompanyDto>>> GetCompanies(
        ICompanyService companyService)
    {
        var companies = await companyService.GetAllAsync();
        return TypedResults.Ok(companies);
    }

    private static async Task<Results<Ok<CompanyDto>, NotFound>> GetCompanyById(
        Guid id,
        ICompanyService companyService)
    {
        var company = await companyService.GetByIdAsync(id);
        return company != null
            ? TypedResults.Ok(company)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<CompanyWithOperationsDto>, NotFound>> GetCompanyWithOperations(
        Guid id,
        ICompanyService companyService)
    {
        var company = await companyService.GetWithOperationsAsync(id) as CompanyWithOperationsDto;
        return company != null
            ? TypedResults.Ok(company)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Created<CompanyDto>, ValidationProblem>> CreateCompany(
        CreateCompanyDto dto,
        ICompanyService companyService,
        IValidator<CreateCompanyDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var company = await companyService.CreateAsync(dto);
        return TypedResults.Created($"/api/companies/{company.Id}", company);
    }

    private static async Task<Results<Ok<CompanyDto>, NotFound, ValidationProblem>> UpdateCompany(
        Guid id,
        UpdateCompanyDto dto,
        ICompanyService companyService,
        IValidator<UpdateCompanyDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var company = await companyService.UpdateAsync(id, dto);
        return company != null
            ? TypedResults.Ok(company)
            : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCompany(
        Guid id,
        ICompanyService companyService)
    {
        await companyService.DeleteAsync(id);
        return TypedResults.NoContent();
    }

    private static async Task<Ok<bool>> CheckCompanyCode(
        string code,
        Guid? excludeId,
        ICompanyService companyService)
    {
        var isUnique = await companyService.IsCodeUniqueAsync(code, excludeId);
        return TypedResults.Ok(isUnique);
    }

    private static async Task<Results<Ok, BadRequest<string>, NotFound>> UploadCompanyLogo(
        Guid id,
        IFormFile file,
        ICompanyService companyService)
    {
        if (file == null || file.Length == 0)
        {
            return TypedResults.BadRequest("No file uploaded");
        }

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();

        await companyService.UpdateLogoAsync(id, fileBytes, file.ContentType);
        return TypedResults.Ok();
    }

    private static async Task<Results<FileContentHttpResult, NotFound>> GetCompanyLogo(
        Guid id,
        ICompanyService companyService)
    {
        var logo = await companyService.GetLogoAsync(id);

        if (logo == null)
        {
            return TypedResults.NotFound();
        }

        // Determine content type from the stored logo
        // In a real implementation, you'd store the content type with the logo
        return TypedResults.File(logo, "image/png");
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCompanyLogo(
        Guid id,
        ICompanyService companyService)
    {
        await companyService.DeleteLogoAsync(id);
        return TypedResults.NoContent();
    }
}
