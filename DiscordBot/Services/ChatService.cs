namespace DiscordBot.Services;

public class ChatService
{
    private readonly HttpClient _client;
    private readonly CachingService _cache;

    public ChatService(IHttpClientFactory factory, CachingService cache)
    {
        _client = factory.CreateClient("OpenAI");
        _cache = cache;
    }

    public async Task<ChatResponse> GetChatAsync(string key, string prompt, string name)
    {
        var message = new Message
        {
            Role = "user",
            Content = prompt,
            Name = name.Replace(" ", "_")
        };
        var messages = await _cache.WriteAsync(key, message);
        var request = new ChatRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = messages
        };
        
        var response = await _client.PostAsJsonAsync("chat/completions", request);
        response.EnsureSuccessStatusCode();
            
        var content = await JsonSerializer.DeserializeAsync<ChatResponse>(await response.Content.ReadAsStreamAsync());
        if (content is null)
        {
            throw new InvalidOperationException("Failed to read response content.");
        }
        
        // First item in 'choices' is the response message.
        await _cache.WriteAsync(key, content.Choices[0].Message);
            
        return content;
    }

    public List<Message>? GetChatLog(string key)
    {
        return _cache.ReadAsync(key);
    }
}