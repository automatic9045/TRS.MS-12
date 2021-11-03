using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualBasic;

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

                int companyNumber = 3;
                string stationName = "";
                string terminalName = "＠１";

                try
                {
                    string path = Path.Combine(AppDirectory, "Settings", "Terminal.xml");
                    XElement terminalSetting = XDocument.Load(path).Element("TerminalSettings");

                    companyNumber = (int?)terminalSetting.Element("CompanyNumber").Attribute("Value") ?? 3;
                    stationName = (string)terminalSetting.Element("StationName").Attribute("Value") ?? "";
                    terminalName = (string)terminalSetting.Element("TerminalName").Attribute("Value") ?? "＠１";

                    bool hasSettingChanged = false;

                    if (terminalSetting.Element("TerminalName") is null)
                    {
                        terminalSetting.Add(new XElement("TerminalName"));
                        terminalSetting.Element("TerminalName").SetAttributeValue("Value", stationName);
                        hasSettingChanged = true;
                    }

                    if (!stationName.IsValidStationName())
                    {
                        while (true)
                        {
                            stationName = Strings.StrConv(InputBox.Show("発行時に使用する駅名を指定して下さい。\n駅名は最大 6 文字で、半角・数字・小文字・空白は使用出来ません。", "MS-12 初期設定", "＊＊＊駅"), VbStrConv.Wide | VbStrConv.Uppercase);

                            if (stationName.IsValidStationName())
                            {
                                if (MessageBox.Show($"\"{stationName}\" でよろしいですか？", "MS-12 初期設定", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    if (terminalSetting.Element("StationName") is null) terminalSetting.Add(new XElement("StationName"));
                                    terminalSetting.Element("StationName").SetAttributeValue("Value", stationName);
                                    hasSettingChanged = true;
                                    break;
                                }
                            }
                            else
                            {
                                MessageBox.Show($"\"{stationName}\" は無効な駅名であるか、使用不可能な文字列を含んでいます。", "MS-12 初期設定", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                    if (hasSettingChanged)
                    {
                        terminalSetting.Save(path);
                    }
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
