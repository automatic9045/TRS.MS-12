using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Prism.Mvvm;
using Prism.Commands;

using Gayak.Collections;

using TRS.TMS12.Static;
using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public partial class MainWindowViewModel : BindableBase
    {
        public DialogViewModel DialogViewModel { get; set; }
        public ResultControlViewModel ResultControlViewModel { get; set; }
        public ResultControlViewModel FullScreenResultControlViewModel { get; set; }

        public Dictionary<Screen, IViewModel> ViewModels { get; set; }

        private bool _MainGridIsEnabled = true;
        public bool MainGridIsEnabled
        {
            get { return _MainGridIsEnabled; }
            set { SetProperty(ref _MainGridIsEnabled, value); }
        }

        private bool _MainGridIsEnabled2 = true;
        public bool MainGridIsEnabled2
        {
            get { return _MainGridIsEnabled2; }
            set { SetProperty(ref _MainGridIsEnabled2, value); }
        }

        private bool _InputControlIsEnabled = true;
        public bool InputControlIsEnabled
        {
            get { return _InputControlIsEnabled; }
            set { SetProperty(ref _InputControlIsEnabled, value); }
        }

        private bool _KeyControlIsEnabled = true;
        public bool KeyControlIsEnabled
        {
            get { return _KeyControlIsEnabled; }
            set { SetProperty(ref _KeyControlIsEnabled, value); }
        }

        private Visibility _InputControlVisibility = Visibility.Visible;
        public Visibility InputControlVisibility
        {
            get { return _InputControlVisibility; }
            set { SetProperty(ref _InputControlVisibility, value); DoEvents(); }
        }

        private Visibility _KeyControlVisibility = Visibility.Visible;
        public Visibility KeyControlVisibility
        {
            get { return _KeyControlVisibility; }
            set { SetProperty(ref _KeyControlVisibility, value); DoEvents(); }
        }

        public ObservableDictionary<FunctionKeys, bool> FIsChecked { get; private set; } = new ObservableDictionary<FunctionKeys, bool>()
        {
            { FunctionKeys.F1, false },
            { FunctionKeys.F14, false },
            { FunctionKeys.Relay, false },
            { FunctionKeys.Hold, false },
        };

        public ObservableDictionary<SendTypes, bool> SendTypeButtonsIsChecked { get; private set; } = new ObservableDictionary<SendTypes, bool>()
        {
            { SendTypes.Sell, false },
            { SendTypes.Reserve, false },
            { SendTypes.Inquire, false },
        };

        public ObservableCollection<SolidColorBrush> StepColors { get; private set; }

        private List<UserControl> _TicketInputControls;
        public List<UserControl> TicketInputControls
        {
            get { return _TicketInputControls; }
            set { SetProperty(ref _TicketInputControls, value); }
        }

        private List<UserControl> _TicketKeyControls;
        public List<UserControl> TicketKeyControls
        {
            get { return _TicketKeyControls; }
            set { SetProperty(ref _TicketKeyControls, value); }
        }

        private string _LogBox1 = "";
        public string LogBox1
        {
            get { return _LogBox1; }
            set { SetProperty(ref _LogBox1, value); }
        }

        private string _LogBox2 = "";
        public string LogBox2
        {
            get { return _LogBox2; }
            set { SetProperty(ref _LogBox2, value); }
        }

        private string _LogBox3 = "";
        public string LogBox3
        {
            get { return _LogBox3; }
            set { SetProperty(ref _LogBox3, value); }
        }

        private string _LogBox4 = "";
        public string LogBox4
        {
            get { return _LogBox4; }
            set { SetProperty(ref _LogBox4, value); }
        }

        public DelegateCommand ShowVersionDialog { get; }

        private Dictionary<FunctionKeys, DelegateCommand> _FClicked = new Dictionary<FunctionKeys, DelegateCommand>();
        public Dictionary<FunctionKeys, DelegateCommand> FClicked
        {
            get { return _FClicked; }
            private set { SetProperty(ref _FClicked, value); }
        }
    }
}
