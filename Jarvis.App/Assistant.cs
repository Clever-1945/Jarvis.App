using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Windows;
using Jarvis.Plugins;

namespace Jarvis.App;


//IHostService
public static class Assistant
{
    public static string ApplicationFileName { get; } = typeof(Assistant).Assembly.Location;
    public static string ApplicationPath { get; } = Path.GetDirectoryName(ApplicationFileName);

    private static JItemPlugin[] _plugins = null;

    public static JItemPlugin[] GetPlugins()
    {
        if (_plugins == null)
        {
            _plugins = GetPluginsInternal().ToArray();
        }

        return _plugins;
    }

    private static IEnumerable<JItemPlugin> GetPluginsInternal()
    {
        var t = new Type[]
        {
            typeof(Jarvis.IIS.Plugin.Plugin),
            typeof(Jarvis.Google.Plugin.Plugin),
            typeof(Jarvis.Tools.Plugin.Plugin)
        };
        IHostService hostService = new HostService();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Contains(typeof(JItemPlugin)))
                {
                    var plugin = Activator.CreateInstance(type) as JItemPlugin;
                    if (plugin != null)
                    {
                        try
                        {
                            plugin.Init(hostService);
                        }
                        catch (Exception e)
                        {
                        }
                        
                        yield return plugin;
                    }
                }
            }
        }
    }
}