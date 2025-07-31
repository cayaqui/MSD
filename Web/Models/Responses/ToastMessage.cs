namespace Web.Models.Responses
{
    public class ToastMessage
    {
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
