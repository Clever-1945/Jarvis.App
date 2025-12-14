namespace Jarvis.Plugins;

public interface IHostService
{
    Action<string> SetTextToClipboard { get; }
}