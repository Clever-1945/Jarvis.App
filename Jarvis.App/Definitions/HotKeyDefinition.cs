using System.Windows.Input;
using Jarvis.Plugins.Extensions;

namespace Jarvis.App.Definitions;

public class HotKeyDefinition
{
    public ModifierKeys[] ModifierKeys;
    public Key[] Keys;

    public HotKeyDefinition()
    {
        
    }

    public HotKeyDefinition(string text)
    {
        Fill(text);
    }

    public void Fill(string text)
    {
        var modifierKeys = new List<ModifierKeys>();
        var keys = new List<Key>();

        var texts = text?.Split('+')?.Select(x => x?.Trim())?.Where(x => !String.IsNullOrWhiteSpace(x))?.ToArray();
        if (texts != null)
        {
            foreach (var textKey in texts)
            {
                if (String.Equals(textKey, "Ctrl", StringComparison.OrdinalIgnoreCase))
                {
                    modifierKeys.Add(System.Windows.Input.ModifierKeys.Control);
                }
                else if (String.Equals(textKey, "Win", StringComparison.OrdinalIgnoreCase))
                {
                    modifierKeys.Add(System.Windows.Input.ModifierKeys.Windows);
                }

                var modifierKey = textKey.ToEnum<System.Windows.Input.ModifierKeys>();
                if (modifierKey != null)
                {
                    modifierKeys.Add(modifierKey.Value);
                }
                else
                {
                    var key = textKey.ToEnum<System.Windows.Input.Key>();
                    if (key != null)
                    {
                        keys.Add(key.Value);
                    }
                }
            }
        }

        ModifierKeys = modifierKeys.ToArray();
        Keys = keys.ToArray();
    }

    public Key? GetKey()
    {
        if (Keys?.Length != 1)
            return null;

        return Keys[0];
    }
    
    public ModifierKeys? GetModifierKeys()
    {
        if (ModifierKeys == null || ModifierKeys.Length < 1)
            return null;


        ModifierKeys modifierKeys = ModifierKeys[0];
        for (int i = 1; i < ModifierKeys.Length; i++)
        {
            modifierKeys = modifierKeys | ModifierKeys[1];
        }

        return modifierKeys;
    }
    
    public override string ToString()
    {
        var modifierKeys = ModifierKeys?.Select(x =>
        {
            if (x == System.Windows.Input.ModifierKeys.Control)
                return "Ctrl";
            
            if (x == System.Windows.Input.ModifierKeys.Windows)
                return "Win";
            
            return x.ToString();
        })?.ToList() ?? new List<string>();
        modifierKeys.AddRange(Keys?.Select(x => x.ToString())?.ToArray() ?? new string[] { });

        return String.Join(" + ", modifierKeys);
    }
}