using System.Diagnostics;
using System.Management;
using System.Windows;
using Jarvis.Plugins;
using Jarvis.Plugins.Extensions;

namespace Jarvis.IIS.Plugin;

public class Plugin: JItemPlugin
{
    private byte[] iconData = null;
    private IHostService _hostService;
    
    public void Init(IHostService hostService)
    {
        _hostService = hostService;
        iconData = this.GetType().Assembly.GetResourceDaa("Jarvis.IIS.Plugin.Images.IIS.png");
    }

    public void Request(RequestPlugin request, ResponseProcessor processor)
    {
        if (!String.Equals(request.Query?.Trim(), "iis", StringComparison.OrdinalIgnoreCase))
            return;
        
        ThreadPool.QueueUserWorkItem((s) =>
        {
            var processes = Process.GetProcesses().Where(x =>
                    String.Equals(x.ProcessName, "w3wp", StringComparison.OrdinalIgnoreCase) ||
                    String.Equals(x.ProcessName, "w3wp.exe", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (var process in processes)
            {
                var definition = GetProcessDefinition(process);
                var text = $"[{definition.Id}] {definition.Name} ( {(definition.Owner ?? "")} )";
                var description = "";
                if (definition.Exceptions?.Length > 0)
                {
                    description = $"{definition.Exceptions.First().Message}\r\n{definition.Exceptions.First().StackTrace}";
                }
                else
                {
                    description = definition.CommandLine;
                }
                processor.ShowItem?.Invoke(new ResponsePlugin()
                {
                    Item = new ItemPlugin()
                    {
                        Id = Guid.NewGuid(),
                        Data = definition,
                        Request = request,
                        IconData = iconData,
                        Text = text,
                        Description = description,
                        Trigger = (i) =>
                        {
                            var definition = i?.Data as ProcessDefinition;
                            if (definition != null)
                            {
                                _hostService?.SetTextToClipboard?.Invoke(definition.Id.ToString());
                            }
                        }
                    }
                });
            }
        });
    }

    public ProcessDefinition GetProcessDefinition(Process process)
    {
        var definition = new ProcessDefinition();
        definition.Id = process.Id;
        definition.Name = process.ProcessName;

        List<Exception> listException = new List<Exception>();
        try
        {
            string query = "Select * From Win32_Process Where ProcessID = " + process.Id;
            var searcher = new ManagementObjectSearcher(query);
            var dictionary = GetNameValues(searcher.Get()).ToDictionarySelf(x => x.Key, x => x.Value);

            definition.Owner = dictionary.GetValueOrDefault("GetOwner");
            definition.CommandLine = dictionary.GetValueOrDefault("CommandLine");
        }
        catch (Exception ex)
        {
            listException.Add(ex);
        }

        definition.Exceptions = listException.ToArray();
        return definition;
    }
    
    public static IEnumerable<KeyValuePair<string, string>> GetNameValues(ManagementObjectCollection processList)
    {
        foreach (ManagementObject obj in processList)
        {
            foreach (var property in obj.Properties)
            {
                if (property.Name != null && property.Value is string)
                {
                    yield return new KeyValuePair<string, string>(property.Name, property.Value.ToString());
                }
            }

            string[] argList = new string[] { string.Empty, string.Empty };
            int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
            yield return new KeyValuePair<string, string>("GetOwner", argList[1] + "\\" + argList[0]);
        }
    }
    
    public class ProcessDefinition
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string CommandLine { set; get; }
        public string Owner { set; get; }
        public Exception[] Exceptions { set; get; }
    }
}