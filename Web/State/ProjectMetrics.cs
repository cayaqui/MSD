namespace Web.State
{
    public class ProjectMetrics
    {
        public decimal CPI { get; set; } // Cost Performance Index
        public decimal SPI { get; set; } // Schedule Performance Index
        public decimal EV { get; set; }  // Earned Value
        public decimal PV { get; set; }  // Planned Value
        public decimal AC { get; set; }  // Actual Cost
        public decimal EAC { get; set; } // Estimate at Completion
        public DateTime LastUpdated { get; set; }
    }
}