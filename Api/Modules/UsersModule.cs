using Carter;
using Core.DTOs.Auth;
using Core.DTOs.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Domain.Entities.Security;
using FluentValidation;
using ValidationProblemDetails = Core.DTOs.Common.ValidationProblemDetails;
using Application.Interfaces.Auth;

namespace API.Modules;

public class UsersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        // Get all users with pagination
        group.MapGet("", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get all users with pagination")
            .Produces<PagedResult<UserDto>>(200);

        // Search users
        group.MapPost("search", SearchUsers)
            .WithName("SearchUsers")
            .WithSummary("Search users with filters")
            .Produces<PagedResult<UserDto>>(200);

        // Get user by ID
        group.MapGet("{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<UserDto>(200)
            .Produces(404);

        // Get user by email
        group.MapGet("by-email/{email}", GetUserByEmail)
            .WithName("GetUserByEmail")
            .WithSummary("Get user by email")
            .Produces<UserDto>(200)
            .Produces(404);

        // Create user (sync from Azure AD)
        group.MapPost("", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create user from Azure AD")
            .Produces<UserDto>(201)
            .Produces<ValidationProblemDetails>(400);

        // Update user
        group.MapPut("{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update user information")
            .Produces<UserDto>(200)
            .Produces<ValidationProblemDetails>(400)
            .Produces(404);

        // Activate/Deactivate user
        group.MapPatch("{id:guid}/activate", ActivateUser)
            .WithName("ActivateUser")
            .WithSummary("Activate user")
            .Produces<UserDto>(200)
            .Produces(404);

        group.MapPatch("{id:guid}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .WithSummary("Deactivate user")
            .Produces<UserDto>(200)
            .Produces(404);

        // Delete user (soft delete)
        group.MapDelete("{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete user (soft delete)")
            .Produces(204)
            .Produces(404);

        // Get user's projects
        group.MapGet("{id:guid}/projects", GetUserProjects)
            .WithName("GetUserProjects")
            .WithSummary("Get all projects for a user")
            .Produces<IEnumerable<ProjectTeamMemberDto>>(200)
            .Produces(404);

        // Sync user with Azure AD
        group.MapPost("{id:guid}/sync", SyncUserWithAzureAD)
            .WithName("SyncUserWithAzureAD")
            .WithSummary("Sync user data with Azure AD")
            .Produces<UserDto>(200)
            .Produces(404);

        // Bulk operations
        group.MapPost("bulk/activate", BulkActivateUsers)
            .WithName("BulkActivateUsers")
            .WithSummary("Activate multiple users")
            .Produces<int>(200);

        group.MapPost("bulk/deactivate", BulkDeactivateUsers)
            .WithName("BulkDeactivateUsers")
            .WithSummary("Deactivate multiple users")
            .Produces<int>(200);
    }

    private static async Task<Ok<PagedResult<UserDto>>> GetUsers(
        int pageNumber,
        int pageSize,
        IUserService userService,
        ILogger<UsersModule> logger)
    {
        pageNumber = pageNumber > 0 ? pageNumber : 1;
        pageSize = pageSize > 0 ? pageSize : 20;
        pageSize = Math.Min(pageSize, 100);

        logger.LogInformation("Getting users page {PageNumber} with size {PageSize}", pageNumber, pageSize);

        var result = await userService.GetPagedAsync(pageNumber, pageSize);
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<PagedResult<UserDto>>> SearchUsers(
        [FromBody] UserFilterDto filter,
        [FromServices] IUserService userService)
    {
        var result = await userService.SearchAsync(filter);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetUserById(
        Guid id,
        IUserService userService)
    {
        var user = await userService.GetByIdAsync(id);
        return user != null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetUserByEmail(
        string email,
        IUserService userService)
    {
        var user = await userService.GetByEmailAsync(email);
        return user != null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Created<UserDto>, ValidationProblem>> CreateUser(
        [FromBody] CreateUserDto dto,
        [FromServices] IUserService userService,
        [FromServices] IValidator<CreateUserDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await userService.CreateAsync(dto);
        return TypedResults.Created($"/api/users/{user.Id}", user);
    }

    private static async Task<Results<Ok<UserDto>, NotFound, ValidationProblem>> UpdateUser(
        Guid id,
        [FromBody] UpdateUserDto dto,
        [FromServices] IUserService userService,
        [FromServices] IValidator<UpdateUserDto> validator)
    {
        var validationResult = await validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await userService.UpdateAsync(id, dto);
        return user != null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> ActivateUser(
        Guid id,
        IUserService userService)
    {
        var user = await userService.ActivateAsync(id);
        return user != null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> DeactivateUser(
        Guid id,
        IUserService userService)
    {
        var user = await userService.DeactivateAsync(id);
        return user != null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUser(
        Guid id,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        await userService.DeleteAsync(id, currentUserService.UserId);
        return TypedResults.NoContent();
    }

    private static async Task<Results<Ok<IEnumerable<ProjectTeamMemberDto>>, NotFound>> GetUserProjects(
        Guid id,
        IUserService userService)
    {
        var projects = await userService.GetUserProjectsAsync(id);
        if (projects == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(projects);
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> SyncUserWithAzureAD(
        Guid id,
        IUserService userService,
        ILogger<UsersModule> logger)
    {
        try
        {
            var user = await userService.SyncWithAzureADAsync(id);
            return user != null
                ? TypedResults.Ok(user)
                : TypedResults.NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error syncing user {UserId} with Azure AD", id);
            throw;
        }
    }

    private static async Task<Ok<int>> BulkActivateUsers(
        [FromBody] BulkUserOperationDto dto,
        [FromServices] IUserService userService)
    {
        var count = await userService.BulkActivateAsync(dto.UserIds);
        return TypedResults.Ok(count);
    }

    private static async Task<Ok<int>> BulkDeactivateUsers(
        [FromBody] BulkUserOperationDto dto,
        [FromServices] IUserService userService)
    {
        var count = await userService.BulkDeactivateAsync(dto.UserIds);
        return TypedResults.Ok(count);
    }
}
