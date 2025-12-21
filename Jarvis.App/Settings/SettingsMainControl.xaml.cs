using System.Windows;
using System.Windows.Controls;


namespace Jarvis.App.Settings;

public partial class SettingsMainControl : UserControl
{
    public static readonly DependencyProperty SettingsProperty;
    
    public SettingsMainControl()
    {
        InitializeComponent();
    }
    
    static SettingsMainControl()
    {
        SettingsProperty = DependencyProperty.Register(
            nameof(Settings),
            typeof(AppSettings),
            typeof(SettingsMainControl),
            new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                
            })
        );
    }

    public AppSettings Settings
    {
        get => (AppSettings)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }
}