namespace DiscordBot.Models.Chat;

public class Choice
{
    [JsonPropertyName("index")]
    public required int Index { get; set; }
    
    [JsonPropertyName("message")]
    public required Message Message { get; set; }
    
    [JsonPropertyName("finish_reason")]
    public required string FinishReason { get; set; }
}