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
using TRS.TMS12.Static;
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

                }, () => CanExecute(FunctionKeys.F1)) },

                { FunctionKeys.F2, CreateObserveDelegateCommand(() =>
                {
                    List<TicketInfo> notPrintedTickets = PluginHost.Tickets.SelectMany(t => t).Where(t => !t.HasPrinted).ToList();

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
                                notPrintedTickets[i].OnPrint();
                                DialogModel.ShowInformationDialog($"再製中\n\n（{i}／{notPrintedTickets.Count}）", false);
                                DoEvents();
                            }, (ex, i) => DialogModel.ShowErrorDialog($"印刷時にエラーが発生しました。\n\n\n詳細：\n\n{ex}"));
                        }
                        catch (Exception ex)
                        {
                            DialogModel.ShowErrorDialog($"プリンターの呼び出しに失敗しました。\n\n\n詳細：\n\n{ex}");
                        }

                        DialogModel.HideDialog();
                    }
                }, () => CanExecute(FunctionKeys.F2)) },

                { FunctionKeys.F3, CreateObserveDelegateCommand(() =>
                {

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
                }, () => (CanExecute(FunctionKeys.F10)) && !IsOneTimeMode).ObservesProperty(() => IsOneTimeMode) },

                { FunctionKeys.F11, CreateObserveDelegateCommand(() =>
                {
                    CurrentScreen = Screen.None;
                    ResultControlModel.Hide();
                    FullScreenResultControlModel.Hide();
                    DoEvents();

                    Screen originMenu;
                    switch (OriginMenu)
                    {
                        case Screen.MainMenu:
                        case Screen.GroupMenu:
                            originMenu = Screen.MainMenu;
                            break;

                        case Screen.OneTouchMenu:
                            originMenu = Screen.OneTouchMenu;
                            break;

                        default:
                            originMenu = Screen.MainMenu;
                            break;
                    }

                    UserControlsConnector.SetCurrentTicket(Plugins.TicketPlugins.Find(t => t.Plugin.TicketName == "一括発券").Plugin, originMenu);

                    CurrentScreen = Screen.Tickets;
                }, () => CanExecute(FunctionKeys.F11)) },

                { FunctionKeys.F12, CreateObserveDelegateCommand(() =>
                {
                    DialogModel.ShowNotImplementedDialog("応答", true);
                }, () => CanExecute(FunctionKeys.F12)) },

                { FunctionKeys.F13, CreateObserveDelegateCommand(() =>
                {
                    CurrentTicket.ClearFocusedAndAfter();
                }, () => CanExecute(FunctionKeys.F13)) },

                { FunctionKeys.F14, CreateObserveDelegateCommand(() =>
                {
                    DialogModel.ShowNotImplementedDialog("クレジット", false);
                }, () => CanExecute(FunctionKeys.F14)) },

                { FunctionKeys.F15, CreateObserveDelegateCommand(() =>
                {
                    DialogModel.ShowNotImplementedDialog("連続加算", false);
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
                    switch (CurrentScreen)
                    {
                        case Screen.GroupMenu:
                        case Screen.OneTouchMenu:
                            CurrentScreen = Screen.None;
                            DoEvents();
                            CurrentScreen = Screen.MainMenu;
                            break;
                        case Screen.Tickets:
                            if ((ResultControlModel.IsVisible || FullScreenResultControlModel.IsVisible) && FunctionKeyToggleButtonsIsChecked[FunctionKeys.Hold])
                            {
                                ResultControlModel.Hide();
                                FullScreenResultControlModel.Hide();
                            }
                            else
                            {
                                CurrentScreen = Screen.None;
                                DoEvents();
                                ResultControlModel.Hide();
                                FullScreenResultControlModel.Hide();
                                CurrentScreen = OriginMenu;
                            }
                            break;
                        default:
                            DialogModel.ShowWarningDialog("画面遷移に失敗しました。");
                            break;
                    }
                }, () => CanExecute(FunctionKeys.Release)) },

                { FunctionKeys.Send, CreateObserveDelegateCommand(async () =>
                {
                    if (CurrentScreen == Screen.Tickets && !(CurrentTicket is null))
                    {
                        IsTicketSending = true;

                        if (SendType == SendTypes.Reserve && !IsOneTimeMode) IsOneTimeMode = true;

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
                        if (!(issuableResult is null))
                        {
                            List<Ticket> tickets = await Task.Run(issuableResult.CreateTicketsMethod);
                            switch (SendType)
                            {
                                case SendTypes.Reserve:
                                    PluginHost.ReservedTickets.AddRange(tickets);
                                    break;

                                case SendTypes.Sell:
                                    List<TicketInfo> ticketInfos = tickets.ConvertAll(t => new TicketInfo(t));
                                    PluginHost.Tickets.Add(ticketInfos);   
                                    try
                                    {
                                        CurrentPrinter.Print(tickets, i =>
                                        {
                                            ticketInfos[i].OnPrint();
                                        }, (ex, i) => DialogModel.ShowErrorDialog($"印刷時にエラーが発生しました。\n\n\n詳細：{ex}"));
                                    }
                                    catch (Exception ex)
                                    {
                                        DialogModel.ShowErrorDialog($"プリンターの呼び出しに失敗しました。\n\n\n詳細：\n\n{ex}");
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
                if (CurrentScreen == Screen.Tickets && CurrentTicket != null)
                {
                    switch ((FunctionKeys)e.NewStartingIndex)
                    {
                        case FunctionKeys.F1:
                            CurrentTicket.Sender.OnChangeMode(Mode.Test, FunctionKeyToggleButtonsIsChecked[FunctionKeys.F1]);
                            break;

                        case FunctionKeys.F14:
                            //CurrentTicket.Sender.ModeChanged(Mode., FunctionKeyToggleButtonsIsChecked[FunctionKeys.F14]);
                            break;

                        case FunctionKeys.Relay:
                            CurrentTicket.Sender.OnChangeMode(Mode.Relay, FunctionKeyToggleButtonsIsChecked[FunctionKeys.F14]);
                            break;
                    }
                }
            });

            PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                if (CurrentScreen == Screen.Tickets && CurrentTicket != null && e.PropertyName == nameof(SendType))
                {
                    CurrentTicket.Sender.OnChangeMode(Mode.Send, (int?)SendType);
                }
            });

            foreach (ITicketPlugin ticketPlugin in Plugins.TicketPlugins.Select(p =>p.Plugin))
            {
                ticketPlugin.FunctionKeysIsEnabled.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
                {
                    FunctionKeyCommands[FunctionKeys.Send].RaiseCanExecuteChanged();
                });
            }

            Models.Values.ForEach(m =>
            {
                m.FIsEnabled.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
                {
                    FunctionKeyCommands[FunctionKeys.Send].RaiseCanExecuteChanged();
                });
            });
        }

        private bool CanExecute(FunctionKeys key)
        {
            switch (CurrentScreen)
            {
                case Screen.None:
                    return false;

                case Screen.Tickets:
                    return CurrentTicket.FunctionKeysIsEnabled[(int)key];

                default:
                    return Models[CurrentScreen].FIsEnabled[(int)key];
            }
        }

        private DelegateCommand CreateObserveDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            return new DelegateCommand(executeMethod, canExecuteMethod)
                .ObservesProperty(() => CurrentTicket)
                .ObservesProperty(() => CurrentScreen);
        }
    }
}
