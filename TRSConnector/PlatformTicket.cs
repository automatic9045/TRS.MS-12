using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Microsoft.VisualBasic;

using Codeplex.Data;

using TRS.TMS12.Interfaces;
using TRS.Tickets;
using NativePlatformTicket = TRS.Tickets.PlatformTicket;

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
                        NativePlatformTicket ticket = new NativePlatformTicket(new IssueInformation() { TerminalName = StationName + TerminalName, Number = CompanyNumber }, new PlatformTicketInformation()
                        {
                            ValidType = TicketValidTypes.Once,
                            UseDate = new DateTime(now.Year, month, day),
                            Persons_Adult = 1,
                            IssuedDate = now,
                            IssueNumber = 0,
                            IsWorkingOnInternet = true,
                            InfoTop = AdditionalInformation_Top.None,
                            CountBeginNumber = 1,
                        }, PrintSetting);

                        return IssuableSendResult.Yes(ticket.ticketImages.Select((t, i) => (TicketBase)new PlatformTicket(ticket, i)).ToList(), "", "", false);
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class PlatformTicket : TicketBase
    {
        private NativePlatformTicket nativeTicket;
        private int index;

        public PlatformTicket(NativePlatformTicket nativeTicket, int index) : base(nativeTicket.ticketImages[index])
        {
            this.nativeTicket = nativeTicket;
            this.index = index;
        }

        public override TicketBase Resend() => this; // 本来はカク再付の再製券を作らないといけない
    }
}
