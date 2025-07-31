using Web.State;

namespace Web.Services.Interfaces
{
    public interface IAppStateService
    {
        // Estado actual
        UserState? CurrentUser { get; set; }
        CompanyState? ActiveCompany { get; set; }
        ProjectState? ActiveProject { get; set; }

        // Eventos
        event Action? OnUserChanged;
        event Action? OnCompanyChanged;
        event Action? OnProjectChanged;

        // Métodos
        void SetUser(UserState? user);
        void SetCompany(CompanyState? company);
        void SetProject(ProjectState? project);
        void Clear();
    }

}
