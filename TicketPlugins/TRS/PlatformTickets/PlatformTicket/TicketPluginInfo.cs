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
using TRS.TMS12.Plugins.TRS;

namespace TRS.TMS12.TicketPlugins.PlatformTickets.PlatformTicket
{
    public enum InputControlTextBox
    {
        Month = 0,
        Day,
        Adult,
        Child,
    }

    public class PluginInfo : BindableBase, ITicketPluginExtension
    {
        public readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string TicketName { get; } = "入場券";

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

        public ObservableCollection<bool> FunctionKeysIsEnabled { get; private set; } = new ObservableCollection<bool>()
        {
            true,
            true, true, true, true, false, false, true, false, true, true, true, true, true, true, true,
            true, true, true, true,
            true, true, false,
        };

        private CommandParser commandParser;
        private IEnumerable<InputControlTextBox> textBoxKeys = (IEnumerable<InputControlTextBox>)Enum.GetValues(typeof(InputControlTextBox));

        public PluginInfo()
        {
            TextBoxes = new ObservableCollection<string>();
            int length = Enum.GetValues(typeof(InputControlTextBox)).Length;
            for (int i = 0; i < length; i++)
            {
                TextBoxes.Add("");
            }

            commandParser = new CommandParser(this, typeof(InputControlTextBox));

            InputControlViewModel inputControlViewModel = new InputControlViewModel(this);
            KeyControlViewModel keyControlViewModel = new KeyControlViewModel(this);

            InputControl = new InputControl(inputControlViewModel);
            KeyControl = new KeyControl(keyControlViewModel);

            Sender = new Sender(this);

            TextBoxes.CollectionChanged += (sender, e) =>
            {
                bool canSend =
                    TextBoxes[(int)InputControlTextBox.Month] != "" &&
                    TextBoxes[(int)InputControlTextBox.Day] != "" &&

                    (TextBoxes[(int)InputControlTextBox.Adult] != "" ||
                    TextBoxes[(int)InputControlTextBox.Child] != "");

                FunctionKeysIsEnabled[(int)FunctionKeys.Send] = canSend;
            };
        }

        public void AddOption(string value)
        {
            throw new NotSupportedException();
        }

        public void SetDiscount(string value)
        {
            throw new NotSupportedException();
        }

        public void SetDefault(string option = "")
        {
            foreach (InputControlTextBox key in textBoxKeys) TextBoxes[(int)key] = "";

            KeyBaseModel.CurrentTab = 0;
            CurrentTextBox = (int)InputControlTextBox.Month;

            if (option == "")
            {
                DateTime today = DateTime.Today;
                TextBoxes[(int)InputControlTextBox.Adult] = "１";
                TextBoxes[(int)InputControlTextBox.Month] = Strings.StrConv(today.Month.ToString(), VbStrConv.Wide);
                TextBoxes[(int)InputControlTextBox.Day] = Strings.StrConv(today.Day.ToString(), VbStrConv.Wide);
            }
            else
            {
                commandParser.Parse(option);
            }
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
                commandParser.Parse(param);
            }
            catch (Exception ex)
            {
                PluginHost.Dialog.ShowErrorDialog($"コマンドの実行に失敗しました。\n\n\n要求コマンド：\n\n{param}\n\n\n詳細：\n\n{ex}");
            }
        }
    }
}
