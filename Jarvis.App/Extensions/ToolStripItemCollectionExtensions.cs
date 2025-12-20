namespace Jarvis.App.Extensions;

public static class ToolStripItemCollectionExtensions
{
    public static void Add(this ToolStripItemCollection collection, string text, Action action)
    {
        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem();
        exitMenuItem.Text = text;
        exitMenuItem.Click += (sender, args) => action?.Invoke();
        collection.Add(exitMenuItem);
    }
}