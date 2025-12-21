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
using Jarvis.ContextMenu;
using Application = System.Windows.Application;

namespace Jarvis.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        TreeIcon.Create();
        
        var iconUri = new Uri("pack://application:,,,/Images/jarvis.ico", UriKind.Absolute);
        var info = Application.GetResourceStream(iconUri);
        TreeIcon.SetIcon(new Icon(info.Stream));
        TreeIcon.SetText("Jarvis");
        TreeIcon.Insert(0, "&Выход", OnClickExit);
        TreeIcon.Insert(0, "&Настройки", OnClickSettings);
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
        Application.Current.Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        TreeIcon.Release();
        base.OnExit(e);
    }
}