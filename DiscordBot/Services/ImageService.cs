namespace DiscordBot.Services;

public class ImageService
{
    private readonly HttpClient _openAIClient;
    private readonly HttpClient _staticContentClient;

    public ImageService(IHttpClientFactory factory)
    {
        _openAIClient = factory.CreateClient("OpenAI");
        _staticContentClient = factory.CreateClient("StaticContent");
    }

    public async Task<IEnumerable<FileAttachment>> GetGeneratedImageAsync(string prompt, string name, string size, int amount)
    {
        var request = new GenerateImageRequest
        {
            Prompt = prompt,
            Size = size,
            Amount = amount,
            User = name.Replace(" ", "_")
        };

        var response = await _openAIClient.PostAsJsonAsync("images/generations", request);
        response.EnsureSuccessStatusCode();
        
        var content = await JsonSerializer.DeserializeAsync<GenerateImageResponse>(await response.Content.ReadAsStreamAsync());
        if (content is null)
        {
            throw new InvalidOperationException("Failed to read response content.");
        }
        
        var attachments = new List<FileAttachment>();

        for (var i = 0; i < content.Data.Count; i++)
        {
            var stream = await GetImageStreamFromUrl(content.Data[i].Url);
            var fileName = $"image{i + 1}.png";
            var fileDescription = $"Generated image {i + 1}/{content.Data.Count}";
            
            attachments.Add(new FileAttachment(stream, fileName, fileDescription));
        }

        return attachments;
    }

    private async Task<Stream> GetImageStreamFromUrl(string url)
    {
        return await _staticContentClient.GetStreamAsync(url);
    }
}