namespace Jarvis.Plugins;

public class ItemPlugin
{
    public Guid Id { set; get; }
    public RequestPlugin Request { set; get; }
    public string Text { set; get; }
    public string Description { set; get; }
    public object Data { set; get; }
    public byte[] IconData { set; get; }
    public Action<ItemPlugin> Trigger { set; get; }
}