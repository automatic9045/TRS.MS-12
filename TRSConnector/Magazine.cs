using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.VisualBasic;

using Codeplex.Data;

using TRS.TMS12.Interfaces;
using TRS.Tickets;
using NativeEventTicket = TRS.Tickets.EventTicket;

namespace TRS.TMS12.Plugins.TRS
{
    public partial class Connector : IPlugin
    {
        public SendResult Reserve(int year, int count, string discount, Option option)
        {
            dynamic json = null;
            switch (SendType)
            {
                case SendTypes.Inquire:
                    try
                    {
                        json = Communicator.CheckClubMagazine(year, count);
                    }
                    catch (Exception ex)
                    {
                        return SendResult.Error(ex);
                    }

                    try
                    {
                        return ParseResult(json, true);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            return SendResult.Error(ex, json, ResultTypeStringToEnum(json));
                        }
                        catch
                        {
                            return SendResult.Error(ex);
                        }
                    }

                case SendTypes.Sell:
                case SendTypes.Reserve:
                    try
                    {
                        json = Communicator.BuyClubMagazine(year, count, discount, StationName + TerminalName, IsTestMode);
                    }
                    catch (Exception ex)
                    {
                        return SendResult.Error(ex);
                    }

                    try
                    {
                        return ParseResult(json, new Func<List<TicketBase>>(() =>
                        {
                            NativeEventTicket ticket = new NativeEventTicket(new IssueInformation() { TerminalName = StationName + TerminalName, Number = CompanyNumber }, new EventTicketInformation()
                            {
                                Title = "部誌購入証",
                                Product = "部誌" + Strings.StrConv(year.ToString(), VbStrConv.Wide) + "号",
                                Description = "出札にて保管すること",
                                Amount_Adult = json.price,
                                ValidType = TicketValidTypes.Once,
                                UseDate = DateTime.Parse(json.now),
                                Persons_Adult = 1,
                                IssuedDate = DateTime.Parse(json.now),
                                IssueNumber = PluginHost.AllSentTickets.Count + 1,
                                CountBeginNumber = PluginHost.IsOneTimeMode ? PluginHost.ReservedTickets.Count + 1 : 1,
                                IsWorkingOnInternet = true,
                                InfoTop = AdditionalInformation_Top.None,
                            }, PrintSetting);

                            return ticket.ticketImages.Select((t, i) => (TicketBase)new EventTicket(ticket, i)).ToList();
                        }));
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            return SendResult.Error(ex, ResultTypeStringToEnum(json));
                        }
                        catch
                        {
                            return SendResult.Error(ex);
                        }
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class EventTicket : TicketBase
    {
        private NativeEventTicket nativeTicket;
        private int index;

        public EventTicket(NativeEventTicket nativeTicket, int index) : base(nativeTicket.ticketImages[index])
        {
            this.nativeTicket = nativeTicket;
            this.index = index;
        }

        public override TicketBase Resend() => this; // 本来はカク再付の再製券を作らないといけない
    }
}
