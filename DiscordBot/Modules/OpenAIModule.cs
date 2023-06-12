using System.Text;

namespace DiscordBot.Modules;

[Group("openai", "collection of commands for using openai services.")]
public class OpenAIModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ChatService _chat;
    private readonly ImageService _image;
    
    public OpenAIModule(ChatService chat, ImageService image)
    {
        _chat = chat;
        _image = image;
    }

    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [SlashCommand("chat", "engage in a conversation with chat-gpt")]
    public async Task Chat(string prompt, [Choice("Personal chat", 1)] [Choice("Channel chat", 0)] int type = 0)
    {
        await DeferAsync();

        var conversationId = GetConversationId(type);

        try
        {
            var result = await _chat.GetChatAsync(conversationId, prompt, Context.User.Username);
            var sb = new StringBuilder();
            sb.AppendLine(prompt.ToBoldItalics());
            sb.AppendLine();
            sb.AppendLine(result.ToString());
            
            await FollowupAsync(sb.ToString());
        }
        catch (HttpRequestException exception)
        {
            var errorMessage = MapResponseToHttpException(exception);
            await FollowupAsync(errorMessage, ephemeral: true);

        }
        catch (InvalidOperationException exception)
        {
            await FollowupAsync(exception.Message, ephemeral: true);
        }
        catch (Exception)
        {
            await FollowupAsync("An unknown error occured.", ephemeral: true);
        }
    }
    
    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [SlashCommand("logs", "see conversation history of a previous or ongoing chat")]
    public async Task Logs([Choice("Personal chat", 1)][Choice("Channel chat", 0)] int type = 0)
    {
        await DeferAsync(ephemeral: true);
        
        var conversationId = GetConversationId(type);

        var logs = _chat.GetChatLog(conversationId);
        if (logs is null)
        {
            await FollowupAsync("No conversations found. Either you did not participate in any chat, or the conversation logs were deleted.", ephemeral: true);
        }
        else
        {
            var sb = new StringBuilder();
            logs.ForEach(log =>
            {
                var name = log.Name.Replace("_", " ");
                var operation = name is "openai" ? "answered" : "asked";

                sb.AppendLine($"{name} {operation}".ToItalics());
                sb.AppendLine(log.Content);
                sb.AppendLine();
            });

            await FollowupAsync(sb.ToString(), ephemeral: true);
        }
    }

    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [SlashCommand("generateimage", "generate an image by a prompt input")]
    public async Task GenerateImage(string prompt, [Choice("small", "256x256")][Choice("medium", "512x512")][Choice("large", "1024x1024")] string size = "1024x1024", [Choice("1", 1)]
        [Choice("2", 2)][Choice("3", 3)]int amount = 1)
    {
        await DeferAsync();
        
        try
        {
            var result = await _image.GetGeneratedImageAsync(prompt, Context.User.Username, size, amount);
            await FollowupWithFilesAsync(result, prompt.ToBoldItalics());
        }
        catch (HttpRequestException exception)
        {
            var errorMessage = MapResponseToHttpException(exception);
            await FollowupAsync(errorMessage, ephemeral: true);
        }
        catch (InvalidOperationException exception)
        {
            await FollowupAsync(exception.Message, ephemeral: true);
        }
        catch (Exception)
        {
            await FollowupAsync("An unknown error occured.", ephemeral: true);
        }
    }

    private string GetConversationId(int type)
    {
        // Allows for both shared and personal conversation among users with chat-gpt.
        return type is not 1 ? "shared-key" : Context.User.Id.ToString();
    }

    private static string MapResponseToHttpException(HttpRequestException exception)
    {
        return exception.StatusCode switch
        {
            HttpStatusCode.BadRequest => "Request was invalid or malformed.",
            HttpStatusCode.Unauthorized =>
                "API key expired or invalid, or maximum amount of requests has been hit. Contact bot owner.",
            HttpStatusCode.Forbidden => "Content flagged due to policy violations.",
            HttpStatusCode.NotFound => "Requested resources not found.",
            HttpStatusCode.TooManyRequests => "Too many concurrent attempts, try again later.",
            HttpStatusCode.InternalServerError => "Server error, try again later.",
            _ => "Unknown error occured, try again later."
        };
    }
}