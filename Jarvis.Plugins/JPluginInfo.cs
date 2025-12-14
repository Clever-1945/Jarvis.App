namespace Jarvis.Plugins;

[AttributeUsage(AttributeTargets.Class)]
public class JPluginInfoAttribute: Attribute
{
    public Guid Id { get; }
    public string Name { get; } 
    public string Description { get; }
    
    public JPluginInfoAttribute(string Id, string Name, string Description)
    {
        this.Id = Guid.Parse(Id);
        this.Name = Name;
        this.Description = Description;
    }
}