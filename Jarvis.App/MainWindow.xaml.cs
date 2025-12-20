using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jarvis.Plugins;
using NHotkey.Wpf;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace Jarvis.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public Guid CurrentRequestId;

    public List<ItemPlugin> ItemPlugins { set; get; } = new List<ItemPlugin>();

    public Visibility ListViewVisibility { set; get; } = Visibility.Collapsed;

    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
        this.Deactivated  += OnLostFocus;

        this.DataContext = this;
        
        InputBindings.Add(new KeyBinding(new ActionCommand(() =>
        {
            this.Hide();
        }), new KeyGesture(Key.Escape)));
        
        InputBindings.Add(new KeyBinding(new ActionCommand(() =>
        {
            
        }), new KeyGesture(Key.V, ModifierKeys.Control)));
    }
    
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void OnLostFocus(object sender, EventArgs e)
    {
        this.Hide();
    }

    private void ProcessingShowWindow()
    {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;

        double windowWidth = this.ActualWidth;
        double windowHeight = this.ActualHeight;

        double centerLeft = (screenWidth / 2) - (windowWidth / 2);
        double centerTop = (screenHeight / 2) - (windowHeight / 2);

        double offsetAmount = 50; 
        double newTop = centerTop - offsetAmount;

        this.Left = centerLeft;
        this.Top = newTop - (screenHeight / 4);

        this.Activate();
        TextBoxName.Focus();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Assistant.ReloadAppSettings();
            Assistant.StartLoadPlugins();

            HotkeyManager.Current.AddOrReplace(
                "ShowCommand",
                Key.Space,
                ModifierKeys.Alt,
                (o, args) =>
                {
                    this.Show();
                    ProcessingShowWindow();
                    Request(TextBoxName.Text);
                });

            string[] args = Environment.GetCommandLineArgs();
            if (args?.Contains("--autoStart", StringComparer.OrdinalIgnoreCase) == true)
            {
                if (Assistant.Settings?.ShowDialogIfAutoStart != true)
                {
                    this.Hide();
                }
            }
            
            ProcessingShowWindow();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
            this.Close();
        }
    }

    private void OnTextChange(object sender, TextChangedEventArgs e)
    {
        Request(TextBoxName.Text);
    }

    private void Request(string request)
    {
        var requestId = Guid.NewGuid();
        CurrentRequestId = requestId;
        var plugins = Assistant.GetPlugins();
        var listSkipPlugins = Assistant.ListSkipPlugins;
        ThreadPool.QueueUserWorkItem((s) =>
        {
            foreach (var plugin in plugins)
            {
                if (listSkipPlugins != null && listSkipPlugins.Contains(plugin.Id))
                    continue;
                
                plugin.Instance.Request(new RequestPlugin()
                {
                    Id = requestId,
                    Query = request
                }, new ResponseProcessor()
                {
                    ShowItem = OnShowItemPlugin
                });

                OnShowItemPlugin(null);
            }
        });
    }

    private void OnShowItemPlugin(ResponsePlugin pesponse)
    {
        Dispatcher.Invoke(() =>
        {
            var currentRequestId = CurrentRequestId;
            ItemPlugins = (ItemPlugins ?? new List<ItemPlugin>()).Where(x => x.Request.Id == currentRequestId).ToList();
            if (pesponse?.Item?.Request?.Id == currentRequestId)
            {
                ItemPlugins.Add(pesponse.Item);
            }

            ListViewVisibility = ItemPlugins.Count < 1 ? Visibility.Collapsed : Visibility.Visible;
            OnPropertyChanged(nameof(ItemPlugins));
            OnPropertyChanged(nameof(ListViewVisibility));
        });
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
        {
            
        }
        
        if ((e.Key == Key.Up || e.Key == Key.Down) && ItemPlugins?.Count > 0)
        {
            var index = ItemPlugins.FindIndex(x => x == ListViewName.SelectedValue);
            if (e.Key == Key.Up)
            {
                index--;
            }
            if (e.Key == Key.Down)
            {
                index++;
            }
            
            if (index < 0)
            {
                ListViewName.SelectedValue = ItemPlugins.FirstOrDefault();
                return;
            }

            if (index >= ItemPlugins.Count - 1)
            {
                ListViewName.SelectedValue = ItemPlugins.LastOrDefault();
                return;
            }

            ListViewName.SelectedValue = ItemPlugins[index];
            ListViewName.ScrollIntoView(ListViewName.SelectedValue);
        }

        if (e.Key == Key.Enter)
        {
            ActivateItem();
        }
    }

    private void ActivateItem()
    {
        var item = ListViewName.SelectedValue as ItemPlugin;
        if (item == null)
            return;

#if DEBUG
        item.Trigger?.Invoke(item);
#else
        try
        {
            item.Trigger?.Invoke(item);
        }
        catch
        {
                
        }
#endif
        this.Hide();
    }

    private void OnSelection(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
        ActivateItem();
    }
}