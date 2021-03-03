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


        public void ShowError(string text, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowErrorDialog(text, buttonIsEnabled);
        }

        public void ShowWarning(string text, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowWarningDialog(text, buttonIsEnabled);
        }

        public void ShowInformation(string text, bool buttonIsEnabled = true)
        {
            mainWindowModel.DialogModel.ShowInformationDialog(text, buttonIsEnabled);
        }

        public void ShowNotImplementedDialog(string text, bool isCollectingInformation = false)
        {
            mainWindowModel.DialogModel.ShowNotImplementedDialog(text, isCollectingInformation);
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
        }

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

        public List<List<TicketInfo>> Tickets
        {
            get => mainWindowModel.Tickets;
        }

        public List<Ticket> ReservedTickets
        {
            get => mainWindowModel.ReservedTickets;
        }


        public bool IsOneTimeMode
        {
            get => mainWindowModel.IsOneTimeMode;
            set => mainWindowModel.IsOneTimeMode = value;
        }

        public void ThrowError(string text, string caption)
        {
            mainWindowModel.UserControlsConnector.Throw(text, caption, ErrorType.Error);
        }

        public void ThrowWarning(string text, string caption)
        {
            mainWindowModel.UserControlsConnector.Throw(text, caption, ErrorType.Warning);
        }

        public void ThrowInformation(string text, string caption)
        {
            mainWindowModel.UserControlsConnector.Throw(text, caption, ErrorType.Information);
        }

        public KeyTab GetKeyLayoutFromFile(string path, int keyCount = 60)
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

        public void ChangeSendType(SendTypes? sendType)
        {
            mainWindowModel.SendType = (SendingType?)sendType;
        }

        public void GoToSideMenu()
        {
            mainWindowModel.ShowingUserControl = UserControls.None;
            DoEvents();
        }
    }
}
