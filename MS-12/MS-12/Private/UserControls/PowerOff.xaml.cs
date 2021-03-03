using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Reflection;
using System.ComponentModel;
using Microsoft.VisualBasic;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Gayak.Collections;

using TRS.TMS12.Interfaces;

namespace TRS.TMS12
{
    public class PowerOffModel : BindableBase, IModel
    {
        public UserControlsConnector UserControlsConnector { get; set; }
        public List<ITicketPlugin> TicketPlugins { get; set; }
        public Dictionary<UserControls, IModel> Models { get; set; }
        public DialogModel DialogModel { get; set; }

        public ObservableCollection<bool> FIsEnabled { get; set; } = new ObservableCollection<bool>()
        {
            false,
            false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,
            false, false, false, false,
            false, false, false,
        };

        public void BeforeShown()
        {

        }
    }

    public class PowerOffViewModel : BindableBase, IViewModel
    {
        public PowerOffViewModel()
        {
            Shutdown = new DelegateCommand(() =>
            {
                /*
                mainWindow.viewmodel.ShowMessageBox("シャットダウンしています。", "プリンター解放中", "情報", false);
                mainWindow.viewmodel.DoEvents();
                TicketDefinition.ClearPort();
                mainWindow.viewmodel.ShowMessageBox("シャットダウンしています。", "カスタマーディスプレイ解放中", "情報", false);
                mainWindow.viewmodel.DoEvents();
                mainWindow.viewmodel.ReleaseLineDisplayPort();
                mainWindow.viewmodel.ShowMessageBox("シャットダウンしています。", "シャットダウン中", "情報", false);
                mainWindow.viewmodel.DoEvents();
                Application.Current.Shutdown();*/

                m.DialogModel.ShowInformationDialog("シャットダウン中……", false);
                Application.Current.Shutdown();
            });

            Start = new DelegateCommand(() =>
            {
                m.UserControlsConnector.SetShowingUserControl(UserControls.None);
                m.UserControlsConnector.SetShowingUserControl(UserControls.MainMenu);
            });
        }

        private PowerOffModel m;
        public IModel Model
        {
            get { return m; }
            set { m = (PowerOffModel)value; }
        }

        private Visibility _Visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        public DelegateCommand Shutdown { get; private set; }
        public DelegateCommand Start { get; private set; }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class PowerOff : UserControl
    {
        public PowerOff()
        {
            InitializeComponent();
        }
    }
}