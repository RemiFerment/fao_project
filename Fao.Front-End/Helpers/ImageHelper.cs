using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Fao.Front_End.Helpers;

public class ImageHelper
{
    public byte[]? CompressImage(byte[] originalImage, int maxWidth = 900, int quality = 50)
    {
        try
        {
            using var image = Image.Load(originalImage);
            if (image.Width > maxWidth)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(maxWidth, 0)
                }));
            }

            using var ms = new MemoryStream();

            var encoder = new JpegEncoder
            {
                Quality = quality
            };

            image.Save(ms, encoder);

            return ms.ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Image compression failed: {e.Message} \n StackTrace: {e.StackTrace}");
            return null;
        }
    }

}