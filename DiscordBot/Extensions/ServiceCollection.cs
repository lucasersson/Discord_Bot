namespace DiscordBot.Extensions;

public static class ServiceCollection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services.AddSingleton<DiscordSocketClient>()
            .AddSingleton<InteractionService>()
            .AddSingleton<ChatService>()
            .AddSingleton<ImageService>()
            .AddSingleton<CachingService>()
            .AddHostedService<DiscordStartupService>()
            .AddHostedService<InteractionHandlingService>();
    }
    
    public static IHttpClientBuilder BuildOpenAiClientFactory(this IServiceCollection services, HostBuilderContext context)
    {
        return services.AddHttpClient("OpenAI", client =>
        {
            client.BaseAddress = new Uri(context.Configuration.GetValue<string>("openai:baseurl") ??
                                         "https://api.openai.com/v1/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAIAPI_TOKEN"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("lucasersson_discordbot");
        }).SetHandlerLifetime(TimeSpan.FromMinutes(3));
    }
    
    public static IHttpClientBuilder BuildStaticContentClientFactory(this IServiceCollection services)
    {
        return services.AddHttpClient("StaticContent", client =>
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/*"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("lucasersson_discordbot");
        }).SetHandlerLifetime(TimeSpan.FromMinutes(3));
    }
}