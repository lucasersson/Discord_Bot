namespace DiscordBot.Extensions;

/// <summary>
/// Collection of static methods that adds characteristics to plain text using markdown syntax.
/// </summary>
public static class Markdown
{
    public static string ToItalics(this string text)
    {
        return $"*{text}*";
    }
    
    public static string ToBold(this string text)
    {
        return $"**{text}**";
    }
    
    public static string ToBoldItalics(this string text)
    {
        return $"***{text}***";
    }

    public static string ToStrikethrough(this string text)
    {
        return $"~~{text}~~";
    }
    
    public static string ToUnderline(this string text)
    {
        return $"__{text}__";
    }
    
    public static string ToUnderlineItalics(this string text)
    {
        return $"__*{text}*__";
    }
    
    public static string ToUnderlineBold(this string text)
    {
        return $"__**{text}**__";
    }
    
    public static string ToUnderlineBoldItalics(this string text)
    {
        return $"__***{text}***__";
    }
}