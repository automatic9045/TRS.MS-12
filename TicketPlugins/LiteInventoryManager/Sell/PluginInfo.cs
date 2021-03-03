using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Prism.Mvvm;
using Prism.Commands;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Resources;

namespace TRS.TMS12.TicketPlugins.LiteInventoryManager.Sell
{
    public enum InputControlTextBox
    {
        Product = 0,
        Count,
    }

    public class PluginInfo : BindableBase, ITicketPluginExtension
    {
        public readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string TicketName { get; } = "商品発売";

        public UserControl InputControl { get; }

        public UserControl KeyControl { get; }

        public KeyControlModel KeyBaseModel { get; private set; }

        public ISender Sender { get; }

        public ObservableCollection<bool> FunctionKeysIsEnabled { get; private set; } = new ObservableCollection<bool>()
        {
            true,
            true, true, true, true, false, false, true, false, true, true, true, true, true, true, true,
            true, true, true, true,
            true, true, false,
        };

        private IPluginHost _PluginHost;
        public IPluginHost PluginHost
        {
            get => _PluginHost;
            set
            {
                SetProperty(ref _PluginHost, value);

                List<KeyTab> tabs = new List<KeyTab>()
                {
                    PluginHost.GetKeyLayoutFromFile(Path.Combine(assemblyPath, @"KeyLayoutDefinitions\FullKey.xml")),
                    PluginHost.GetKeyLayoutFromFile(Path.Combine(assemblyPath, @"KeyLayoutDefinitions\Number.xml")),
                    new KeyTab("", null),
                    new KeyTab("", null),
                };

                KeyBaseModel = new KeyControlModel(tabs, new DelegateCommand<string>(param =>
                {
                    try
                    {
                        commandParser.Parse(param);
                    }
                    catch (Exception ex)
                    {
                        PluginHost.Dialog.ShowError("コマンドの実行に失敗しました。\n\n\n要求コマンド：\n\n" + param + "\n\n\n詳細：\n\n" + ex.ToString());
                    }
                }));
            }
        }

        public int CurrentTextBox { get; set; } = 0;
        public ObservableCollection<string> TextBoxes { get; private set; }

        private CommandParser commandParser;

        public void ClearFocusedAndLater()
        {

        }

        public void SetDefault(string option = "")
        {

        }

        public void AddOption(string value)
        {
            throw new NotImplementedException();
        }

        public void SetDiscount(string value)
        {
            throw new NotImplementedException();
        }

        public PluginInfo()
        {

        }
    }
}