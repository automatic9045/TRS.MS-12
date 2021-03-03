using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class LoadedPlugin<IPlugin>
    {
        public string Path { get; private set; }
        public IPlugin Plugin { get; private set; }

        public LoadedPlugin(IPlugin plugin, string path)
        {
            Path = path;
            Plugin = plugin;
        }
    }

    public class PluginsInfo
    {
        public List<LoadedPlugin<IPlugin>> Plugins { get; private set; }
        public List<LoadedPlugin<ITicketPlugin>> TicketPlugins { get; private set; }
        public List<LoadedPlugin<IPrinterPlugin>> PrinterPlugins { get; private set; }

        public PluginsInfo(List<LoadedPlugin<IPlugin>> plugins, List<LoadedPlugin<ITicketPlugin>> ticketPlugins, List<LoadedPlugin<IPrinterPlugin>> printerPlugins)
        {
            Plugins = plugins;
            TicketPlugins = ticketPlugins;
            PrinterPlugins = printerPlugins;
        }
    }

    public static class PluginsLoader
    {
        public static PluginsInfo Load()
        {
            List<LoadedPlugin<IPlugin>> plugins = new List<LoadedPlugin<IPlugin>>();
            List<LoadedPlugin<ITicketPlugin>> ticketPlugins = new List<LoadedPlugin<ITicketPlugin>>();
            List<LoadedPlugin<IPrinterPlugin>> printerPlugins = new List<LoadedPlugin<IPrinterPlugin>>();

            string iPrinterPluginName = typeof(IPrinterPlugin).FullName;
            string iTicketPluginName = typeof(ITicketPlugin).FullName;
            string iPluginName = typeof(IPlugin).FullName;

            string pluginFolder = Path.Combine(AppDirectory, "Plugins");
            if (!Directory.Exists(pluginFolder))
            {
                throw new ApplicationException("フォルダ \"" + pluginFolder + "\" が見つかりませんでした。");
            }

            string[] pluginDlls = Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in pluginDlls)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dll);
                    IEnumerable<Type> types = assembly.GetTypes();

                    ticketPlugins.AddRange(types.Where(t =>
                    {
                        return t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(iTicketPluginName) != null;
                    }).Select(t =>
                    {
                        return new LoadedPlugin<ITicketPlugin>((ITicketPlugin)assembly.CreateInstance(t.FullName), dll.Replace(AppDirectory + @"\", ""));
                    }));

                    plugins.AddRange(types.Where(t =>
                    {
                        return t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(iPluginName) != null && t.GetInterface(iTicketPluginName) == null;
                    }).Select(t =>
                    {
                        return new LoadedPlugin<IPlugin>((IPlugin)assembly.CreateInstance(t.FullName), dll.Replace(AppDirectory + @"\", ""));
                    }));
                }
                catch (Exception ex)
                {
                    Exception[] loaderExceptions = (ex as ReflectionTypeLoadException != null) ? ((ReflectionTypeLoadException)ex).LoaderExceptions : null;
                }
            }

            string printerFolder = Path.Combine(AppDirectory, "Printers");
            if (!Directory.Exists(printerFolder))
            {
                throw new ApplicationException("フォルダ \"" + printerFolder + "\" が見つかりませんでした。");
            }

            string[] printerDlls = Directory.GetFiles(printerFolder, "*.dll", SearchOption.AllDirectories);

            foreach (string dll in printerDlls)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dll);
                    IEnumerable<Type> types = assembly.GetTypes();

                    var a = types.Where(t =>
                    {
                        return t.IsClass && t.IsPublic && !t.IsAbstract && t.GetInterface(iPrinterPluginName) != null;
                    });
                    printerPlugins.AddRange(a.Select(t =>
                    {
                        return new LoadedPlugin<IPrinterPlugin>((IPrinterPlugin)assembly.CreateInstance(t.FullName), dll.Replace(AppDirectory + @"\", ""));
                    }));
                }
                catch
                { }
            }

            return new PluginsInfo(plugins, ticketPlugins, printerPlugins);
        }
    }
}
