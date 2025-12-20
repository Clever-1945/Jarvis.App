using Jarvis.App.Extensions;
using Jarvis.App.Interfaces;

namespace Jarvis.App.Settings;

public class AppSettings: IClone<AppSettings>
{
    public Guid Id { set; get; }
    
    /// <summary>
    /// Запускать приложение при старте системы
    /// </summary>
    public bool AutoStart { set; get; }

    /// <summary>
    /// Показать главное окно при автозапуске
    /// </summary>
    public bool ShowDialogIfAutoStart { set; get; }

    /// <summary> Настройки плагинов </summary>
    public AppPluginSettings[] Plugins  { set; get; }

    public AppSettings Clone()
    {
        return new AppSettings()
        {
            Id = Id,
            AutoStart = this.AutoStart,
            ShowDialogIfAutoStart = this.ShowDialogIfAutoStart,
            Plugins = this.Plugins.CloneArray()
        };
    }
}