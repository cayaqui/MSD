namespace Web.Models;

public class ToastMessage
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public ToastType Type { get; set; } = ToastType.Info;
}

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}