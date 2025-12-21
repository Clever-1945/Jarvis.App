using System.IO;
using System.Windows;
using System.Windows.Controls;
using Jarvis.App.Extensions;
using Image = System.Drawing.Image;


namespace Jarvis.App.Settings;

public partial class SettingsPluginItemControl : UserControl
{
    public static readonly DependencyProperty PluginDescriptionProperty;
    
    public SettingsPluginItemControl()
    {
        InitializeComponent();
    }

    static SettingsPluginItemControl()
    {
        PluginDescriptionProperty = DependencyProperty.Register(
            nameof(PluginDescription),
            typeof(JarvisPluginDescription),
            typeof(SettingsPluginItemControl),
            new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var control = d as SettingsPluginItemControl;
                var pluginDescription = control?.PluginDescription;
                if (pluginDescription != null)
                {
                    control.TextBlockName.Text = control.PluginDescription?.Name ?? "";
                    control.TextBlockDescription.Text = control.PluginDescription?.Description ?? "";
                    var assembly = pluginDescription.Instance?.GetType()?.Assembly;
                    if (assembly != null)
                    {
                        foreach (var name in assembly.GetManifestResourceNames())
                        {
                            try
                            {
                                var stream = assembly.GetManifestResourceStream(name);
                                Image.FromStream(stream);
                                stream.Seek(0, SeekOrigin.Begin);
                                control.ImagePlugin.Source = stream.ConvertByteArrayToImageSource();
                                break;
                            }
                            catch
                            {
                                
                            }
                        }
                    }
                }
            })
        );
    }

    public JarvisPluginDescription PluginDescription 
    {
        get => (JarvisPluginDescription)GetValue(PluginDescriptionProperty);
        set => SetValue(PluginDescriptionProperty, value);
    }
}