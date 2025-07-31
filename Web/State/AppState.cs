namespace Web.State
{
    public class AppState
    {
        public UserState? CurrentUser { get; set; }
        public CompanyState? ActiveCompany { get; set; }
        public ProjectState? ActiveProject { get; set; }
        public SystemConfiguration Configuration { get; set; } = new();
        public List<NotificationState> Notifications { get; set; } = new();
        public bool IsLoading { get; set; }
        public string? LoadingMessage { get; set; }
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }
}
