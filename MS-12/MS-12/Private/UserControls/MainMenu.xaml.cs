using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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
using System.Reflection;
using Microsoft.VisualBasic;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Gayak.Collections;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public partial class MainMenuModel : BindableBase, IModel
    {
        public UserControlsConnector UserControlsConnector { get; set; }
        public List<ITicketPlugin> TicketPlugins { get; set; }
        public Dictionary<UserControls, IModel> Models { get; set; }
        public DialogModel DialogModel { get; set; }

        public ObservableCollection<bool> FIsEnabled { get; set; } = new ObservableCollection<bool>()
        {
            false,
            false, true, true, true, true, true, true, true, true, true, true, true, false, false, false,
            false, false, false, false,
            false, false, false,
        };

        public void BeforeShown()
        {
        }
    }

    public class MainMenuViewModel : BindableBase, IViewModel
    {
        public MainMenuViewModel()
        {
            OneTouchClicked = new DelegateCommand(() =>
            {
                m.UserControlsConnector.SetShowingUserControl(UserControls.None);
                DoEvents();
                m.UserControlsConnector.SetShowingUserControl(UserControls.OneTouchMenu);
            });

            MaintenanceClicked = new DelegateCommand(() =>
            {

            });

            PowerOffClicked = new DelegateCommand(() =>
            {
                m.UserControlsConnector.SetShowingUserControl(UserControls.None);
                DoEvents();
                m.UserControlsConnector.SetShowingUserControl(UserControls.PowerOff);
            });
        }

        private void ModelSet()
        {
            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MainMenuModel.GroupBoxInfos):
                        GroupBoxes = m.GroupBoxInfos;
                        break;
                }
            });
        }

        private MainMenuModel m;
        public IModel Model
        {
            get { return m; }
            set
            {
                m = (MainMenuModel)value;
                ModelSet();
            }
        }

        private Visibility _Visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        private List<List<MainMenuGroupBox>> _GroupBoxes;
        public List<List<MainMenuGroupBox>> GroupBoxes
        {
            get { return _GroupBoxes; }
            set { SetProperty(ref _GroupBoxes, value); }
        }

        public DelegateCommand OneTouchClicked { get; set; }
        public DelegateCommand MaintenanceClicked { get; set; }
        public DelegateCommand PowerOffClicked { get; set; }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();

            DataContextChanged += new DependencyPropertyChangedEventHandler(ViewModelPropertyChanged);
        }
    }
}