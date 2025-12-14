using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Web;
using Jarvis.Plugins;
using Jarvis.Plugins.Extensions;

namespace Jarvis.Google.Plugin;

[JPluginInfo("d58d51bd-2331-4deb-b98b-c74e08831631", "Google поиск", "Открывает страницу googl с введеным запросом")]
public class Plugin: JItemPlugin
{
    private byte[] iconData = null;
    private IHostService _hostService;
    
    public void Init(IHostService hostService)
    {
        _hostService = hostService;
        iconData = this.GetType().Assembly.GetResourceDaa("Jarvis.Google.Plugin.Images.google.png");
    }

    public void Request(RequestPlugin request, ResponseProcessor processor)
    {
        if (String.IsNullOrWhiteSpace(request.Query))
            return;

        processor?.ShowItem?.Invoke(new ResponsePlugin()
        {
            Item = new ItemPlugin()
            {
                Id = Guid.NewGuid(),
                Request = request,
                IconData = iconData,
                Text = "Поиск в Google",
                Description = "Открою браузер со страницей google в режиме ИИ",
                Trigger = (r) =>
                {
                    var q = HttpUtility.UrlEncode(r.Request.Query);
                    var url = $"https://www.google.com/search?q={q}";
                    System.Diagnostics.Process.Start(new ProcessStartInfo(url)
                    {
                        UseShellExecute = true
                    });
                }
            }
        });
        
        processor?.ShowItem?.Invoke(new ResponsePlugin()
        {
            Item = new ItemPlugin()
            {
                Id = Guid.NewGuid(),
                Request = request,
                IconData = iconData,
                Text = "ИИ поиск в Google",
                Description = "Открою браузер со страницей google и твоим запросом",
                Trigger = (r) =>
                {
                    var q = HttpUtility.UrlEncode(r.Request.Query);
                    var url = $"https://www.google.com/search?q={q}&udm=50";
                    System.Diagnostics.Process.Start(new ProcessStartInfo(url)
                    {
                        UseShellExecute = true
                    });
                }
            }
        });
    }
}