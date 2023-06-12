namespace DiscordBot.Services;

public class InteractionHandlingService : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _app;
    private readonly ILogger<InteractionService> _logger;

    public InteractionHandlingService(DiscordSocketClient client, InteractionService interactions, IServiceProvider services, IConfiguration configuration, IHostEnvironment app, ILogger<InteractionService> logger)
    {
        _client = client;
        _interactions = interactions;
        _services = services;
        _configuration = configuration;
        _app = app;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.Ready += RegisterInteractions;
        _client.InteractionCreated += OnInteractionExecutedAsync;
        
        _interactions.SlashCommandExecuted += OnSlashCommandExecutedAsync;
        _interactions.Log += message => _logger.OnLogAsync(message);
        
        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _interactions.Dispose();
        return Task.CompletedTask;
    }

    private Task RegisterInteractions()
    {
        if (_app.IsDevelopment())
        {
            var guildSections = _configuration.GetSection("guilds").GetChildren();
            foreach (var section in guildSections)
            {
                if (!ulong.TryParse(section.Value, out var guildId))
                {
                    throw new InvalidOperationException("Guild id missing is missing from configuration.");
                }

                _interactions.RegisterCommandsToGuildAsync(guildId);
            }
        }
        else
        {
            _interactions.RegisterCommandsGloballyAsync();
        }
        
        return Task.CompletedTask;
    }

    private async Task OnInteractionExecutedAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);
            var result = await _interactions.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ToString());
            }
        }
        catch
        {
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync()
                    .ContinueWith(msg => msg.Result.DeleteAsync());
            }
        }
    }

    private async Task OnSlashCommandExecutedAsync(SlashCommandInfo info, IInteractionContext context, IResult result)
    {
        // TODO: Complete logging for slash commands.
        _logger.LogInformation($"{context.User.Username} executed slash command '{info.Name}'");

        await Task.CompletedTask;
    }
}