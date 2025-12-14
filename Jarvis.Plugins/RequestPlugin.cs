namespace Jarvis.Plugins;

public class RequestPlugin
{
    public Guid Id { set; get; }
    public string Query { set; get; }
}

public class ResponsePlugin
{
    public ItemPlugin Item { set; get; }
}

public class ResponseProcessor
{
    public Action<ResponsePlugin> ShowItem { set; get; }
}

