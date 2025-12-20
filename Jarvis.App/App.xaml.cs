using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using Jarvis.App.Extensions;
using Jarvis.App.Settings;
using Application = System.Windows.Application;

namespace Jarvis.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private NotifyIcon _notifyIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        _notifyIcon = new NotifyIcon();

        var iconUri = new Uri("pack://application:,,,/Images/jarvis.ico", UriKind.Absolute);
        var info = Application.GetResourceStream(iconUri);
        _notifyIcon.Icon = new Icon(info.Stream);

        _notifyIcon.Text = "Jarvis";
        _notifyIcon.Visible = true;
        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.Add("&Настройки", OnClickSettings);
        _notifyIcon.ContextMenuStrip.Items.Add("&Выход", OnClickExit);
    }

    private void OnClickSettings()
    {
        var window = new SettingsWindow();
        window.ShowDialog();
        if (window.IsOk)
        {
            Assistant.InTransaction((db) =>
            {
                Assistant.SaveAppSettings(window.Settings, db);
                Assistant.ReloadAppSettings(db);
            });
        }
    }
    
    private void OnClickExit()
    {
        _notifyIcon?.Dispose();
        _notifyIcon = null;
        Application.Current.Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        _notifyIcon = null;
        base.OnExit(e);
    }
}