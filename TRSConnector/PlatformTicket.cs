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
        public SendResult SendPlatformTicket(int month, int day, CustomersInfo customer, Pay pay)
        {
            DateTime now = DateTime.Now;

            switch (SendType)
            {
                case SendTypes.Inquire:
                    if (customer.Total > 5)
                    {
                        return SendResult.Rethink("要求人数超過");
                    }
                    else
                    {
                        return SendResult.Yes("", "", false);
                    }

                case SendTypes.Sell:
                case SendTypes.Reserve:
                    if (customer.Total > 5)
                    {
                        return SendResult.Rethink("要求人数超過");
                    }
                    else
                    {
                        Func<List<TicketBase>> createTicketsFunc = () =>
                        {
                            NativeEventTicket ticket = new NativeEventTicket(new IssueInformation() { TerminalName = StationName + TerminalName, Number = CompanyNumber }, new EventTicketInformation()
                            {
                                Title = "普通入場券",
                                Product = StationName,
                                Description = "旅客車内に立ち入ることはできません。",
                                Amount_Adult = 140,
                                ValidType = TicketValidTypes.Once,
                                UseDate = new DateTime(now.Year, month, day),
                                Persons_Adult = customer.Adult + customer.Student,
                                Persons_Child = customer.Child + customer.Preschooler,
                                IssuedDate = now,
                                IssueNumber = PluginHost.AllSentTickets.Count + 1,
                                CountBeginNumber = PluginHost.IsOneTimeMode ? PluginHost.ReservedTickets.Count + 1 : 1,
                                IsWorkingOnInternet = true,
                                InfoTop = IsTestMode ? AdditionalInformation_Top.Test : pay.PayType switch
                                {
                                    PayType.Cash => AdditionalInformation_Top.None,
                                    PayType.IC => AdditionalInformation_Top.IC,
                                    _ => throw new ArgumentOutOfRangeException(),
                                },
                            }, PrintSetting);

                            return ticket.ticketImages.Select((t, i) => (TicketBase)new PlatformTicket(ticket, i)).ToList();
                        };

                        if (IsOneTimeMode)
                        {
                            return IssueReservableSendResult.Yes(createTicketsFunc, "", "", false);
                        }
                        else
                        {
                            return IssuableSendResult.Yes(createTicketsFunc(), "", "", false);
                        }
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class PlatformTicket : TicketBase
    {
        private NativeEventTicket nativeTicket;
        private int index;

        public PlatformTicket(NativeEventTicket nativeTicket, int index) : base(nativeTicket.ticketImages[index])
        {
            this.nativeTicket = nativeTicket;
            this.index = index;
        }

        public override TicketBase Resend() => this; // 本来はカク再付の再製券を作らないといけない
    }
}
