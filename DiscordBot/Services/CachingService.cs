namespace DiscordBot.Services;

public class CachingService
{
    private readonly IMemoryCache _cache;
    private readonly double _lifetime;

    public CachingService(IConfiguration configuration)
    {
        var options = new MemoryCacheOptions
        {
            ExpirationScanFrequency = TimeSpan.FromMinutes(5),
            Clock = new SystemClock()
        };
        _cache = new MemoryCache(options);
        _lifetime = configuration.GetValue<double>("chat:lifetime");

    }

    public Task<List<Message>> WriteAsync(string key, Message message)
    {
        var conversation = ReadAsync(key) ?? new List<Message>();
        conversation.Add(message);

        _cache.Set(key, conversation, TimeSpan.FromMinutes(_lifetime));

        return Task.FromResult(conversation);
    }

    public List<Message>? ReadAsync(string key)
    {
        return _cache.Get<List<Message>>(key);
    }
}