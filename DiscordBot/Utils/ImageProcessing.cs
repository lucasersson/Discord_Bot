using Image = SixLabors.ImageSharp.Image;

namespace DiscordBot.Utils;

public static class ImageProcessing
{
    public static async Task<Image> ToImage(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        return await Image.LoadAsync(stream: stream);
    }

    public static async Task<byte[]> ToByteArray(Image image)
    {
        using var stream = new MemoryStream();
        await image.SaveAsync(stream, new PngEncoder());

        return stream.ToArray();
    }
    
    public static Image MakeSquare(Image image, int size)
    {
        image.Mutate(x => x.Resize(size, size));
        return image;
    }

    public static bool HasTransparency(Image image)
    {
        using var rgba = image.CloneAs<Rgba32>();
        for (var y = 0; y < rgba.Height; y++)
        {
            for (var x = 0; x < rgba.Width; x++)
            {
                var pixel = rgba[x, y];
                if (pixel.A < 255) // Check if the alpha channel value is less than 255
                {
                    return true;
                }
            }
        }

        return false;
    }
}