using Web.Services.Interfaces;
using Web.State;

namespace Web.Services.Implementation
{

    public class AppStateService : IAppStateService
    {
        private readonly ILogger<AppStateService> _logger;

        public UserState? CurrentUser { get; set; }
        public CompanyState? ActiveCompany { get; set; }
        public ProjectState? ActiveProject { get; set; }

        public event Action? OnUserChanged;
        public event Action? OnCompanyChanged;
        public event Action? OnProjectChanged;

        public AppStateService(ILogger<AppStateService> logger)
        {
            _logger = logger;
        }

        public void SetUser(UserState? user)
        {
            CurrentUser = user;
            _logger.LogInformation("Usuario establecido: {UserName}", user?.Name ?? "null");
            OnUserChanged?.Invoke();
        }

        public void SetCompany(CompanyState? company)
        {
            ActiveCompany = company;

            // Si cambia la empresa, limpiar el proyecto
            if (ActiveProject?.CompanyId != company?.Id)
            {
                SetProject(null);
            }

            _logger.LogInformation("Empresa activa: {CompanyName}", company?.Name ?? "null");
            OnCompanyChanged?.Invoke();
        }

        public void SetProject(ProjectState? project)
        {
            ActiveProject = project;
            _logger.LogInformation("Proyecto activo: {ProjectName}", project?.Name ?? "null");
            OnProjectChanged?.Invoke();
        }

        public void Clear()
        {
            CurrentUser = null;
            ActiveCompany = null;
            ActiveProject = null;

            OnUserChanged?.Invoke();
            OnCompanyChanged?.Invoke();
            OnProjectChanged?.Invoke();
        }
    }





}
