using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Jarvis.App.Settings;

public partial class SettingsPluginControl : UserControl
{
    public static readonly DependencyProperty PluginDescriptionProperty;
    public static readonly DependencyProperty PluginSettingsProperty;

    public SettingsPluginControl()
    {
        InitializeComponent();
    }
    
    static SettingsPluginControl()
    {
        PluginDescriptionProperty = DependencyProperty.Register(
            nameof(PluginDescription),
            typeof(JarvisPluginDescription),
            typeof(SettingsPluginControl),
            new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                
            })
        );
        
        PluginSettingsProperty = DependencyProperty.Register(
            nameof(PluginSettings),
            typeof(AppPluginSettings),
            typeof(SettingsPluginControl),
            new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var control = d as SettingsPluginControl;
                control.NameGrid.Visibility = control.PluginSettings == null ? Visibility.Hidden : Visibility.Visible;
            })
        );
    }

    public JarvisPluginDescription PluginDescription 
    {
        get => (JarvisPluginDescription)GetValue(PluginDescriptionProperty);
        set => SetValue(PluginDescriptionProperty, value);
    }
    
    public AppPluginSettings PluginSettings 
    {
        get => (AppPluginSettings)GetValue(PluginSettingsProperty);
        set => SetValue(PluginSettingsProperty, value);
    }
}