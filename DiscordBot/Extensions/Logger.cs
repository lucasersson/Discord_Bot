namespace DiscordBot.Extensions;

public static class Logger
{
    public static Task OnLogAsync(this ILogger logger, LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Verbose:
            case LogSeverity.Info:
                logger.LogInformation(message.ToString());
                break;
            case LogSeverity.Warning:
                logger.LogWarning(message.ToString());
                break;
            case LogSeverity.Error:
                logger.LogError(message.ToString());
                break;
            case LogSeverity.Critical:
                logger.LogCritical(message.ToString());
                break;
            case LogSeverity.Debug:
                logger.LogDebug(message.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(message.Severity));
        }        
        
        return Task.CompletedTask;
    }
}