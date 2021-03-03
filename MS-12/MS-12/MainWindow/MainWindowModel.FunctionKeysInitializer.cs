using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public partial class MainWindowModel : BindableBase
    {
        public void InitializeFunctionKeys()
        {
            FunctionKeyCommands = new Dictionary<FunctionKeys, DelegateCommand>()
            {
                { FunctionKeys.Menu, CreateObserveDelegateCommand(() =>
                {
                    FunctionKeyCommands[FunctionKeys.Release].Execute();
                }, () => CanExecute(FunctionKeys.Menu)) },


                { FunctionKeys.F1, CreateObserveDelegateCommand(() =>
                {
                    if (FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1]) DialogModel.ShowErrorDialog("テスト");
                }, () => CanExecute(FunctionKeys.F1)) },

                { FunctionKeys.F2, CreateObserveDelegateCommand(() =>
                {
                    List<TicketInfo> notPrintedTickets = new List<TicketInfo>();
                    PluginHost.Tickets.ConvertAll(t => t.FindAll(t => !t.HasBeenPrinted)).ForEach(t => notPrintedTickets.AddRange(t));

                    if (notPrintedTickets.Count == 0)
                    {
                        DialogModel.ShowInformationDialog("回復の必要はありません。");
                    }
                    else
                    {
                        DialogModel.ShowInformationDialog("再製中", false);
                        DoEvents();

                        try
                        {
                            CurrentPrinter.Print(notPrintedTickets.ConvertAll(t => t.Ticket), i =>
                            {
                                notPrintedTickets[i].Printed();
                                DialogModel.ShowInformationDialog("再製中\n\n（" + i.ToString() + "／" + notPrintedTickets.Count + "）", false);
                                DoEvents();
                            }, (ex, i) => DialogModel.ShowErrorDialog("印刷時にエラーが発生しました。\n\n\n詳細：\n\n" + ex.ToString()));
                        }
                        catch (Exception ex)
                        {
                            DialogModel.ShowErrorDialog("プリンターの呼び出しに失敗しました。\n\n\n詳細：\n\n" + ex.ToString());
                        }

                        DialogModel.HideDialog();
                    }
                }, () => CanExecute(FunctionKeys.F2)) },

                { FunctionKeys.F3, CreateObserveDelegateCommand(() =>
                {
                    DialogModel.ShowErrorDialog("テスト");
                }, () => CanExecute(FunctionKeys.F3)) },

                { FunctionKeys.F4, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F4)) },

                { FunctionKeys.F5, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F5)) },

                { FunctionKeys.F6, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F6)) },

                { FunctionKeys.F7, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F7)) },

                { FunctionKeys.F8, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F8)) },

                { FunctionKeys.F9, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F9)) },

                { FunctionKeys.F10, CreateObserveDelegateCommand(() =>
                {
                    IsOneTimeMode = true;
                }, () => CanExecute(FunctionKeys.F10)) },

                { FunctionKeys.F11, CreateObserveDelegateCommand(() =>
                {
                    ShowingUserControl = UserControls.None;
                    ResultControlModel.Hide();
                    FullScreenResultControlModel.Hide();
                    DoEvents();

                    UserControls menuOrigin;
                    switch (OriginMenu)
                    {
                        case UserControls.MainMenu:
                        case UserControls.GroupMenu:
                            menuOrigin = UserControls.MainMenu;
                            break;

                        case UserControls.OneTouchMenu:
                            menuOrigin = UserControls.OneTouchMenu;
                            break;

                        default:
                            menuOrigin = UserControls.MainMenu;
                            break;
                    }

                    UserControlsConnector.SetCurrentTicket(Plugins.TicketPlugins.Find(t => t.Plugin.TicketName == "一括発券").Plugin, menuOrigin);

                    ShowingUserControl = UserControls.Tickets;
                }, () => CanExecute(FunctionKeys.F11)) },

                { FunctionKeys.F12, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.F12)) },

                { FunctionKeys.F13, CreateObserveDelegateCommand(() =>
                {
                    CurrentTicket.ClearFocusedAndLater();
                }, () => CanExecute(FunctionKeys.F13)) },

                { FunctionKeys.F14, CreateObserveDelegateCommand(() =>
                {
                    if (FunctionKeyToggleButtonsIsChecked[FunctionKeys.F14]) DialogModel.ShowErrorDialog("テスト");
                }, () => CanExecute(FunctionKeys.F14)) },

                { FunctionKeys.F15, CreateObserveDelegateCommand(() =>
                {
                    DialogModel.ShowNotImplementedDialog("連続加算");
                }, () => CanExecute(FunctionKeys.F15)) },


                { FunctionKeys.Sell, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.Sell)) },

                { FunctionKeys.Reserve, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.Reserve)) },

                { FunctionKeys.Inquire, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.Inquire)) },

                { FunctionKeys.Relay, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.Relay)) },


                { FunctionKeys.Hold, CreateObserveDelegateCommand(() =>
                {

                }, () => CanExecute(FunctionKeys.Hold)) },

                { FunctionKeys.Release, CreateObserveDelegateCommand(() =>
                {
                    switch (ShowingUserControl)
                    {
                        case UserControls.GroupMenu:
                        case UserControls.OneTouchMenu:
                            ShowingUserControl = UserControls.None;
                            DoEvents();
                            ShowingUserControl = UserControls.MainMenu;
                            break;
                        case UserControls.Tickets:
                            if ((ResultControlModel.IsVisible || FullScreenResultControlModel.IsVisible) && FunctionKeyToggleButtonsIsChecked[FunctionKeys.Hold])
                            {
                                ResultControlModel.Hide();
                                FullScreenResultControlModel.Hide();
                            }
                            else
                            {
                                ShowingUserControl = UserControls.None;
                                DoEvents();
                                ResultControlModel.Hide();
                                FullScreenResultControlModel.Hide();
                                ShowingUserControl = OriginMenu;
                            }
                            break;
                        default:
                            DialogModel.ShowWarningDialog("画面遷移に失敗しました。");
                            break;
                    }
                }, () => CanExecute(FunctionKeys.Release)) },

                { FunctionKeys.Send, CreateObserveDelegateCommand(async () =>
                {
                    if (ShowingUserControl == UserControls.Tickets && CurrentTicket != null)
                    {
                        IsTicketSending = true;

                        if (SendType == SendingType.Reserve && !IsOneTimeMode) IsOneTimeMode = true;

                        SendResult result = await Task.Run(() => CurrentTicket.Sender.Send());
                        if (result.IsFullScreen)
                        {
                            FullScreenResultControlModel.Show(result);
                        }
                        else
                        {
                            ResultControlModel.Show(result);
                        }
                        DoEvents();

                        IssuableSendResult issuableResult = result as IssuableSendResult;
                        if (issuableResult != null)
                        {
                            List<Ticket> tickets = await Task.Run(issuableResult.CreateTicketsMethod);
                            switch (SendType)
                            {
                                case SendingType.Reserve:
                                    PluginHost.ReservedTickets.AddRange(tickets);
                                    break;

                                case SendingType.Sell:
                                    List<TicketInfo> ticketInfos = tickets.ConvertAll(t => new TicketInfo(t));
                                    PluginHost.Tickets.Add(ticketInfos);   
                                    try
                                    {
                                        CurrentPrinter.Print(tickets, i =>
                                        {
                                            ticketInfos[i].Printed();
                                        }, (ex, i) => DialogModel.ShowErrorDialog("印刷時にエラーが発生しました。\n\n\n詳細：\n\n" + ex.ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        DialogModel.ShowErrorDialog("プリンターの呼び出しに失敗しました。\n\n\n詳細：\n\n" + ex.ToString());
                                    }
                                    break;
                            }
                        }
                        IsTicketSending = false;
                    }
                }, () => CanExecute(FunctionKeys.Send) && SendType != null).ObservesProperty(() => SendType) },
            };

            FunctionKeyToggleButtonsIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                if (ShowingUserControl == UserControls.Tickets && CurrentTicket != null)
                {
                    switch ((FunctionKeys)e.NewStartingIndex)
                    {
                        case FunctionKeys.F1:
                            CurrentTicket.Sender.ModeChanged(Mode.TestMode, FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1]);
                            break;

                        case FunctionKeys.F14:
                            //CurrentTicket.Sender.ModeChanged(Mode., FunctionKeyToggleButtonsIsChecked[FunctionKeys.F14]);
                            break;

                        case FunctionKeys.Relay:
                            CurrentTicket.Sender.ModeChanged(Mode.RelayMode, FunctionKeyToggleButtonsIsChecked[FunctionKeys.F14]);
                            break;
                    }
                }
            });

            PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                if (ShowingUserControl == UserControls.Tickets && CurrentTicket != null && e.PropertyName == nameof(SendType))
                {
                    CurrentTicket.Sender.ModeChanged(Mode.SendType, (int?)SendType);
                }
            });

            foreach (ITicketPlugin ticketPlugin in Plugins.TicketPlugins.Select(p =>p.Plugin))
            {
                ticketPlugin.FunctionKeysIsEnabled.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
                {
                    FunctionKeyCommands[FunctionKeys.Send].RaiseCanExecuteChanged();
                });
            }

            foreach (KeyValuePair<UserControls, IModel> model in Models)
            {
                model.Value.FIsEnabled.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
                {
                    FunctionKeyCommands[FunctionKeys.Send].RaiseCanExecuteChanged();
                });
            }
        }

        private bool CanExecute(FunctionKeys key)
        {
            switch (ShowingUserControl)
            {
                case UserControls.None:
                    return false;

                case UserControls.Tickets:
                    return CurrentTicket.FunctionKeysIsEnabled[(int)key];

                default:
                    return Models[ShowingUserControl].FIsEnabled[(int)key];
            }
        }

        private DelegateCommand CreateObserveDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            return new DelegateCommand(executeMethod, canExecuteMethod)
                .ObservesProperty(() => CurrentTicket)
                .ObservesProperty(() => ShowingUserControl);
        }
    }
}
