namespace DiscordBot.Models.Chat;

public class ChatResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("choices")]
    public required List<Choice> Choices { get; set; }

    public override string ToString()
    {
        // First item in choices will be the response.
        return Choices[0].Message.Content;
    }
}