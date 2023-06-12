namespace DiscordBot.Services;

public class DiscordStartupService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<DiscordSocketClient> _logger;
    private readonly string _token;

    public DiscordStartupService(DiscordSocketClient client, ILogger<DiscordSocketClient> logger)
    {
        _client = client;
        _logger = logger;
        _token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ??
                 throw new InvalidOperationException("token not found");
    }
    

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();

        _client.Log += message => _logger.OnLogAsync(message);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.LogoutAsync();
        await _client.StopAsync();
    }
}