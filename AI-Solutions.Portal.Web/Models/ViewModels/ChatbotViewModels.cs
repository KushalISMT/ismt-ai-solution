using System.ComponentModel.DataAnnotations;

namespace AI_Solutions.Portal.Web.Models.ViewModels;

public class ChatbotRequest
{
    [Required]
    public string Message { get; set; } = string.Empty;
}

public class ChatbotResponse
{
    public string Reply { get; set; } = string.Empty;

    public List<ChatbotSuggestion> Suggestions { get; set; } = new();
}

public class ChatbotSuggestion
{
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// "quick" sends the value as a new chat message,
    /// "link" navigates to the value URL.
    /// </summary>
    public string Type { get; set; } = "quick";

    public string Value { get; set; } = string.Empty;
}
