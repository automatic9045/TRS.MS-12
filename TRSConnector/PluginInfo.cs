using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using TRS.TMS12.Interfaces;
using TRS.Tickets;
using static TRS.TMS12.Static.App;
using System.IO;

namespace TRS.TMS12.Plugins.TRS
{
    public partial class Connector : IPlugin
    {
        private IPluginHost _PluginHost;
        public IPluginHost PluginHost
        {
            get => _PluginHost;
            set
            {
                _PluginHost = value;

                int companyNumber = 0;
                string stationName = "名無駅";
                string terminalName = "";

                try
                {
                    XElement terminalSetting = XDocument.Load(Path.Combine(AppDirectory, "Settings", "Terminal.xml")).Element("TerminalSettings");

                    companyNumber = (int?)terminalSetting.Element("CompanyNumber").Attribute("Value") ?? 0;
                    stationName = (string)terminalSetting.Element("StationName").Attribute("Value") ?? "";
                    terminalName = (string)terminalSetting.Element("TerminalName").Attribute("Value") ?? "";
                }
                catch
                {
                    PluginHost.ThrowWarning("端末名の設定に失敗しました。", "端末名設定エラー");
                }

                Initialize(companyNumber, stationName, terminalName, PluginHost.CurrentPrinter.GetType().Name);
                TicketDefinition.Initialize(Path.Combine(AppDirectory, "JF-Dot-Ayu20.ttf"));
            }
        }
    }
}
