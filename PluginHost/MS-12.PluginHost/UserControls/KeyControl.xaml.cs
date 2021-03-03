using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Prism.Mvvm;
using Prism.Commands;

using TRS.TMS12.Interfaces;

namespace TRS.TMS12.Resources
{
    public class KeyTab
    {
        public string Header { get; private set; }
        public List<KeyInfo> KeyList { get; private set; }

        public KeyTab(string header, List<KeyInfo> keyList)
        {
            Header = header;
            KeyList = keyList;
        }
    }

    public class KeyInfo
    {
        public string KeyName { get; set; } = "";
        public string Command { get; set; }
    }

    public class KeyControlModel : BindableBase
    {
        public KeyControlModel(List<KeyTab> tabs, DelegateCommand<string> buttonClickedCommand)
        {
            Tabs = tabs;
            ButtonClickedCommand = buttonClickedCommand;
        }

        public List<List<KeyInfo>> KeyLayout { get; private set; }

        private List<KeyTab> _Tabs;
        public List<KeyTab> Tabs
        {
            get => _Tabs;
            private set => SetProperty(ref _Tabs, value);
        }

        private DelegateCommand<string> _ButtonClickedCommand;
        public DelegateCommand<string> ButtonClickedCommand
        {
            get => _ButtonClickedCommand;
            set => SetProperty(ref _ButtonClickedCommand, value);
        }

        private int _CurrentTab = 0;
        public int CurrentTab
        {
            get => _CurrentTab;
            set => SetProperty(ref _CurrentTab, value);
        }
    }

    public class KeyControlViewModel : BindableBase
    {
        public KeyControlViewModel()
        {
            TabChecked = new DelegateCommand<string>(i => UpdateTab(int.Parse(i)));
        }

        private void UpdateTab(int index)
        {
            List<KeyInfo> keyList = m.Tabs[index].KeyList;
            Keys = keyList.ConvertAll(k => k.KeyName);
            CommandParameters = keyList.ConvertAll(k => k.Command);
        }

        private KeyControlModel m;
        public KeyControlModel M
        {
            get => m;
            set
            {
                m = value;
                m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
                {
                    if (e.PropertyName == nameof(m.CurrentTab))
                    {
                        TabsIsChecked[m.CurrentTab] = true;
                    }
                });

                TabsIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
                {
                    m.CurrentTab = TabsIsChecked.IndexOf(true);
                });

                TabsHeader = new List<string>(m.Tabs.Select(t => t.Header));
                Command = m.ButtonClickedCommand;
                CommandParameters = m.Tabs[0].KeyList.ConvertAll(k => "");
                UpdateTab(0);
            }
        }

        private List<string> _TabsHeader;
        public List<string> TabsHeader
        {
            get => _TabsHeader;
            set => SetProperty(ref _TabsHeader, value);
        }

        public ObservableCollection<bool> TabsIsChecked { get; set; } = new ObservableCollection<bool>() { true, false, false, false };

        private List<string> _Keys;
        public List<string> Keys
        {
            get => _Keys;
            set => SetProperty(ref _Keys, value);
        }

        private List<string> _CommandParameters;
        public List<string> CommandParameters
        {
            get => _CommandParameters;
            set => SetProperty(ref _CommandParameters, value);
        }

        private DelegateCommand<string> _Command;
        public DelegateCommand<string> Command
        {
            get => _Command;
            set => SetProperty(ref _Command, value);
        }

        public DelegateCommand<string> TabChecked { get; private set; }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class KeyControl : UserControl
    {
        public KeyControl()
        {
            InitializeComponent();
        }
    }
}
