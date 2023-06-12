
using var host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(app =>
{
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .Build();
    
    app.AddConfiguration(configuration);
}).ConfigureServices((context, services) =>
{
    services.RegisterServices();
    services.BuildOpenAiClientFactory(context);
    services.BuildStaticContentClientFactory();
}).ConfigureLogging(logger =>
{
    logger.AddConsole()
        .SetMinimumLevel(LogLevel.Information);
}).Build();

await host.RunAsync();