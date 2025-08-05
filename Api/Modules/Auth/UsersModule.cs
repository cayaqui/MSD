using Api.Authorization;
using Application.Interfaces.Auth;
using Carter;
using Core.Constants;
using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Auth.Users;
using Core.DTOs.Common;
using Microsoft.AspNetCore.Authorization;

namespace Api.Modules;

/// <summary>
/// User management endpoints
/// </summary>
public class UsersModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        // User CRUD operations
        users.MapGet("/", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get paginated list of users")
            .Produces<PagedResult<UserDto>>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapPost("/search", SearchUsers)
            .WithName("SearchUsers")
            .WithSummary("Search users with filters")
            .Produces<PagedResult<UserDto>>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<UserDto>(200)
            .Produces(404);

        users.MapGet("/by-email/{email}", GetUserByEmail)
            .WithName("GetUserByEmail")
            .WithSummary("Get user by email")
            .Produces<UserDto>(200)
            .Produces(404);

        users.MapGet("/by-entra-id/{entraId}", GetUserByEntraId)
            .WithName("GetUserByEntraId")
            .WithSummary("Get user by Entra ID")
            .Produces<UserDto>(200)
            .Produces(404);

        users.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .Produces<UserDto>(201)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update user details")
            .Produces<UserDto>(200)
            .Produces(404)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapPost("/{id:guid}/activate", ActivateUser)
            .WithName("ActivateUser")
            .WithSummary("Activate a user")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapPost("/{id:guid}/deactivate", DeactivateUser)
            .WithName("DeactivateUser")
            .WithSummary("Deactivate a user")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .Produces(204)
            .Produces(404)
            .Produces(400)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // User projects
        users.MapGet("/{id:guid}/projects", GetUserProjects)
            .WithName("GetUserProjects")
            .WithSummary("Get user's project assignments")
            .Produces<IEnumerable<ProjectTeamMemberDto>>(200)
            .Produces(404);

        // Azure AD sync
        users.MapPost("/{id:guid}/sync", SyncUserWithAzure)
            .WithName("SyncUserWithAzure")
            .WithSummary("Sync user data with Azure AD")
            .Produces<UserDto>(200)
            .Produces(404)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // User photo
        users.MapGet("/{id:guid}/photo", GetUserPhoto)
            .WithName("GetUserPhoto")
            .WithSummary("Get user photo from Azure AD")
            //.Produces<UserPhotoResponse>(200)
            .Produces(404);

        // Bulk operations
        users.MapPost("/bulk/activate", BulkActivateUsers)
            .WithName("BulkActivateUsers")
            .WithSummary("Activate multiple users")
            .Produces<BulkOperationResult>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        users.MapPost("/bulk/deactivate", BulkDeactivateUsers)
            .WithName("BulkDeactivateUsers")
            .WithSummary("Deactivate multiple users")
            .Produces<BulkOperationResult>(200)
            .RequireAuthorization(policy => policy.RequireRoleOrAdmin(ProjectRoles.ProjectManager));

        // Validation endpoints
        users.MapGet("/check-email/{email}", CheckEmailExists)
            .WithName("CheckEmailExists")
            .WithSummary("Check if email already exists")
            .Produces<bool>(200);

        users.MapGet("/{id:guid}/can-delete", CanDeleteUser)
            .WithName("CanDeleteUser")
            .WithSummary("Check if user can be deleted")
            .Produces<CanDeleteResult>(200)
            .Produces(404);
    }

    private static async Task<IResult> GetUsers(
        [AsParameters] PaginationQuery query,
        IUserService userService)
    {
        var result = await userService.GetPagedAsync(query.PageNumber, query.PageSize);
        return Results.Ok(result);
    }

    private static async Task<IResult> SearchUsers(
        UserFilterDto filter,
        IUserService userService)
    {
        var result = await userService.SearchAsync(filter);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        IUserService userService)
    {
        var user = await userService.GetByIdAsync(id);
        return user != null ? Results.Ok(user) : Results.NotFound($"User {id} not found");
    }

    private static async Task<IResult> GetUserByEmail(
        string email,
        IUserService userService)
    {
        var user = await userService.GetByEmailAsync(email);
        return user != null ? Results.Ok(user) : Results.NotFound($"User with email {email} not found");
    }

    private static async Task<IResult> GetUserByEntraId(
        string entraId,
        IUserService userService)
    {
        var user = await userService.GetByEntraIdAsync(entraId);
        return user != null ? Results.Ok(user) : Results.NotFound($"User with Entra ID {entraId} not found");
    }

    private static async Task<IResult> CreateUser(
        CreateUserDto dto,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Check if email already exists
            if (await userService.EmailExistsAsync(dto.Email))
            {
                return Results.BadRequest("A user with this email already exists");
            }

            var user = await userService.CreateAsync(dto);
            return Results.Created($"/api/users/{user.Id}", user);
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "User creation failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        UpdateUserDto dto,
        IUserService userService)
    {
        try
        {
            // Check if email is being changed and already exists
            if (!string.IsNullOrEmpty(dto.Email))
            {
                if (await userService.EmailExistsAsync(dto.Email, id))
                {
                    return Results.BadRequest("A user with this email already exists");
                }
            }

            var user = await userService.UpdateAsync(id, dto);
            return user != null ? Results.Ok(user) : Results.NotFound($"User {id} not found");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "User update failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> ActivateUser(
        Guid id,
        IUserService userService)
    {
        var user = await userService.ActivateAsync(id);
        return user != null ? Results.Ok(user) : Results.NotFound($"User {id} not found");
    }

    private static async Task<IResult> DeactivateUser(
        Guid id,
        IUserService userService)
    {
        var user = await userService.DeactivateAsync(id);
        return user != null ? Results.Ok(user) : Results.NotFound($"User {id} not found");
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        try
        {
            // Check if user can be deleted
            if (!await userService.CanUserBeDeletedAsync(id))
            {
                return Results.BadRequest("User cannot be deleted due to existing dependencies");
            }

            await userService.DeleteAsync(id, currentUserService.UserId);
            return Results.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound($"User {id} not found");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "User deletion failed",
                detail: ex.Message,
                statusCode: 400);
        }
    }

    private static async Task<IResult> GetUserProjects(
        Guid id,
        IUserService userService)
    {
        var projects = await userService.GetUserProjectsAsync(id);
        return Results.Ok(projects);
    }

    private static async Task<IResult> SyncUserWithAzure(
        Guid id,
        IUserService userService)
    {
        try
        {
            var user = await userService.SyncWithAzureADAsync(id);
            return user != null ? Results.Ok(user) : Results.NotFound($"User {id} not found");
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Azure AD sync failed",
                detail: ex.Message,
                statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserPhoto(
        Guid id,
        IUserService userService)
    {
        // This would need to be implemented in the service
        var user = await userService.GetByIdAsync(id);
        if (user == null) return Results.NotFound();
        
        // Return empty photo for now
        return Results.Ok(new { ContentType = "image/png", Data = Array.Empty<byte>() });
    }

    private static async Task<IResult> BulkActivateUsers(
        BulkUserOperationRequest request,
        IUserService userService)
    {
        var count = await userService.BulkActivateAsync(request.UserIds);
        return Results.Ok(new BulkOperationResult(count, 0));
    }

    private static async Task<IResult> BulkDeactivateUsers(
        BulkUserOperationRequest request,
        IUserService userService)
    {
        var count = await userService.BulkDeactivateAsync(request.UserIds);
        return Results.Ok(new BulkOperationResult(count, 0));
    }

    private static async Task<IResult> CheckEmailExists(
        string email,
        IUserService userService)
    {
        var exists = await userService.EmailExistsAsync(email);
        return Results.Ok(exists);
    }

    private static async Task<IResult> CanDeleteUser(
        Guid id,
        IUserService userService)
    {
        var canDelete = await userService.CanUserBeDeletedAsync(id);
        return Results.Ok(new CanDeleteResult(canDelete, 
            canDelete ? null : "User has active project assignments or other dependencies"));
    }
}

// Request/Response DTOs
