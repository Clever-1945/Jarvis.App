using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Forms; // Используем пространство имен Forms
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

        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem();
        exitMenuItem.Text = "&Выход";
        exitMenuItem.Click += new EventHandler(OnClickExit);

        _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
        _notifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[] {
            exitMenuItem
        });
    }

    private void OnClickExit(object sender, EventArgs e)
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