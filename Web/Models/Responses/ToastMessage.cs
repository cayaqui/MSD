namespace Web.Models.Responses
{
    public class ToastMessage
    {
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; }
        public DateTime Timestamp { get; set; }

        public int Position { get; set; } = 0; // 0: Top, 1: Bottom
    }
}
