using Jarvis.Plugins;
using Jarvis.Plugins.Extensions;

namespace Jarvis.Tools.Plugin;

[JPluginInfo("16a51ffc-ec85-46bb-9859-37b8e6adbbbe", "Мини помошники", "Плагин с минимальными утилитами - помошниками")]
public class Plugin: JItemPlugin
{
    private IHostService _hostService;
    private byte[] iconUidData = null;
    
    public void Init(IHostService hostService)
    {
        _hostService = hostService;
        iconUidData = this.GetType().Assembly.GetResourceDaa("Jarvis.Tools.Plugin.Images.uid.png");
    }

    public void Request(RequestPlugin request, ResponseProcessor processor)
    {
        if (request.Query.IsEqualsKeyboard("uid"))
        {
            var uid = Guid.NewGuid();
            processor?.ShowItem?.Invoke(new ResponsePlugin()
            {
                Item = new ItemPlugin()
                {
                    Id = Guid.NewGuid(),
                    Request = request,
                    IconData = iconUidData,
                    Text = uid.ToString(),
                    Description = $"Скопирую {uid} в буфер обмена",
                    Data = uid,
                    Trigger = (r) =>
                    {
                        if (r?.Data is Guid id)
                        {
                            _hostService.SetTextToClipboard(id.ToString());
                        }
                    }
                }
            });
        }
    }
}