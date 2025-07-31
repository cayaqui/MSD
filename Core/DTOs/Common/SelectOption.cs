namespace Core.DTOs.Common;

public class SelectOption
{
    public SelectOption()
    {
        
    }
    public SelectOption(string value, string text)
    {
        Value = value;
        Text = text;
    }
    public string Value { get; set; } = "";
    public string Text { get; set; } = "";
    public bool IsDisabled { get; set; }
    public bool IsGroup { get; set; }
    public string? GroupLabel { get; set; }
    public List<SelectOption>? SubOptions { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public object? Data { get; set; }
}