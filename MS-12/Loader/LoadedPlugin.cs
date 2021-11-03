using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TRS.TMS12.Interfaces;

namespace TRS.TMS12
{
    public struct LoadedPlugin<IPlugin>
    {
        public string Path { get; private set; }
        public IPlugin Plugin { get; private set; }

        public LoadedPlugin(IPlugin plugin, string path)
        {
            Path = path;
            Plugin = plugin;
        }
    }
}