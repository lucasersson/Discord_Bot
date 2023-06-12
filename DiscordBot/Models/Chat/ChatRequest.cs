namespace DiscordBot.Models.Chat;

public class ChatRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; set; }
    
    [JsonPropertyName("messages")]
    public required List<Message> Messages { get; set; }
}