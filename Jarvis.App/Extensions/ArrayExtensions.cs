using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Jarvis.App.Extensions;

public static class ArrayExtensions
{
    public static T[] Add<T>(this T[] array, T instance)
    {
        T[] next = new T[(array?.Length ?? 0) + 1];
        if (array != null && array.Length > 0)
        {
            // public static unsafe void Copy(Array sourceArray, Array destinationArray, int length)
            Array.Copy(array, next, array.Length);
        }

        next[next.Length - 1] = instance;
        return next;
    }

    public static ImageSource ConvertByteArrayToImageSource(this Stream imageData)
    {
        BitmapImage biImg = new BitmapImage();
        biImg.BeginInit();
        biImg.StreamSource = imageData;
        
        biImg.CacheOption = BitmapCacheOption.OnLoad;
        biImg.EndInit();
    
        biImg.Freeze(); 
        return biImg;
    }
    
    public static ImageSource ConvertByteArrayToImageSource(this byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            return null;

        MemoryStream ms = new MemoryStream(imageData);
        return ms.ConvertByteArrayToImageSource();
    }
}