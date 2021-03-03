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

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class UserControlsConnector
    {
        private MainWindowModel mainWindowModel;

        public void SetShowingUserControl(UserControls control)
        {
            mainWindowModel.ShowingUserControl = control;
        }

        public void SetCurrentTicket(ITicketPlugin ticket, UserControls from, string option = "")
        {
            mainWindowModel.OriginMenu = from;
            mainWindowModel.CurrentTicket = ticket;
            ticket.SetDefault(option);
        }

        public void Throw(string text, string caption, ErrorType errorType) => mainWindowModel.AppConnector.OnError(text, caption, errorType);

        public UserControlsConnector(MainWindowModel mainWindowModel)
        {
            this.mainWindowModel = mainWindowModel;
        }
    }

    public partial class MainWindowModel : BindableBase
    {
        public static readonly FunctionKeys[] functionKeys = (FunctionKeys[])Enum.GetValues(typeof(FunctionKeys));
        public static readonly int functionKeysCount = functionKeys.Length;
        public static readonly Color stepDone = Color.FromRgb(0x00, 0xff, 0x00);

        public AppConnector AppConnector { get; private set; }

        public UserControlsConnector UserControlsConnector { get; private set; }
        public PluginHost PluginHost { get; private set; }

        public DialogModel DialogModel { get; private set; } = new DialogModel();
        public ResultControlModel ResultControlModel { get; private set; } = new ResultControlModel();
        public ResultControlModel FullScreenResultControlModel { get; private set; } = new ResultControlModel();
        public Dictionary<UserControls, IModel> Models { get; private set; }

        public PluginsInfo Plugins { get; private set; }

        public Dictionary<FunctionKeys, DelegateCommand> FunctionKeyCommands { get; private set; }
        public ObservableDictionary<FunctionKeys, bool> FunctionKeyToggleButtonsIsChecked { get; private set; } = new ObservableDictionary<FunctionKeys, bool>();

        private bool _IsTicketSending = false;
        public bool IsTicketSending
        {
            get => _IsTicketSending;
            set => SetProperty(ref _IsTicketSending, value);
        }

        public List<List<TicketInfo>> Tickets { get; } = new List<List<TicketInfo>>();

        public List<Ticket> ReservedTickets { get; } = new List<Ticket>();

        private bool _IsOneTimeMode = false;
        public bool IsOneTimeMode
        {
            get => _IsOneTimeMode;
            set
            {
                SetProperty(ref _IsOneTimeMode, value);
                if (IsOneTimeMode)
                {
                    Tickets.Add(null);
                }
            }
        }

        private SendingType? _SendType = null;
        public SendingType? SendType
        {
            get => _SendType;
            set => SetProperty(ref _SendType, value);
        }

        private UserControls _ShowingUserControl = UserControls.None;
        public UserControls ShowingUserControl
        {
            get { return _ShowingUserControl; }
            set
            {
                if (value != UserControls.None && value != UserControls.Tickets) Models[value].BeforeShown();
                SetProperty(ref _ShowingUserControl, value);
            }
        }

        private ITicketPlugin _CurrentTicket;
        public ITicketPlugin CurrentTicket
        {
            get { return _CurrentTicket; }
            set
            {
                SetProperty(ref _CurrentTicket, value);
                
                CurrentTicket.Sender.ModeChanged(Mode.TestMode, FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1]);
                CurrentTicket.Sender.ModeChanged(Mode.OneTimeMode, IsOneTimeMode);
                CurrentTicket.Sender.ModeChanged(Mode.SendType, (int?)SendType);
                CurrentTicket.Sender.ModeChanged(Mode.RelayMode, FunctionKeyToggleButtonsIsChecked[FunctionKeys.Relay]);

                PluginHost.CurrentTicketChanged();
            }
        }

        private IPrinterPlugin _CurrentPrinter;
        public IPrinterPlugin CurrentPrinter
        {
            get => _CurrentPrinter;
            set
            {
                SetProperty(ref _CurrentPrinter, value);
                PluginHost.CurrentPrinterChanged();
            }
        }

        public UserControls OriginMenu { get; set; }


        public MainWindowModel(AppConnector appConnector)
        {
            AppConnector = appConnector;

            UserControlsConnector = new UserControlsConnector(this);
            PluginHost = new PluginHost(this);

            AppConnector.ChangeProgressStatus("プラグインを検索しています", Progress.LoadingPlugins);
            Plugins = PluginsLoader.Load();

            LoadedPlugin<IPrinterPlugin> plugin = Plugins.PrinterPlugins.Find(p => p.Plugin.GetType().FullName == PrinterPluginsNamespace + AppConnector.PrinterClass);
            if (plugin != null)
            {
                CurrentPrinter = plugin.Plugin;

                try
                {
                    CurrentPrinter.Initialize(AppConnector.PrinterName);
                }
                catch (Exception ex)
                {
                    AppConnector.OnError("プリンター (プリンタークラス： \"" + AppConnector.PrinterClass + "\"　プリンター名： \"" + AppConnector.PrinterName + "\" ) が見つかりませんでした。", "プリンターエラー", ErrorType.Warning);
                }
            }
            else
            {
                AppConnector.OnError("プリンター \"" + AppConnector.PrinterClass + "\"が見つかりませんでした。", "プリンターエラー", ErrorType.Error);
            }

            foreach (LoadedPlugin<IPlugin> p in Plugins.Plugins)
            {
                AppConnector.ChangeProgressStatus("プラグインを読み込んでいます\n\n　" + p.Path, Progress.LoadingPlugins);
                p.Plugin.PluginHost = PluginHost;
            }
            foreach (LoadedPlugin<ITicketPlugin> p in Plugins.TicketPlugins)
            {
                AppConnector.ChangeProgressStatus("プラグインを読み込んでいます\n\n　" + p.Path, Progress.LoadingPlugins);
                p.Plugin.PluginHost = PluginHost;
            }
            foreach (LoadedPlugin<IPrinterPlugin> p in Plugins.PrinterPlugins)
            {
                AppConnector.ChangeProgressStatus("プラグインを読み込んでいます\n\n　" + p.Path, Progress.LoadingPlugins);
                p.Plugin.PluginHost = PluginHost;
            }
            AppConnector.ChangeProgressStatus("プラグインを読み込んでいます", Progress.LoadingPlugins);
            List<ITicketPlugin> ticketPlugins = Plugins.TicketPlugins.Select(p => p.Plugin).ToList();

            
            AppConnector.ChangeProgressStatus("ページを読み込んでいます", Progress.LoadingPages);
            Models = new Dictionary<UserControls, IModel>()
            {
                { UserControls.PowerOff, new PowerOffModel() },
                { UserControls.MainMenu, new MainMenuModel() },
                { UserControls.GroupMenu, new GroupMenuModel() },
                { UserControls.OneTouchMenu, new OneTouchMenuModel() },
            };
            foreach (KeyValuePair<UserControls, IModel> p in Models)
            {
                IModel model = p.Value;
                model.UserControlsConnector = UserControlsConnector;
                model.Models = Models;
                model.DialogModel = DialogModel;
                model.TicketPlugins = ticketPlugins;
            }

            ResultControlModel.DialogModel = DialogModel;
            FullScreenResultControlModel.DialogModel = DialogModel;

            InitializeFunctionKeys();
        }

        public void Loaded()
        {
            AppConnector.ChangeProgressStatus("メインメニュー・券種メニューのレイアウトを構築しています\n\n　初期化中", Progress.LoadingMainMenuAndGroupMenuLayout);
            ((MainMenuModel)Models[UserControls.MainMenu]).LoadFromFile(AppConnector.MainMenuLayoutSourcePath,
                message => AppConnector.ChangeProgressStatus("メインメニュー・券種メニューのレイアウトを構築しています\n\n　" + message));

            AppConnector.ChangeProgressStatus("ワンタッチメニューのレイアウトを構築しています", Progress.LoadingOneTouchMenuLayout);
            ((OneTouchMenuModel)Models[UserControls.OneTouchMenu]).LoadFromFile(AppConnector.OneTouchMenuLayoutSourcePath);

            AppConnector.ChangeProgressStatus("ウィンドウを表示しています", Progress.PreparingToDisplay);
            ShowingUserControl = UserControls.PowerOff;
        }
    }
}
