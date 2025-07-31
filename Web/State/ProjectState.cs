namespace Web.State
{

    public class ProjectState
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Progress { get; set; }
    }
}