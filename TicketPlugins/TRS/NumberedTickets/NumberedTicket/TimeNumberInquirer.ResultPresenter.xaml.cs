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
    public class TimeNumberInquirerResultPresenterViewModel : BindableBase
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

        private List<string> _Times;
        public List<string> Times
        {
            get { return _Times; }
            set { SetProperty(ref _Times, value); }
        }

        private string _GameName;
        public string GameName
        {
            get { return _GameName; }
            set { SetProperty(ref _GameName, value); }
        }

        private int _SelectedTimeIndex = 0;
        public int SelectedTimeIndex
        {
            get { return _SelectedTimeIndex; }
            set { SetProperty(ref _SelectedTimeIndex, value); }
        }

        public ObservableDictionary<InputControlTextBox, string> TextBoxes { get; set; }

        public DelegateCommand<string> GotFocus { get; private set; }

        public DelegateCommand Cancel { get; private set; }
        public DelegateCommand Finish { get; private set; }

        public TimeNumberInquirerResultPresenterViewModel(PluginInfo m)
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
                        if (m.TimeNumberInquirerStatus == TimeNumberInquirerStatus.Sent)
                        {
                            GameName = m.Times.Count > 0 ? m.Times[0].Game.GetEnumDisplayName() : "";
                            Times = m.Times.ConvertAll(t => t.String);
                            Visibility = Visibility.Visible;
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

            Finish = new DelegateCommand(() =>
            {
                m.TextBoxes[(int)InputControlTextBox.Service] = m.Times[SelectedTimeIndex].Game.GetEnumDisplayName();
                m.TextBoxes[(int)InputControlTextBox.Number] = Strings.StrConv(m.Times[SelectedTimeIndex].TimeNumber.ToString(), VbStrConv.Wide);
                m.TimeNumberInquirerStatus = TimeNumberInquirerStatus.Hidden;
            }, () => SelectedTimeIndex != -1).ObservesProperty(() => SelectedTimeIndex);
        }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeNumberInquirerResultPresenter : UserControl
    {
        public TimeNumberInquirerResultPresenterViewModel vm;

        public TimeNumberInquirerResultPresenter()
        {
            InitializeComponent();

            DataContextChanged += new DependencyPropertyChangedEventHandler((csender, ce) =>
            {
                vm = (TimeNumberInquirerResultPresenterViewModel)DataContext;

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
