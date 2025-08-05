using Core.DTOs.Auth.ProjectTeamMembers;
using Core.DTOs.Auth.Users;
using Core.DTOs.Organization.Company;
using Core.DTOs.Organization.Operation;
using Core.DTOs.Organization.Project;
using Core.DTOs.UI.Notifications;
using Core.Enums.Projects;
using Domain.Entities.Auth.Security;
using Domain.Entities.Organization.Core;
using Domain.Entities.UI;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth Mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EntraId, opt => opt.MapFrom(src => src.EntraId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.GivenName, opt => opt.MapFrom(src => src.GivenName))
            .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.JobTitle))
            .ForMember(dest => dest.PreferredLanguage, opt => opt.MapFrom(src => src.PreferredLanguage))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt))
            .ForMember(dest => dest.LoginCount, opt => opt.MapFrom(src => src.LoginCount))
            .ForMember(dest => dest.ProjectTeamMembers, opt => opt.MapFrom(src => src.ProjectTeamMembers));

        CreateMap<CreateUserDto, User>()
            .ConstructUsing(src => new User(src.EntraId, src.Email, src.Name));

        // ProjectTeamMember mappings
        CreateMap<ProjectTeamMember, ProjectTeamMemberDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
            .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project.Code));

        CreateMap<ProjectTeamMember, ProjectTeamMemberDetailDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
            .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project.Code))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.UserJobTitle, opt => opt.MapFrom(src => src.User.JobTitle))
            .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.CreatedBy));
        // Company Mappings
        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.HasLogo, opt => opt.MapFrom(src => src.Logo != null && src.Logo.Length > 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<CreateCompanyDto, Company>()
            .ConstructUsing(src => new Company(src.Code, src.Name, src.TaxId));

        CreateMap<UpdateCompanyDto, Company>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Company, CompanyWithOperationsDto>()
            .ForMember(dest => dest.Operations, opt => opt.MapFrom(src => src.Operations))
            .ForMember(dest => dest.TotalProjects, opt => opt.MapFrom(src => src.Operations.Sum(o => o.Projects.Count)))
            .ForMember(dest => dest.ActiveProjects, opt => opt.MapFrom(src =>
                src.Operations.Sum(o => o.Projects.Count(p => p.IsActive))))
            .ForMember(dest => dest.TotalBudget, opt => opt.MapFrom(src =>
                src.Operations.Sum(o => o.Projects.Sum(p => p.TotalBudget))));

        CreateMap<Operation, OperationSummaryDto>()
            .ForMember(dest => dest.ActiveProjectCount, opt => opt.MapFrom(src =>
                src.Projects.Count(p => p.IsActive)));

        // Project Mappings
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src => src.Operation.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.TeamMemberCount, opt => opt.MapFrom(src => src.ProjectTeamMembers.Count));
        //.ForMember(dest => dest.PhaseCount, opt => opt.MapFrom(src => src.Phases.Count));

        CreateMap<Project, ProjectListDto>()
            .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src => src.Operation.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Project, ProjectSummaryDto>()
           .IncludeBase<Project, ProjectDto>()
           .ForMember(dest => dest.BudgetUtilization, opt => opt.MapFrom(src =>
               src.ActualCost.HasValue && src.TotalBudget > 0 ?
                   (src.ActualCost.Value / src.TotalBudget) * 100 : 0))
           .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src =>
               (src.PlannedEndDate - DateTime.UtcNow).Days))
           .ForMember(dest => dest.DaysOverdue, opt => opt.MapFrom(src =>
               src.PlannedEndDate < DateTime.UtcNow ?
                   (DateTime.UtcNow - src.PlannedEndDate).Days : 0))
           .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src =>
               src.PlannedEndDate < DateTime.UtcNow && src.Status != ProjectStatus.Completed))
           .ForMember(dest => dest.IsOverBudget, opt => opt.MapFrom(src =>
               src.ActualCost.HasValue && src.ActualCost.Value > src.TotalBudget))
           .ForMember(dest => dest.ActiveTeamMembers, opt => opt.MapFrom(src =>
               src.ProjectTeamMembers.Count(ptm => ptm.IsActive)))
           //.ForMember(dest => dest.CompletedPhases, opt => opt.MapFrom(src =>
           //    src.Phases.Count(p => p.Status == Core.Enums.Setup.PhaseStatus.Completed)))
           //.ForMember(dest => dest.TotalPhases, opt => opt.MapFrom(src => src.Phases.Count))
           .ForMember(dest => dest.CostPerformanceIndex, opt => opt.MapFrom(src => 1.0)) // Placeholder
           .ForMember(dest => dest.SchedulePerformanceIndex, opt => opt.MapFrom(src => 1.0)); // Placeholder

        CreateMap<CreateProjectDto, Project>()
            .ConstructUsing(src => new Project(
                src.Code,
                src.Name,
                src.OperationId,
                src.PlannedStartDate,
                src.PlannedEndDate,
                src.TotalBudget,
                src.Currency
                ));

        CreateMap<UpdateProjectDto, Project>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null))
            .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project != null ? src.Project.Code : null))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.MetadataJson)
                    ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(src.MetadataJson, new System.Text.Json.JsonSerializerOptions())
                    : null));

        // Notification Mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : null))
            .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project != null ? src.Project.Code : null))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : null))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired))
            .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src =>
                !string.IsNullOrEmpty(src.MetadataJson)
                    ? System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(src.MetadataJson, new System.Text.Json.JsonSerializerOptions())
                    : null));

        CreateMap<CreateNotificationDto, Notification>()
            .ConstructUsing(src => new Notification(
                src.Title,
                src.Message,
                src.UserId ?? Guid.Empty,
                src.Type,
                src.Priority));
    }
}