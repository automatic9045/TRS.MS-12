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
                        bool isFirstReservation = false;
                        if (!PluginHost.IsOneTimeMode && SendType == SendTypes.Reserve)
                        {
                            PluginHost.IsOneTimeMode = true;
                            isFirstReservation = true;
                        }

                        Func<int, int, List<TicketBase>> createTicketsFunc = (issueNumber, countStartNumber) =>
                        {
                            NativeEventTicket ticket = new NativeEventTicket(new IssueInformation() { TerminalName = StationName + TerminalName, Number = CompanyNumber }, new EventTicketInformation()
                            {
                                Title = "普通入場券",
                                Product = StationName,
                                Description = "旅客車内に立ち入ることはできません。",
                                Amount_Adult = 140,
                                Amount_Child = 70,
                                ValidType = TicketValidTypes.Once,
                                UseDate = new DateTime(now.Year, month, day),
                                Persons_Adult = customer.Adult + customer.Student,
                                Persons_Child = customer.Child + customer.Preschooler,
                                IssuedDate = now,
                                IssueNumber = issueNumber,
                                CountBeginNumber = countStartNumber,
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

                        SendResult result = IsOneTimeMode ?
                            (SendResult)IssueReservableSendResult.Yes(createTicketsFunc, "", "", false) :
                            IssuableSendResult.Yes(createTicketsFunc(PluginHost.GetIssueNumber(), 1), "", "", false);

                        if (SendType == SendTypes.Reserve)
                        {
                            result.Text = "＃" + Strings.StrConv($"{PluginHost.ReservedResults.Count + 1}", VbStrConv.Wide);
                        }
                        if (isFirstReservation) result.Message = "一括一件開始しました";

                        return result;
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
