using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;

using Prism.Mvvm;
using Prism.Commands;

using Gayak.Collections;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Resources;
using TRS.TMS12.Static;

namespace TRS.TMS12.TicketPlugins.OneTimePrinting
{
    public enum InputControlTextBox
    {
    }

    public class PluginInfo : BindableBase, ITicketPlugin
    {
        public readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string TicketName { get; } = "一括発券";

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
                    new KeyTab("", null),
                    new KeyTab("", null),
                    new KeyTab("", null),
                };
                KeyBaseModel = new KeyControlModel(tabs, new DelegateCommand<string>(ButtonClicked));
            }
        }
        public UserControl InputControl { get; private set; }
        public UserControl KeyControl { get; private set; }

        private KeyControlModel _KeyBaseModel;
        public KeyControlModel KeyBaseModel
        {
            get => _KeyBaseModel;
            set => SetProperty(ref _KeyBaseModel, value);
        }

        public ISender Sender { get; private set; }

        public ObservableCollection<string> TextBoxes { get; set; }

        private int _CurrentTextBox = 0;
        public int CurrentTextBox
        {
            get => _CurrentTextBox;
            set => SetProperty(ref _CurrentTextBox, value);
        }

        public ObservableCollection<object> Options { get; private set; } = new ObservableCollection<object>();

        public ObservableCollection<bool> FunctionKeysIsEnabled { get; private set; } = new ObservableCollection<bool>()
        {
            true,
            true, true, true, true, false, false, true, false, true, true, true, true, true, true, true,
            true, false, false, false,
            true, true, false,
        };

        private IEnumerable<InputControlTextBox> textBoxKeys = (IEnumerable<InputControlTextBox>)Enum.GetValues(typeof(InputControlTextBox));

        public PluginInfo()
        {
            TextBoxes = new ObservableCollection<string>();
            int length = Enum.GetValues(typeof(InputControlTextBox)).Length;
            for (int i = 0; i < length; i++)
            {
                TextBoxes.Add("");
            }

            InputControlViewModel inputControlViewModel = new InputControlViewModel(this);
            KeyControlViewModel keyControlViewModel = new KeyControlViewModel(this);

            InputControl = new InputControl(inputControlViewModel);
            KeyControl = new KeyControl(keyControlViewModel);

            Sender = new Sender(this);

            TextBoxes.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {

            });
        }

        public void AddOption(string value)
        {

        }

        public void SetDiscount(string value)
        {

        }

        public void SetDefault(string option = "")
        {
            foreach (InputControlTextBox key in textBoxKeys) TextBoxes[(int)key] = "";

            KeyBaseModel.CurrentTab = 0;
            PluginHost.ChangeSendType(SendTypes.Sell);
            FunctionKeysIsEnabled[(int)FunctionKeys.Send] = PluginHost.IsOneTimeMode;
        }

        public void ClearFocusedAndAfter()
        {
            InputControlTextBox[] textBoxes = (InputControlTextBox[])Enum.GetValues(typeof(InputControlTextBox));
            foreach (InputControlTextBox textBox in textBoxes)
            {
                if (textBox >= (InputControlTextBox)CurrentTextBox) TextBoxes[(int)textBox] = "";
            }
        }

        public void ButtonClicked(string param)
        {
            try
            {

            }
            catch (Exception ex)
            {
                PluginHost.Dialog.ShowError("コマンドの実行に失敗しました。\n\n\n要求コマンド：\n\n" + param + "\n\n\n詳細：\n\n" + ex.ToString());
            }
        }
    }
}
