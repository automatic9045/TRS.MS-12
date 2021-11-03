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
using TRS.TMS12.Static;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class UserControlHost
    {
        private MainWindowModel mainWindowModel;

        public List<ITicketPlugin> TicketPlugins { get; }
        public IPrinterPlugin CurrentPrinter
        {
            get => mainWindowModel.CurrentPrinter;
        }
        public Dictionary<Screen, IModel> Models { get; }
        public DialogModel DialogModel { get; }

        public void SetCurrentScreen(Screen screen)
        {
            mainWindowModel.CurrentScreen = screen;
        }

        public void SetCurrentTicket(ITicketPlugin ticket, Screen from, string option = "")
        {
            mainWindowModel.OriginMenu = from;
            mainWindowModel.CurrentTicket = ticket;
            ticket.SetDefault(option);
        }

        public void Throw(string text, string caption, ErrorType errorType) => mainWindowModel.AppConnector.OnError(text, caption, errorType);

        public UserControlHost(MainWindowModel mainWindowModel)
        {
            this.mainWindowModel = mainWindowModel;
            Models = mainWindowModel.Models;
            DialogModel = mainWindowModel.DialogModel;
            TicketPlugins = mainWindowModel.Plugins.TicketPlugins.Select(p => p.Plugin).ToList();
        }
    }

    public partial class MainWindowModel : BindableBase
    {
        public static readonly FunctionKeys[] functionKeys = (FunctionKeys[])Enum.GetValues(typeof(FunctionKeys));
        public static readonly int functionKeysCount = functionKeys.Length;
        public static readonly Color stepDone = Color.FromRgb(0x00, 0xff, 0x00);

        public AppConnector AppConnector { get; private set; }

        public UserControlHost UserControlHost { get; private set; }
        public PluginHost PluginHost { get; private set; }

        public DialogModel DialogModel { get; private set; } = new DialogModel();
        public ResultControlModel ResultControlModel { get; private set; } = new ResultControlModel();
        public ResultControlModel FullScreenResultControlModel { get; private set; } = new ResultControlModel();
        public Dictionary<Screen, IModel> Models { get; private set; }

        public PluginList Plugins { get; private set; }

        public Dictionary<FunctionKeys, DelegateCommand> FunctionKeyCommands { get; private set; }
        public ObservableDictionary<FunctionKeys, bool> FunctionKeyToggleButtonsIsChecked { get; private set; } = new ObservableDictionary<FunctionKeys, bool>();

        private bool _IsTicketSending = false;
        public bool IsTicketSending
        {
            get => _IsTicketSending;
            set => SetProperty(ref _IsTicketSending, value);
        }

        public List<List<TicketInfo>> AllSentTickets { get; } = new List<List<TicketInfo>>();

        public List<IssueReservableSendResult> ReservedResults { get; } = new List<IssueReservableSendResult>();

        private bool _IsOneTimeMode = false;
        public bool IsOneTimeMode
        {
            get => _IsOneTimeMode;
            set
            {
                SetProperty(ref _IsOneTimeMode, value);
            }
        }

        private SendTypes? _SendType = null;
        public SendTypes? SendType
        {
            get => _SendType;
            set => SetProperty(ref _SendType, value);
        }

        private Screen _CurrentScreen = Screen.None;
        public Screen CurrentScreen
        {
            get { return _CurrentScreen; }
            set
            {
                if (value != Screen.None && value != Screen.Tickets) Models[value].BeforeShown();
                SetProperty(ref _CurrentScreen, value);
            }
        }

        private ITicketPlugin _CurrentTicket;
        public ITicketPlugin CurrentTicket
        {
            get { return _CurrentTicket; }
            set
            {
                SetProperty(ref _CurrentTicket, value);
                
                PluginHost.RaiseModeEnabledChanged(Mode.Test, FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1]);
                PluginHost.RaiseModeEnabledChanged(Mode.OneTime, IsOneTimeMode);
                PluginHost.RaiseModeEnabledChanged(Mode.Relay, FunctionKeyToggleButtonsIsChecked[FunctionKeys.Relay]);

                PluginHost.RaiseSendTypeChanged(SendType);

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

        public Screen OriginMenu { get; set; }


        public MainWindowModel(AppConnector appConnector)
        {
            AppConnector = appConnector;

            PluginHost = new PluginHost(this);

            AppConnector.ChangeProgressStatus("プラグインを検索しています", Progress.LoadingPlugins);
            Plugins = PluginsLoader.Load(AppConnector);

            LoadedPlugin<IPrinterPlugin> plugin = Plugins.PrinterPlugins.Find(p => p.Plugin.GetType().FullName == PrinterPluginsNamespace + AppConnector.PrinterClass);
            if (!(plugin is null))
            {
                plugin.Plugin.PluginHost = PluginHost;
                CurrentPrinter = plugin.Plugin;

                try
                {
                    CurrentPrinter.Initialize(AppConnector.PrinterName);
                }
                catch (Exception ex)
                {
                    AppConnector.OnError($"プリンター (プリンタークラス： \"{AppConnector.PrinterClass}\"　プリンター名： \"{AppConnector.PrinterName}\" ) の初期化に失敗しました（{ex.GetType().FullName}）。", "プリンターエラー", ErrorType.Warning);
                }
            }
            else
            {
                AppConnector.OnError($"プリンター \"{AppConnector.PrinterClass}\" が見つかりませんでした。", "プリンターエラー", ErrorType.Error);
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

            
            AppConnector.ChangeProgressStatus("ページを読み込んでいます", Progress.LoadingPages);
            Models = new Dictionary<Screen, IModel>()
            {
                { Screen.MainMenu, new MainMenuModel() },
                { Screen.GroupMenu, new GroupMenuModel() },
                { Screen.OneTouchMenu, new OneTouchMenuModel() },
            };
            UserControlHost = new UserControlHost(this);
            foreach (KeyValuePair<Screen, IModel> p in Models)
            {
                IModel model = p.Value;
                model.UserControlHost = UserControlHost;
            }

            ResultControlModel.DialogModel = DialogModel;
            FullScreenResultControlModel.DialogModel = DialogModel;

            InitializeFunctionKeys();
        }

        public void Loaded()
        {
            AppConnector.ChangeProgressStatus("メインメニュー・券種メニューのレイアウトを構築しています\n\n　初期化中", Progress.LoadingMainMenuAndGroupMenuLayout);
            Action<string> changeProgressStatus = message => AppConnector.ChangeProgressStatus("メインメニュー・券種メニューのレイアウトを構築しています\n\n　" + message);
            ((MainMenuModel)Models[Screen.MainMenu]).LoadGroupBoxesFromFile(AppConnector.MainMenuLayoutSourcePath, changeProgressStatus);
            ((MainMenuModel)Models[Screen.MainMenu]).LoadMaintenanceMenuFromFile(AppConnector.MaintenanceMenuLayoutSourcePath, changeProgressStatus);

            AppConnector.ChangeProgressStatus("ワンタッチメニューのレイアウトを構築しています", Progress.LoadingOneTouchMenuLayout);
            ((OneTouchMenuModel)Models[Screen.OneTouchMenu]).LoadFromFile(AppConnector.OneTouchMenuLayoutSourcePath);

            AppConnector.ChangeProgressStatus("ウィンドウを表示しています", Progress.PreparingToDisplay);
            CurrentScreen = Screen.MainMenu;
        }
    }
}
