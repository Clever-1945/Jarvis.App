using System.Windows;
using Jarvis.Plugins;

namespace Jarvis.App;

public class HostService: IHostService
{
    public void SetTextToClipboard(string text)
    {
        Clipboard.SetText(text ?? "");
    }
}