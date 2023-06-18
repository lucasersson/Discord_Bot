namespace DiscordBot.Services;

public class ImageService
{
    private readonly HttpClient _openAiClient;
    private readonly HttpClient _staticContentClient;

    public ImageService(IHttpClientFactory factory)
    {
        _openAiClient = factory.CreateClient("OpenAI");
        _staticContentClient = factory.CreateClient("StaticContent");
    }

    public async Task<IEnumerable<FileAttachment>> GetGeneratedImageAsync(string prompt, string name, string size, int amount)
    {
        var request = new ImageRequest
        {
            Prompt = prompt,
            Size = size,
            Amount = amount,
            User = name.Replace(" ", "_")
        };

        var response = await _openAiClient.PostAsJsonAsync("images/generations", request);
        response.EnsureSuccessStatusCode();
        
        var content = await JsonSerializer.DeserializeAsync<ImageResponse>(await response.Content.ReadAsStreamAsync());
        if (content is null)
        {
            throw new InvalidOperationException("Failed to read response content.");
        }

        return await CreateAttachmentsFromContent(content);
    }

    public async Task<IEnumerable<FileAttachment>> GetEditedImage(IAttachment attachment, string prompt, string name,
        string size, int amount)
    {
        await ValidateAttachmentProperties(attachment);

        var imageByteArray = await GetImageByteArrayFromUrl(attachment.Url);
        
        // Convert the byte array to an ImageSharp.Image object.
        var image = await ImageProcessing.ToImage(imageByteArray);

        if (!ImageProcessing.HasTransparency(image))
        {
            throw new InvalidOperationException("Image must have transparency.");
        }

        var width = attachment.Width ?? throw new ArgumentNullException(nameof(attachment.Width), "Image must have a width.");
        var height = attachment.Height ?? throw new ArgumentNullException(nameof(attachment.Height), "Image must have a height.");
        
        // Make image square if needed and update the byte array.
        if (!IsSquare(width, height))
        {
            var resizedImage = ImageProcessing.MakeSquare(image, Math.Min(width, height));
            imageByteArray = await ImageProcessing.ToByteArray(resizedImage);
        }

        using var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(new ByteArrayContent(imageByteArray), "image", "image.png");
        multipartContent.Add(new StringContent(prompt, Encoding.UTF8, "application/json"), "prompt");
        multipartContent.Add(new StringContent(size, Encoding.UTF8, "application/json"), "size");
        multipartContent.Add(new StringContent(name, Encoding.UTF8, "application/json"), "user");
        multipartContent.Add(new StringContent(amount.ToString(), Encoding.UTF8, "application/json"), "n");

        var response = await _openAiClient.PostAsync("images/edits", multipartContent);
        response.EnsureSuccessStatusCode();

        var content = await JsonSerializer.DeserializeAsync<ImageResponse>(await response.Content.ReadAsStreamAsync());
        if (content is null)
        {
            throw new InvalidOperationException("Failed to read response content.");
        }

        return await CreateAttachmentsFromContent(content);
    }

    private async Task<Stream> GetImageStreamFromUrl(string url)
    {
        return await _staticContentClient.GetStreamAsync(url);
    }

    private async Task<byte[]> GetImageByteArrayFromUrl(string url)
    {
        return await _staticContentClient.GetByteArrayAsync(url);
    }

    private async Task<IEnumerable<FileAttachment>> CreateAttachmentsFromContent(ImageResponse content)
    {
        var attachments = new List<FileAttachment>();

        for (var i = 0; i < content.Data.Count; i++)
        {
            var stream = await GetImageStreamFromUrl(content.Data[i].Url);
            var fileName = $"image{i + 1}.png";
            var fileDescription = $"image {i + 1}/{content.Data.Count}";
            
            attachments.Add(new FileAttachment(stream, fileName, fileDescription));
        }

        return attachments;
    }

    private static bool IsSquare(int width, int height)
    {
        return height == width;
    }

    private static async Task ValidateAttachmentProperties(IAttachment attachment)
    {
        if (!attachment.Filename.EndsWith(".png"))
        {
            throw new InvalidOperationException("Image must be a .png");
        }

        if (attachment.Size > 4000000)
        {
            throw new InvalidOperationException("Image size can't exceed 4MB.");
        }

        await Task.CompletedTask;
    }
}