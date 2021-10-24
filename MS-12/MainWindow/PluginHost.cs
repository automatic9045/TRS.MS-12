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

using TRS.TMS12.Static;
using TRS.TMS12.Interfaces;
using TRS.TMS12.Resources;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class DialogService : IDialog
    {
        private MainWindowModel mainWindowModel;

        public DialogService(MainWindowModel mainWindowModel)
        {
            this.mainWindowModel = mainWindowModel;
        }


        public void Show(string text, string caption, ImageSource icon, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowDialog(text, caption, icon, buttonIsEnabled);
        }

        public void Show(string text, string caption, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowDialog(text, caption, buttonIsEnabled);
        }


        public void ShowErrorDialog(string text, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowErrorDialog(text, buttonIsEnabled);
        }

        public void ShowWarningDialog(string text, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowWarningDialog(text, buttonIsEnabled);
        }

        public void ShowInformationDialog(string text, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowInformationDialog(text, buttonIsEnabled);
        }

        public void ShowNotImplementedDialog(string text, bool isHelpWanted = false)
        {
            mainWindowModel.DialogModel.ShowNotImplementedDialog(text, isHelpWanted);
        }


        public async Task ShowErrorDialogAsync(string text)
        {
            await mainWindowModel.DialogModel.ShowErrorDialogAsync(text);
        }

        public async Task ShowWarningDialogAsync(string text)
        {
            await mainWindowModel.DialogModel.ShowWarningDialogAsync(text);
        }

        public async Task ShowInformationDialogAsync(string text)
        {
            await mainWindowModel.DialogModel.ShowInformationDialogAsync(text);
        }

        public async Task ShowNotImplementedDialogAsync(string text, bool isHelpWanted = false)
        {
            await mainWindowModel.DialogModel.ShowNotImplementedDialogAsync(text, isHelpWanted);
        }


        public async Task<bool> ShowConfirmDialogAsync(string text)
        {
            return await mainWindowModel.DialogModel.ShowConfirmDialogAsync(text);
        }

        public void Hide()
        {
            mainWindowModel.DialogModel.HideDialog();
        }
    }

    public class PluginHost : BindableBase, IPluginHost
    {
        private MainWindowModel mainWindowModel;

        public IDialog Dialog { get; private set; }

        public PluginHost(MainWindowModel mainWindowModel)
        {
            this.mainWindowModel = mainWindowModel;

            Dialog = new DialogService(mainWindowModel);

            mainWindowModel.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(mainWindowModel.IsOneTimeMode):
                        RaiseModeEnabledChanged(Mode.OneTime, mainWindowModel.IsOneTimeMode);
                        break;
                }
            };

            mainWindowModel.FunctionKeyToggleButtonsIsChecked.CollectionChanged += (sender, e) =>
            {
                switch (mainWindowModel.FunctionKeyToggleButtonsIsChecked.Keys[e.NewStartingIndex])
                {
                    case FunctionKeys.F1:
                        RaiseModeEnabledChanged(Mode.Test, mainWindowModel.FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1]);
                        break;

                    case FunctionKeys.Relay:
                        RaiseModeEnabledChanged(Mode.Relay, mainWindowModel.FunctionKeyToggleButtonsIsChecked[FunctionKeys.Relay]);
                        break;
                }
            };
        }

        public event ModeEnabledChangedEventHandler ModeEnabledChanged;

        public event SendTypeChangedEventHandler SendTypeChanged;

        public List<IPlugin> Plugins
        {
            get => mainWindowModel.Plugins.Plugins.ConvertAll(p => p.Plugin);
        }

        public ITicketPlugin CurrentTicket
        {
            get => mainWindowModel.CurrentTicket;
            set => mainWindowModel.CurrentTicket = value;
        }

        public IPrinterPlugin CurrentPrinter
        {
            get => mainWindowModel.CurrentPrinter;
            set => mainWindowModel.CurrentPrinter = value;
        }

        public List<List<TicketInfo>> AllSentTickets
        {
            get => mainWindowModel.AllSentTickets;
        }

        public List<IssueReservableSendResult> ReservedResults
        {
            get => mainWindowModel.ReservedResults;
        }


        public bool IsOneTimeMode
        {
            get => mainWindowModel.IsOneTimeMode;
            set => mainWindowModel.IsOneTimeMode = value;
        }

        public bool IsTestMode
        {
            get => mainWindowModel.FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1];
        }

        public SendTypes? SendType
        {
            get => mainWindowModel.SendType;
            set => mainWindowModel.SendType = value;
        }


        public void ThrowError(string text, string caption)
        {
            mainWindowModel.AppConnector.OnError(text, caption, ErrorType.Error);
        }

        public void ThrowWarning(string text, string caption)
        {
            mainWindowModel.AppConnector.OnError(text, caption, ErrorType.Warning);
        }

        public void ThrowInformation(string text, string caption)
        {
            mainWindowModel.AppConnector.OnError(text, caption, ErrorType.Information);
        }

        public void GoToSideMenu()
        {
            mainWindowModel.CurrentScreen = Screen.None;
            DoEvents();
            mainWindowModel.CurrentScreen = Screen.GroupMenu;
        }

        public int GetIssueNumber()
        {
            int count = mainWindowModel.AllSentTickets.Count + 1;
            return /*mainWindowModel.IsOfflineMode ? ((count % 3 + 7) * 10000) + count : */count % 7 * 10000 + count;
        }

        public KeyTab CreateKeyTabFromFile(string path, int keyCount = 60)
        {
            return KeyLayoutLoader.LoadFromXmlFile(path, keyCount, ThrowError, ThrowWarning, ThrowInformation);
        }

        public void CurrentTicketChanged()
        {
            RaisePropertyChanged(nameof(CurrentTicket));
        }

        public void CurrentPrinterChanged()
        {
            RaisePropertyChanged(nameof(CurrentPrinter));
        }

        public void RaiseModeEnabledChanged(Mode targetMode, bool isModeEnabled)
        {
            if (ModeEnabledChanged is null) return;
            ModeEnabledChanged(new ModeEnabledChangedEventArgs(targetMode, isModeEnabled));
        }

        public void RaiseSendTypeChanged(SendTypes? newSendType)
        {
            if (SendTypeChanged is null) return;
            SendTypeChanged(new SendTypeChangedEventArgs(newSendType));
        }
    }
}
