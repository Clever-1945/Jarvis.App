using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Jarvis.Plugins;
using UserControl = System.Windows.Controls.UserControl;


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
                    control.ImageName.Source = control.ConvertByteArrayToImageSource(iconData);
                }
            })
        );
    }
    
    private ImageSource ConvertByteArrayToImageSource(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            return null;

        MemoryStream ms = new MemoryStream(imageData);

        BitmapImage biImg = new BitmapImage();
        biImg.BeginInit();
        biImg.StreamSource = ms;
        
        biImg.CacheOption = BitmapCacheOption.OnLoad;
        biImg.EndInit();
    
        biImg.Freeze(); 
        return biImg;
    }

    public ItemPlugin Item
    {
        get => (ItemPlugin)GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }
}