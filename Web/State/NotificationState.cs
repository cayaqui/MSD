namespace Web.State
{

    public class NotificationState
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public string? ActionUrl { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
