using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

using Codeplex.Data;

using TRS.TMS12.Interfaces;
using TRS.Tickets;

namespace TRS.TMS12.Plugins.TRS
{
    public partial class Connector : IPlugin
    {
        public SendResult Reserve(int year, int count, string discount, Option option)
        {
            dynamic json = null;
            SendResult result = null;
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
                        result = ParseResult(json, true);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            return SendResult.Error(ex, json, ResultTypeStringToEnum(json));
                        }
                        catch (Exception ex2)
                        {
                            return SendResult.Error(ex);
                        }
                    }
                    break;

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
                        result = ParseResult(json, new Func<List<Ticket>>(() =>
                        {
                            EventTicket ticket = new EventTicket(new IssueInformation() { TerminalName = StationName + TerminalName, Number = CompanyNumber }, new EventTicketInformation()
                            {
                                Title = "部誌購入証",
                                Product = "部誌" + Strings.StrConv(year.ToString(), VbStrConv.Wide) + "号",
                                Description = "出札にて保管すること",
                                Amount_Adult = json.price,
                                ValidType = TicketValidTypes.Once,
                                UseDate = DateTime.Parse(json.now),
                                Persons_Adult = 1,
                                IssuedDate = DateTime.Parse(json.now),
                                IssueNumber = 0,
                                IsWorkingOnInternet = true,
                                InfoTop = AdditionalInformation_Top.None,
                                CountBeginNumber = 1,
                            }, PrintSetting);

                            List<Ticket> tickets = new List<Ticket>();
                            for (int i = 0; i < ticket.ticketImages.Length; i++)
                            {
                                tickets.Add(new Ticket(ticket.ticketImages[i], ticket.ticketPrintStartPosition[i]));
                            }

                            return tickets;
                        }));
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            return SendResult.Error(ex, ResultTypeStringToEnum(json));
                        }
                        catch (Exception ex2)
                        {
                            return SendResult.Error(ex);
                        }
                    }
                    break;
            }

            return result;
        }
    }
}
