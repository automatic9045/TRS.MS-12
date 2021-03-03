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
using TRS.TMS12.Plugins.TRS;

namespace TRS.TMS12.TicketPlugins.NumberedTickets.NumberedTicket
{
    public class TimeNumberInquirerViewModel : BindableBase
    {
        private PluginInfo m;

        private Visibility _Visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }


        private InputControlTextBox _CurrentTextBox;
        public InputControlTextBox CurrentTextBox
        {
            get { return _CurrentTextBox; }
            set { SetProperty(ref _CurrentTextBox, value); m.CurrentTextBox = (int)CurrentTextBox; }
        }

        private int _SelectedGameIndex;
        public int SelectedGameIndex
        {
            get { return _SelectedGameIndex; }
            set { SetProperty(ref _SelectedGameIndex, value); }
        }

        public ObservableDictionary<InputControlTextBox, string> TextBoxes { get; set; }

        public DelegateCommand<string> GotFocus { get; private set; }

        public DelegateCommand Cancel { get; private set; }
        public DelegateCommand Send { get; private set; }

        public TimeNumberInquirerViewModel(PluginInfo m)
        {
            this.m = m;
            this.m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(m.CurrentTextBox):
                        CurrentTextBox = (InputControlTextBox)m.CurrentTextBox;
                        break;

                    case nameof(m.TimeNumberInquirerStatus):
                        if (m.TimeNumberInquirerStatus == TimeNumberInquirerStatus.PreparingToSend)
                        {
                            DateTime now = DateTime.Now;
                            TextBoxes[InputControlTextBox.TimeNumberInquirerStartHour] = Strings.StrConv(now.Hour.ToString(), VbStrConv.Wide);
                            TextBoxes[InputControlTextBox.TimeNumberInquirerStartMinute] = Strings.StrConv(now.Minute.ToString(), VbStrConv.Wide);

                            Visibility = Visibility.Visible;
                            CurrentTextBox = InputControlTextBox.TimeNumberInquirerEndHour;
                        }
                        else
                        {
                            Visibility = Visibility.Hidden;
                        }
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
                try
                {
                    KeyValuePair<InputControlTextBox, string> textBox = TextBoxes.First(t => m.TextBoxes[(int)t.Key] != t.Value);
                    m.TextBoxes[(int)textBox.Key] = textBox.Value;
                }
                catch { }

                if (TextBoxes[CurrentTextBox].Length == 2)
                {
                    CurrentTextBox++;
                }
            });

            GotFocus = new DelegateCommand<string>(param =>
            {
                CurrentTextBox = (InputControlTextBox)Enum.Parse(typeof(InputControlTextBox), param);
            });

            Cancel = new DelegateCommand(() =>
            {
                m.TimeNumberInquirerStatus = TimeNumberInquirerStatus.Hidden;
            });

            Send = new DelegateCommand(() =>
            {
                m.Times = ((Sender)m.Sender).InquireTimeNumbers((Game)(SelectedGameIndex + 1));
                m.TimeNumberInquirerStatus = m.Times == null ? TimeNumberInquirerStatus.Hidden : TimeNumberInquirerStatus.Sent;
            });
        }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeNumberInquirer : UserControl
    {
        public TimeNumberInquirerViewModel vm;

        public TimeNumberInquirer()
        {
            InitializeComponent();

            DataContextChanged += new DependencyPropertyChangedEventHandler((csender, ce) =>
            {
                vm = (TimeNumberInquirerViewModel)DataContext;

                vm.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
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
            });
        }
    }
}
