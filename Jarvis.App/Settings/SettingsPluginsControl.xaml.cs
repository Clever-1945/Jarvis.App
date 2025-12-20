using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Jarvis.App.Extensions;

namespace Jarvis.App.Settings;

public partial class SettingsPluginsControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty SettingsProperty;
    public event PropertyChangedEventHandler PropertyChanged;
    
    public PluginViewModel[] Plugins { set; get; }
    
    public PluginViewModel SelectedPlugin { set; get; }
    
    public SettingsPluginsControl()
    {
        InitializeComponent();
    }
    
    static SettingsPluginsControl()
    {
        SettingsProperty = DependencyProperty.Register(
            nameof(Settings),
            typeof(AppSettings),
            typeof(SettingsPluginsControl),
            new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var control = d as SettingsPluginsControl;
                control.Plugins = control.GetPlugins();
                control.OnPropertyChanged(nameof(control.Plugins));
            })
        );
    }
    
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public AppSettings Settings
    {
        get => (AppSettings)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    private PluginViewModel[] GetPlugins()
    {
        List<PluginViewModel> models = new List<PluginViewModel>();
        var plugins = Assistant.GetPlugins();
        if (plugins != null)
        {
            foreach (var plugin in plugins)
            {
                var pluginSettings = Settings?.Plugins?.FirstOrDefault(x => x.Id == plugin.Id);
                if (pluginSettings == null)
                {
                    pluginSettings = new AppPluginSettings();
                    pluginSettings.Id = plugin.Id;
                    if (Settings != null)
                    {
                        Settings.Plugins = Settings.Plugins.Add(pluginSettings);
                    }
                }
                
                models.Add(new PluginViewModel()
                {
                    PluginDescription = plugin,
                    PluginSettings = pluginSettings
                });
            }
        }

        return models.ToArray();
    }

    private void OnSelectionPlugin(object sender, SelectionChangedEventArgs e)
    {
        SelectedPlugin = ListViewName.SelectedValue as PluginViewModel;
        OnPropertyChanged(nameof(SelectedPlugin));
    }
}

public class PluginViewModel
{
    public JarvisPluginDescription PluginDescription { set; get; }
    public AppPluginSettings PluginSettings { set; get; }
}