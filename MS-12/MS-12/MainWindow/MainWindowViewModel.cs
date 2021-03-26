using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;
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

using TRS.TMS12.Interfaces;
using TRS.TMS12.Static;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public partial class MainWindowViewModel : BindableBase
    {
        public MainWindowModel M { get; private set; }

        private AppConnector appConnector;

        public MainWindowViewModel(AppConnector appConnector)
        {
            this.appConnector = appConnector;
            M = new MainWindowModel(this.appConnector);

            DialogViewModel = new DialogViewModel(M.DialogModel);
            ResultControlViewModel = new ResultControlViewModel(M.ResultControlModel);
            FullScreenResultControlViewModel = new ResultControlViewModel(M.FullScreenResultControlModel);

            ViewModels = new Dictionary<Screen, IViewModel>()
            {
                { Screen.MainMenu, new MainMenuViewModel() },
                { Screen.GroupMenu, new GroupMenuViewModel() },
                { Screen.OneTouchMenu, new OneTouchMenuViewModel() },
            };
            foreach (KeyValuePair<Screen, IViewModel> p in ViewModels)
            {
                IViewModel vm = p.Value;
                vm.Model = M.Models[p.Key];
            }

            StepColors = new ObservableCollection<SolidColorBrush>()
            {
                Brushes.White,
                Brushes.White,
                Brushes.White,
                Brushes.White,
            };

            ShowVersionDialog = new DelegateCommand(() =>
            {
                M.DialogModel.ShowDialog($"ＴＲＳ係員操作端末　ＭＳ－１２\n\n\nVersion {ProductVersion}\n\n{Copyright}\n\n\n\nバージョン及び著作権の詳細は、取扱説明書をご覧下さい。", "バージョン情報");
            });

            List<ITicketPlugin> ticketPlugins = M.Plugins.TicketPlugins.ConvertAll(p => p.Plugin);
            TicketInputControls = ticketPlugins.ConvertAll(t => t.InputControl);
            TicketKeyControls = ticketPlugins.ConvertAll(t => t.KeyControl);

            FClicked = M.FunctionKeyCommands;
            
            foreach (KeyValuePair<FunctionKeys, bool> item in FIsChecked) M.FunctionKeyToggleButtonsIsChecked.Add(item);


            M.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(M.CurrentScreen):
                        Screen showedControl = Screen.None;
                        try
                        {
                            showedControl = ViewModels.First(p => p.Value.Visibility == Visibility.Visible).Key;
                            ViewModels[(Screen)showedControl].Visibility = Visibility.Hidden;
                        }
                        catch { }

                        if (M.CurrentScreen == Screen.Tickets)
                        {
                        }
                        else if (M.CurrentScreen != Screen.None)
                        {
                            IModel model = ViewModels[M.CurrentScreen].Model;
                            ViewModels[M.CurrentScreen].Visibility = Visibility.Visible;
                        }

                        if (M.CurrentScreen == Screen.None)
                        {
                            MainGridIsEnabled = false;
                            DoEvents();
                        }
                        else if (showedControl == Screen.None)
                        {
                            MainGridIsEnabled = true;
                        }
                        break;

                    case nameof(M.IsTicketSending):
                        MainGridIsEnabled2 = !M.IsTicketSending;
                        DoEvents();
                        break;

                    case nameof(M.SendType):
                        foreach (SendTypes type in Enum.GetValues(typeof(SendTypes)))
                        {
                            SendTypeButtonsIsChecked[type] = M.SendType == type;
                        }
                        break;

                    case nameof(M.IsOneTimeMode):
                        LogBox4 = M.IsOneTimeMode ? "一件操作中" : "";
                        break;
                }
            });

            M.ResultControlModel.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                if (e.PropertyName == nameof(M.ResultControlModel.SendResult))
                {
                    if (M.ResultControlModel.IsVisible)
                    {
                        KeyControlVisibility = Visibility.Hidden;
                        InputControlIsEnabled = false;
                        KeyControlIsEnabled = false;
                    }
                    else
                    {
                        KeyControlVisibility = Visibility.Visible;
                        InputControlIsEnabled = true;
                        KeyControlIsEnabled = true;
                    }
                    DoEvents();
                }
            });

            M.FullScreenResultControlModel.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                if (e.PropertyName == nameof(M.FullScreenResultControlModel.SendResult))
                {
                    if (M.FullScreenResultControlModel.IsVisible)
                    {
                        InputControlVisibility = Visibility.Hidden;
                        KeyControlVisibility = Visibility.Hidden;
                        InputControlIsEnabled = false;
                        KeyControlIsEnabled = false;
                    }
                    else
                    {
                        InputControlVisibility = Visibility.Visible;
                        KeyControlVisibility = Visibility.Visible;
                        InputControlIsEnabled = true;
                        KeyControlIsEnabled = true;
                    }
                    DoEvents();
                }
            });

            M.FunctionKeyToggleButtonsIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                try
                {
                    FunctionKeys key = M.FunctionKeyToggleButtonsIsChecked.First(c => c.Value != FIsChecked[c.Key]).Key;
                    FIsChecked[key] = M.FunctionKeyToggleButtonsIsChecked[key];
                }
                catch { }
            });

            FIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                try
                {
                    FunctionKeys key = FIsChecked.First(c => c.Value != M.FunctionKeyToggleButtonsIsChecked[c.Key]).Key;

                    M.FunctionKeyToggleButtonsIsChecked[key] = FIsChecked[key];
                }
                catch { }
            });

            SendTypeButtonsIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                SendTypes? currentType = null;
                try
                {
                    currentType = SendTypeButtonsIsChecked.First(b => b.Value).Key;
                }
                catch { }
                M.SendType = currentType;
            });
        }
    }
}
