using System;
using System.Diagnostics;
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

            var appData= Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var startup = new DirectoryInfo(Path.Combine(appData, "Microsoft", "Windows", "Start Menu", "Programs", "Startup"));
            var location = this.GetType().Assembly.Location;
            var shortcutPath = Path.Combine(startup.FullName, Path.GetFileNameWithoutExtension(location) + ".lnk");
            
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }
            
            if (window.Settings.AutoStart)
            {
                var locationDirectory = Path.GetDirectoryName(location);
                var targetPath = $"{Path.Combine(locationDirectory, Path.GetFileNameWithoutExtension(location) + ".exe")}";
            
                string script = $"$s=(New-Object -ComObject WScript.Shell).CreateShortcut('{shortcutPath}')";
                script += $";$s.TargetPath='{targetPath}'";
                script += $";$s.Arguments='--autoStart'";
                script += $";$s.WorkingDirectory='{Path.GetDirectoryName(targetPath)}'";
                script += $";$s.IconLocation='{targetPath}'";
                script += $";$s.Save()";
                
                Process.Start(new ProcessStartInfo {
                    FileName = "powershell",
                    Arguments = $"-Command \"{script}\"",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
            
            
            
            
            // cd %appdata%\Microsoft\Windows\Start Menu\Programs\Startup
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