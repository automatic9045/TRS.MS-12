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
    public static class PluginsLoader
    {
        public static PluginList Load(AppConnector appConnector)
        {
            List<LoadedPlugin<IPlugin>> plugins = new List<LoadedPlugin<IPlugin>>();
            List<LoadedPlugin<ITicketPlugin>> ticketPlugins = new List<LoadedPlugin<ITicketPlugin>>();
            List<LoadedPlugin<IPrinterPlugin>> printerPlugins = new List<LoadedPlugin<IPrinterPlugin>>();

            string iPrinterPluginName = typeof(IPrinterPlugin).FullName;
            string iTicketPluginName = typeof(ITicketPlugin).FullName;
            string iPluginName = typeof(IPlugin).FullName;

            string pluginDirectory = Path.Combine(AppDirectory, "Plugins");
            if (!Directory.Exists(pluginDirectory))
            {
                appConnector.OnError($"フォルダ \"{pluginDirectory}\" が見つかりませんでした。", "プラグイン読込エラー", ErrorType.Error);
                return new PluginList(plugins, ticketPlugins, printerPlugins);
            }

            List<string> pluginPaths = LoadPluginLists(pluginDirectory);

            string[] pluginDlls = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.AllDirectories);
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
                catch (ReflectionTypeLoadException ex)
                {
                    Exception[] loaderExceptions = ex.LoaderExceptions;
                }
                catch (FileLoadException ex)
                {
                    if (ex.InnerException is NotSupportedException && pluginPaths.Contains(dll))
                    {
                        appConnector.OnError($"プラグインDLL \"{dll}\" はブロックされているため、読み込めませんでした。ファイルのプロパティからブロックを解除して下さい。", "プラグイン読込エラー", ErrorType.Warning);
                    }
                }
                catch { }
            }

            string printerDirectory = Path.Combine(AppDirectory, "Printers");
            if (!Directory.Exists(printerDirectory))
            {
                appConnector.OnError($"フォルダ \"{printerDirectory}\" が見つかりませんでした。", "プラグイン読込エラー", ErrorType.Error);
                return new PluginList(plugins, ticketPlugins, printerPlugins);
            }

            List<string> printerPaths = LoadPluginLists(printerDirectory);

            string[] printerDlls = Directory.GetFiles(printerDirectory, "*.dll", SearchOption.AllDirectories);
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
                catch (FileLoadException ex)
                {
                    if (ex.InnerException is NotSupportedException && printerPaths.Contains(dll))
                    {
                        appConnector.OnError($"プリンタープラグインDLL {dll} はブロックされているため、読み込めませんでした。ファイルのプロパティからブロックを解除して下さい。", "プラグイン読込エラー", ErrorType.Warning);
                    }
                }
                catch
                { }
            }

            return new PluginList(plugins, ticketPlugins, printerPlugins);
        }

        private static List<string> LoadPluginLists(string baseDirectory)
        {
            string[] pluginLists = Directory.GetFiles(baseDirectory, "*.pluginlist", SearchOption.AllDirectories);
            List<string> pluginPaths = new List<string>();
            foreach (string list in pluginLists)
            {
                string currentDirectory = Path.GetDirectoryName(list);

                using (StreamReader sr = new StreamReader(list))
                {
                    while (!sr.EndOfStream)
                    {
                        string path = Path.Combine(currentDirectory, sr.ReadLine());
                        if (File.Exists(path)) pluginPaths.Add(path);
                    }
                }
            }

            return pluginPaths;
        }
    }
}
