using System;
using System.ComponentModel;
using System.Collections.Generic;
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
    public class CharacterKeyControlViewModel : BindableBase
    {
        private ITicketPlugin m;

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        public CharacterKeyControlViewModel(ITicketPlugin m)
        {/*
            this.m = m;
            this.m.PropertyChanged += new PropertyChangedEventHandler((sender2, e2) =>
            {
                switch (e2.PropertyName)
                {
                    case nameof(m.PluginHost):
                        ((BindableBase)this.m.PluginHost).PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
                        {
                            switch (e.PropertyName)
                            {
                                case nameof(m.PluginHost.CurrentTicket):
                                    Visibility = m.PluginHost.CurrentTicket == this.m ? Visibility.Visible : Visibility.Hidden;
                                    break;
                            }
                        });
                        break;
                }
            });*/
        }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class CharacterKeyControl : UserControl
    {
        public CharacterKeyControlViewModel vm;

        public CharacterKeyControl(ITicketPlugin m)
        {
            vm = new CharacterKeyControlViewModel(m);
            InitializeComponent();
            DataContext = vm;
        }
    }
}
