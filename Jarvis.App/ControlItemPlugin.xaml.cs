using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.App.Extensions;
using Jarvis.Plugins;


namespace Jarvis.App;

public partial class ControlItemPlugin : UserControl
{
    public static readonly DependencyProperty ItemProperty;
    
    public ControlItemPlugin()
    {
        InitializeComponent();
    }

    static ControlItemPlugin()
    {
        ItemProperty = DependencyProperty.Register(
            nameof(Item),
            typeof(ItemPlugin),
            typeof(ControlItemPlugin),
            new PropertyMetadata((DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            {
                var control = d as ControlItemPlugin;
                if (control != null)
                {
                    var iconData = control.Item?.IconData;
                    control.ImageName.Source = iconData.ConvertByteArrayToImageSource();
                }
            })
        );
    }

    public ItemPlugin Item
    {
        get => (ItemPlugin)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }
}