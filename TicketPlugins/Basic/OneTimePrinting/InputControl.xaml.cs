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

namespace TRS.TMS12.TicketPlugins.OneTimePrinting
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

        private InputControlTextBox _CurrentTextBox;
        public InputControlTextBox CurrentTextBox
        {
            get { return _CurrentTextBox; }
            set { SetProperty(ref _CurrentTextBox, value); m.CurrentTextBox = (int)CurrentTextBox; }
        }

        public ObservableDictionary<InputControlTextBox, string> TextBoxes { get; set; }
        
        public DelegateCommand TextChanged { get; private set; }


        public DelegateCommand SideMenu { get; private set; }

        public DelegateCommand<string> GotFocus { get; private set; }


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

            });

            SideMenu = new DelegateCommand(() =>
            {
                m.PluginHost.GoToSideMenu();
            });

            GotFocus = new DelegateCommand<string>(param =>
            {
                CurrentTextBox = (InputControlTextBox)Enum.Parse(typeof(InputControlTextBox), param);
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
