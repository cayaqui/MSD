namespace Web.State
{
    public class CompanyState
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
    }
}
