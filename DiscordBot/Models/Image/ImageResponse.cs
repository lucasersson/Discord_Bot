namespace DiscordBot.Models.Image;

public class ImageResponse
{
    [JsonPropertyName("created")]
    public required int Created { get; set; }
    
    [JsonPropertyName("data")]
    public required List<Image> Data { get; set; }
}