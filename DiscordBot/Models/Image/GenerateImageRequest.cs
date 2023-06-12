namespace DiscordBot.Models.Image;

public class GenerateImageRequest
{
    [JsonPropertyName("prompt")]
    public required string Prompt { get; set; }
    
    [JsonPropertyName("n")]
    public int Amount { get; set; }
    
    // TODO: Make enum?
    [JsonPropertyName("size")]
    public string Size { get; set; }
    
    [JsonPropertyName("user")]
    public string User { get; set; }
}