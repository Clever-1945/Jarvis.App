using System.Drawing;
using System.Windows;
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
    }

    protected override void OnExit(ExitEventArgs e)
    {
        TreeIcon.Release();
        base.OnExit(e);
    }
}