﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TRS.TMS12.Interfaces;
using TRS.Tickets;
using NativeNumberedTicket = TRS.Tickets.NumberedTicket;

namespace TRS.TMS12.Plugins.TRS
{
    public partial class Connector : IPlugin
    {
        public SendResult Reserve(Game game, int timeCode, int seatCode, int cNumber, CustomersInfo customer, Pay pay, Discount discount, IEnumerable<Option> options)
        {
            string payType = pay.PayType switch
            {
                PayType.Cash => Tickets.Communicator.Cash,
                PayType.IC => pay.ICIDm,
                PayType.Credit => throw new NotImplementedException("クレジットカードによる決済は未実装です。"),
                _ => throw new ArgumentOutOfRangeException(),
            };
            int number = (int)game * 100 + timeCode;

            dynamic json = null;
            SendResult result = null;
            switch (SendType)
            {
                case SendTypes.Inquire:
                    try
                    {
                        json = Communicator.Inquire(number);
                    }
                    catch (Exception ex)
                    {
                        return SendResult.Error(ex);
                    }

                    try
                    {
                        result = ParseResult(json);
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
                    break;

                case SendTypes.Sell:
                case SendTypes.Reserve:
                    try
                    {
                        json = seatCode == -1 ?
                            Communicator.Reserve(number, customer.Total, payType, StationName + TerminalName, IsTestMode) :
                            Communicator.Reserve_Forced(number, seatCode, cNumber, payType, StationName + TerminalName, IsTestMode);
                    }
                    catch (Exception ex)
                    {
                        return SendResult.Error(ex);
                    }

                    try
                    {
                        NativeNumberedTicket ticket = new NativeNumberedTicket(new IssueInformation() { TerminalName = StationName + TerminalName, Number = CompanyNumber }, new NumberedTicketInformation()
                        {
                            StartDate = DateTime.Parse(json.date + " " + json.time1),
                            EndDate = DateTime.Parse(json.date + " " + json.time2),
                            Name = (GameName)game,
                            Persons_Adult = customer.Adult + customer.Student,
                            Persons_Child = customer.Child + customer.Preschooler,
                            ReserveNumber = ((string[])json.number).Select(n => int.Parse(n)).ToArray(),
                            CNumber = ((string[])json.cNumber).Select(n => int.Parse(n)).ToArray(),
                            Forced = seatCode != -1,
                            IssuedDate = DateTime.Parse(json.now),
                            IssueNumber = PluginHost.IsOneTimeMode ? PluginHost.AllSentTickets.IndexOf(null) + 1 : PluginHost.AllSentTickets.Count + 1,
                            CountBeginNumber = PluginHost.IsOneTimeMode ? PluginHost.ReservedTickets.Count + 1 : 1,
                            IsWorkingOnInternet = true,
                            WriteNumberOfPerson = IsOneTimeMode,
                            IsIC = pay.PayType == PayType.IC,
                            DoOmitGuides = options.Contains(Option.OmitGuidePrinting),
                            DoHelp = !options.Contains(Option.NoHelp),
                            IsChanged = options.Contains(Option.Changed),
                            InfoTop = IsTestMode ? AdditionalInformation_Top.Test : pay.PayType switch
                            {
                                PayType.Cash => AdditionalInformation_Top.None,
                                PayType.IC => AdditionalInformation_Top.IC,
                                _ => throw new ArgumentOutOfRangeException(),
                            },
                            InfoBottom = discount switch
                            {
                                Discount.CompanyUse => AdditionalInformation_Bottom.CompanyUse,
                                Discount.Member => AdditionalInformation_Bottom.Staff,
                                Discount.School => AdditionalInformation_Bottom.Student,
                                _ => AdditionalInformation_Bottom.None,
                            },
                        }, PrintSetting);

                        result = ParseResult(json, ticket.ticketImages.Select((t, i) => (TicketBase)new NumberedTicket(ticket, i)).ToList());
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
                    break;
            }

            return result;
        }

        public SendResult Reserve(Game game, int timeCode, CustomersInfo customer, Pay pay, Discount discount, IEnumerable<Option> options) =>
            Reserve(game, timeCode, -1, -1, customer, pay, discount, options);
    }

    public class NumberedTicket : TicketBase
    {
        private NativeNumberedTicket nativeTicket;
        private int index;

        public NumberedTicket(NativeNumberedTicket nativeTicket, int index) : base(nativeTicket.ticketImages[index])
        {
            this.nativeTicket = nativeTicket;
            this.index = index;
        }

        public override TicketBase Resend() => this; // 本来はカク再付の再製券を作らないといけない
    }
}
