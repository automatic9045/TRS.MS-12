using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TRS.TMS12.Interfaces;

namespace TRS.TMS12
{
    public struct PluginList
    {
        public List<LoadedPlugin<IPlugin>> Plugins { get; private set; }
        public List<LoadedPlugin<ITicketPlugin>> TicketPlugins { get; private set; }
        public List<LoadedPlugin<IPrinterPlugin>> PrinterPlugins { get; private set; }

        public PluginList(List<LoadedPlugin<IPlugin>> plugins, List<LoadedPlugin<ITicketPlugin>> ticketPlugins, List<LoadedPlugin<IPrinterPlugin>> printerPlugins)
        {
            Plugins = plugins;
            TicketPlugins = ticketPlugins;
            PrinterPlugins = printerPlugins;
        }
    }
}
