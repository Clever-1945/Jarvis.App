using System.Windows;

namespace Jarvis.App.Settings;

public partial class SettingsWindow : Window
{
    public AppSettings Settings { get; }
    
    public bool IsOk { private set; get; }
    
    public SettingsWindow()
    {
        InitializeComponent();
        Settings = Assistant.Settings?.Clone() ?? new AppSettings()
        {
            Id = Guid.NewGuid()
        };
        DataContext = this;
    }

    private void OnOk(object sender, RoutedEventArgs e)
    {
        IsOk = true;
        this.Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        IsOk = false;
        this.Close();
    }
}