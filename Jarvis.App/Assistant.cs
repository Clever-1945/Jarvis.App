using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using Jarvis.App.Definitions;
using Jarvis.App.Settings;
using Jarvis.Plugins;
using LiteDB;

namespace Jarvis.App;

public static class Assistant
{
    public static string ApplicationFileName { get; } = typeof(Assistant).Assembly.Location;
    public static string ApplicationPath { get; } = Path.GetDirectoryName(ApplicationFileName);

    private static JarvisPluginDescription[] _plugins = new JarvisPluginDescription[] { };
    private static object lock_db = new object();
    
    public static AppSettings Settings { private set; get; }
    
    public static HashSet<Guid> ListSkipPlugins { private set; get; }

    public static JarvisPluginDescription[] GetPlugins() => _plugins;

    public static void LoadPlugins()
    {
        IHostService hostService = new HostService();

        var listLocationPlugin = new string[]
        {
            Path.Combine(ApplicationPath, "Plugins"),
            Path.Combine(ApplicationPath, "..", "Plugins")
        };
#if DEBUG

#else
        listLocationPlugin = listLocationPlugin.Take(1).ToArray();
#endif

        var dictionaryPlugin = new Dictionary<Guid, JarvisPluginDescription>();

        foreach (var locationPlugin in listLocationPlugin)
        {
            if (!Directory.Exists(locationPlugin))
                continue;

            var alc = new AssemblyLoadContext("ProcessorLoadAssembly", isCollectible: true);
            var files = new DirectoryInfo(locationPlugin).GetFiles("*.dll", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    {
                        var typePlugin = alc.LoadFromAssemblyPath(file.FullName)
                            .GetTypes()
                            .Where(x => x.GetInterfaces().Contains(typeof(JItemPlugin)))
                            .ToArray();
                    }

                    {
                        var assembly = Assembly.LoadFrom(file.FullName);

                        var listTypePlugin = assembly
                            .GetTypes()
                            .Where(x => x.GetInterfaces().Contains(typeof(JItemPlugin)))
                            .ToArray();

                        foreach (var typePlugin in listTypePlugin)
                        {
                            var description = GetPluginDescription(typePlugin, hostService);
                            if (description != null)
                            {
                                dictionaryPlugin[description.Id] = description;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            alc.Unload();
        }
        _plugins = dictionaryPlugin.Values.ToArray();
    }

    private static JarvisPluginDescription GetPluginDescription(Type typePlugin, IHostService hostService)
    {
        var instance = Activator.CreateInstance(typePlugin) as JItemPlugin;
        if (instance != null)
        {
            instance.Init(hostService);
            
            var info = typePlugin.GetCustomAttribute<JPluginInfoAttribute>();
            
            string name = info?.Name ?? typePlugin.Name;
            string description = info?.Description ?? $"{typePlugin.Namespace}.{typePlugin.Name}";
            Guid id = info?.Id ?? Guid.Empty;
            if (id == Guid.Empty)
            {
                var hashAssembly = MD5.HashData(File.ReadAllBytes(typePlugin.Assembly.Location));
                var uidAssembly = new Guid(hashAssembly);
                var hashType = $"{uidAssembly}.{typePlugin.Namespace}.{typePlugin.Name}";

                byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(hashType));
                id = new Guid(hashBytes);
            }

            return new JarvisPluginDescription(id, name, description, instance);
        }

        return null;
    }

    public static void InTransaction(Action<LiteDatabase> action, LiteDatabase db = null)
    {
        if (db == null)
        {
            lock (lock_db)
            {
                using (db = new LiteDatabase(Path.Combine(ApplicationPath, "data.db")))
                {
                    action?.Invoke(db);
                }
            }
        }
        else
        {
            action?.Invoke(db);
        }
    }

    /// <summary> Загрузить настройки из базы данных </summary>
    public static void ReloadAppSettings(LiteDatabase db = null)
    {
        InTransaction((db) =>
        {
            Settings = db.GetCollection<AppSettings>().FindAll().FirstOrDefault();
            var listSkipPlugins = Settings?.Plugins?.Where(x => x.IsDisabled)?.Select(x => x.Id)?.ToArray();
            ListSkipPlugins = new HashSet<Guid>(listSkipPlugins ?? Array.Empty<Guid>());
        }, db);
    }
    
    /// <summary> Сохранить настройки из базы данных </summary>
    public static void SaveAppSettings(AppSettings settings, LiteDatabase db = null)
    {
        InTransaction((db) =>
        {
            if (settings != null)
            {
                db.GetCollection<AppSettings>().DeleteAll();
                db.GetCollection<AppSettings>().Insert(settings);
            }
        }, db);
    }

    public static HotKeyDefinition GetHotKeyDefinition(Key[] keys)
    {
        List<ModifierKeys> hotModifierKeys = new List<ModifierKeys>();
        List<Key> hotKey = new List<Key>();

        if (keys != null)
        {
            foreach (var key in keys)
            {
                if (key == Key.LeftCtrl || key == Key.RightCtrl)
                {
                    hotModifierKeys.Add(ModifierKeys.Control);
                }
                else  if (key == Key.LeftAlt || key == Key.RightAlt)
                {
                    hotModifierKeys.Add(ModifierKeys.Alt);
                }
                else  if (key == Key.LeftShift || key == Key.RightShift)
                {
                    hotModifierKeys.Add(ModifierKeys.Shift);
                }
                else  if (key == Key.LWin || key == Key.RWin)
                {
                    hotModifierKeys.Add(ModifierKeys.Windows);
                }
                else
                {
                    hotKey.Add(key);
                }
            }
        }

        return new HotKeyDefinition()
        {
            Keys = hotKey.ToArray(),
            ModifierKeys = hotModifierKeys.ToArray()
        };
    }
}