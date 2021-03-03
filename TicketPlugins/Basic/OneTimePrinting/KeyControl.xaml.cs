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

using IOPath = System.IO;

using Prism.Mvvm;
using Prism.Commands;

using TRS.TMS12.Resources;

namespace TRS.TMS12.TicketPlugins.OneTimePrinting
{
    public class KeyControlViewModel : BindableBase
    {
        private PluginInfo m;

        public Resources.KeyControlViewModel KeyBaseViewModel { get; private set; } = new Resources.KeyControlViewModel();

        private Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        public KeyControlViewModel(PluginInfo m)
        {
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

                    case nameof(m.KeyBaseModel):
                        KeyBaseViewModel.M = m.KeyBaseModel;
                        break;
                }
            });
        }
    }

    public partial class KeyControl : UserControl
    {
        public KeyControlViewModel vm;

        public KeyControl(KeyControlViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            DataContext = this.vm;
            Base.DataContext = this.vm.KeyBaseViewModel;
        }
    }
}
