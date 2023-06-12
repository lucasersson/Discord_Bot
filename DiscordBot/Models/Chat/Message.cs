namespace DiscordBot.Models.Chat;

public class Message
{
    // TODO: Make role an enum?
    [JsonPropertyName("role")]
    public required string Role { get; set; }
    
    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("name")]
    // Defaults the message author to 'openai'.
    // Will make it easier to parse a conversation or some similar logic where you want to distinct the messages between human and ai.
    public string Name { get; set; } = "openai";
}