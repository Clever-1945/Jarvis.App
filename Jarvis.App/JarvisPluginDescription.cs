using Jarvis.Plugins;

namespace Jarvis.App;

public class JarvisPluginDescription
{
    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public JItemPlugin Instance { get; }

    public JarvisPluginDescription(Guid Id, string Name, string Description, JItemPlugin Instance)
    {
        this.Id = Id;
        this.Name = Name;
        this.Description = Description;
        this.Instance = Instance;
    }
}