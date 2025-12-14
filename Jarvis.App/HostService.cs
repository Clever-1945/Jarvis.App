using Jarvis.Plugins;

namespace Jarvis.App;

public class HostService: IHostService
{
    public Action<string> SetTextToClipboard { get; } = SetTextToClipboardInternal;

    private static void SetTextToClipboardInternal(string text)
    {
        Clipboard.SetText(text ?? "");
    }
}