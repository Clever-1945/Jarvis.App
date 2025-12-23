using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Image = System.Drawing.Image;


namespace Jarvis.App.Settings;

public partial class SettingsMainControl : UserControl
{
    public static readonly DependencyProperty SettingsProperty;

    private HashSet<Key> statePressedKeys = new HashSet<Key>();
    private Key[] stateShortcutKeys = new Key[0];

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
                var control = d as SettingsMainControl;
                if (control?.Settings != null)
                {
                    control.TextShortcutName.Text = control.Settings.ShowHotKey ?? "";
                }
            })
        );
    }

    public AppSettings Settings
    {
        get => (AppSettings)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    private Key GetKey(KeyEventArgs e)
    {
        var key = e.SystemKey == Key.None ? e.Key : e.SystemKey;
        return key;
    }
    
    private void OnShortcutUp(object sender, KeyEventArgs e)
    {
        var key = GetKey(e);
        
        if (statePressedKeys.Contains(key))
        {
            statePressedKeys.Remove(key);
        }

        if (statePressedKeys.Count < 1)
        {
            var definition = Assistant.GetHotKeyDefinition(stateShortcutKeys);
            if (definition.ModifierKeys.Length < 1)
            {
                TextShortcutError.Text = "Не установлена функциональная клавиша ( Ctrl, Alt, Shift )";
                return;
            }
            else if (definition.Keys.Length < 1)
            {
                TextShortcutError.Text = "Не установлена дополнительная клавиша ( Буква, цифра, пробел )";
                return;
            }
            else if (definition.Keys.Length != 1)
            {
                TextShortcutError.Text = "Дополнительная клавиша ( Буква, цифра, пробел ) может быть только одна";
                return;
            }

            var settings = Settings;
            if (settings != null)
            {
                settings.ShowHotKey = definition.ToString();
            }
            TextShortcutError.Text = null;
            TextShortcutName.Text = definition.ToString();
        }
    }

    private void OnShortcutDown(object sender, KeyEventArgs e)
    {
        var key = GetKey(e);

        statePressedKeys.Add(key);
        stateShortcutKeys = statePressedKeys.ToArray();
        Debug.WriteLine(key.ToString());
        e.Handled = true; 
    }
}