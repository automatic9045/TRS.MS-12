using System;
using System.ComponentModel;
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

namespace TRS.TMS12
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel VM { get; private set; }
        private Dictionary<Screen, UserControl> userControls;

        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            VM = vm;
            DataContext = VM;

            Dialog.DataContext = VM.DialogViewModel;
            ResultControl.DataContext = VM.ResultControlViewModel;
            FullScreenResultControl.DataContext = VM.FullScreenResultControlViewModel;

            userControls = new Dictionary<Screen, UserControl>()
            {
                {Screen.MainMenu, MainMenu },
                {Screen.GroupMenu, GroupMenu },
                {Screen.OneTouchMenu, OneTouchMenu },
            };
            foreach (KeyValuePair<Screen, UserControl> p in userControls)
            {
                p.Value.DataContext = VM.ViewModels[p.Key];
            }

            VM.TicketInputControls.ForEach(c => TicketInputControlsGrid.Children.Add(c));
            VM.TicketKeyControls.ForEach(c => TicketKeyControlsGrid.Children.Add(c));
        }
    }
}
