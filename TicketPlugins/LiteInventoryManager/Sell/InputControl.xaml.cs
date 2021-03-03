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
using Microsoft.VisualBasic;

using Prism.Mvvm;
using Prism.Commands;

using Gayak.Collections;

using static TRS.TMS12.Static.App;
using TRS.TMS12.Interfaces;
using TRS.TMS12.Static;

namespace TRS.TMS12.TicketPlugins.LiteInventoryManager.Sell
{
    public class InputControlViewModel : BindableBase
    {
        private PluginInfo m;

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }


        private string _DayOfWeek = "　";
        public string DayOfWeek
        {
            get { return _DayOfWeek; }
            set { SetProperty(ref _DayOfWeek, value); }
        }

        private InputControlTextBox _CurrentTextBox;
        public InputControlTextBox CurrentTextBox
        {
            get { return _CurrentTextBox; }
            set { SetProperty(ref _CurrentTextBox, value); m.CurrentTextBox = (int)CurrentTextBox; }
        }

        public ObservableDictionary<InputControlTextBox, string> TextBoxes { get; set; }
        
        public DelegateCommand TextChanged { get; private set; }


        private int _OptionsIndex = -1;
        public int OptionsIndex
        {
            get => _OptionsIndex;
            set => SetProperty(ref _OptionsIndex, value);
        }

        public ObservableCollection<string> Options { get; set; } = new ObservableCollection<string>();

        public DelegateCommand DeleteOption { get; private set; }


        public DelegateCommand SideMenu { get; private set; }

        public DelegateCommand<string> GotFocus { get; private set; }

        public DelegateCommand InquireTimeNumber { get; private set; }


        private string[] week = new string[] { "日", "月", "火", "水", "木", "金", "土", };

        public InputControlViewModel(PluginInfo m)
        {
            this.m = m;
            this.m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(m.PluginHost):
                        ((BindableBase)this.m.PluginHost).PropertyChanged += new PropertyChangedEventHandler((hsender, he) =>
                        {
                            switch (he.PropertyName)
                            {
                                case nameof(m.PluginHost.CurrentTicket):
                                    Visibility = m.PluginHost.CurrentTicket == this.m ? Visibility.Visible : Visibility.Hidden;
                                    break;
                            }
                        });
                        break;

                    case nameof(m.CurrentTextBox):
                        CurrentTextBox = (InputControlTextBox)m.CurrentTextBox;
                        break;
                }
            });

            m.TextBoxes.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                try
                {
                    TextBoxes[(InputControlTextBox)e.OldStartingIndex] = m.TextBoxes[e.NewStartingIndex];
                }
                catch { }
            });

            TextBoxes = new ObservableDictionary<InputControlTextBox, string>();
            foreach (InputControlTextBox i in Enum.GetValues(typeof(InputControlTextBox)))
            {
                TextBoxes.Add(i, "");
            }

            TextBoxes.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                if ((InputControlTextBox)e.NewStartingIndex == InputControlTextBox.Month || (InputControlTextBox)e.NewStartingIndex == InputControlTextBox.Day)
                {
                    int month = WideStringToInt(TextBoxes[InputControlTextBox.Month]);
                    int date = WideStringToInt(TextBoxes[InputControlTextBox.Day]);
                    string dayOfWeek = "　";
                    try
                    {
                        DateTime today = DateTime.Today;
                        DateTime dateTime = new DateTime(today.Year, month, date);
                        if (dateTime < today) dateTime.AddYears(1);
                        dayOfWeek = week[(int)dateTime.DayOfWeek];
                    }
                    catch { }
                    DayOfWeek = dayOfWeek;
                }

                try
                {
                    KeyValuePair<InputControlTextBox, string> textBox = TextBoxes.First(t => m.TextBoxes[(int)t.Key] != t.Value);
                    m.TextBoxes[(int)textBox.Key] = textBox.Value;
                }
                catch { }

                if (CurrentTextBox != InputControlTextBox.Service && CurrentTextBox != InputControlTextBox.Child && TextBoxes[CurrentTextBox].Length == 2)
                {
                    CurrentTextBox++;
                }
            });

            m.Options.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                Options.Clear();
                Options.AddRange(m.Options.Select(p =>
                {
                    if (p is Option) return ((Option)p).GetEnumDisplayName();
                    else if (p is Discount) return ((Discount)p).GetEnumDisplayName();
                    else throw new ArgumentOutOfRangeException();
                }));
            });

            DeleteOption = new DelegateCommand(() =>
            {
                if (0 <= OptionsIndex && OptionsIndex < Options.Count)
                {
                    m.Options.RemoveAt(OptionsIndex);
                }
            });

            SideMenu = new DelegateCommand(() =>
            {
                m.PluginHost.GoToSideMenu();
            });

            GotFocus = new DelegateCommand<string>(param =>
            {
                CurrentTextBox = (InputControlTextBox)Enum.Parse(typeof(InputControlTextBox), param);
            });

            InquireTimeNumber = new DelegateCommand(() =>
            {
                m.TimeNumberInquirerStatus = TimeNumberInquirerStatus.PreparingToSend;
            });
        }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class InputControl : UserControl
    {
        public InputControlViewModel vm;

        public InputControl(InputControlViewModel vm)
        {
            this.vm = vm;
            InitializeComponent();
            DataContext = this.vm;

            this.vm.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(this.vm.CurrentTextBox):
                        TextBox textBox = FindName(this.vm.CurrentTextBox.ToString()) as TextBox;
                        if (textBox != null)
                        {
                            textBox.Focus();
                        }
                        break;
                }
            });
        }
    }
}
