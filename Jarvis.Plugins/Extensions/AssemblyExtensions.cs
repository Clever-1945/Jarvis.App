using System.Reflection;

namespace Jarvis.Plugins.Extensions;

public static class AssemblyExtensions
{
    // Extensions
    public static byte[] GetResourceDaa(this Assembly assembly, string name)
    {
        using (var stream = assembly.GetManifestResourceStream(name))
        {
            if (stream != null)
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    return ms.ToArray();
                }
            }
        }
        
        return null;
    }
}