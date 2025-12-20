using Jarvis.App.Interfaces;

namespace Jarvis.App.Settings;

/// <summary>
/// Настройки одного плагина
/// </summary>
public class AppPluginSettings: IClone<AppPluginSettings>
{
    public Guid Id { set; get; }
    public bool IsDisabled { set; get; }

    public AppPluginSettings Clone()
    {
        return new AppPluginSettings()
        {
            Id = this.Id,
            IsDisabled = this.IsDisabled
        };
    }
}