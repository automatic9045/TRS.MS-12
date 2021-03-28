using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

using TRS.TMS12.Interfaces;
using TRS.Tickets;
using NativeNumberedTicket = TRS.Tickets.NumberedTicket;

namespace TRS.TMS12.Plugins.TRS
{
    public partial class Connector : IPlugin
    {
        public SendResult CancelNumberedTicket(Game game, int timeCode, int seatCode, int cNumber)
        {
            int number = (int)game * 100 + timeCode;

            dynamic json = null;
            SendResult result = null;
            switch (PluginHost.SendType)
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
                        json = Communicator.Cancel((int)game * 10000 + timeCode * 100 + seatCode, cNumber);
                    }
                    catch (Exception ex)
                    {
                        return SendResult.Error(ex);
                    }

                    try
                    {
                        switch ((string)json.result)
                        {
                            case "yes":
                                result = SendResult.Yes("", "", false);
                                break;

                            case "notReserved":
                                result = SendResult.No("指定された枠は予約されていません");
                                break;

                            case "used":
                                result = SendResult.No("使用済");
                                break;

                            case "wrongCNumber":
                                result = SendResult.Rethink("Ｃ符号誤り");
                                break;

                            case "invalidValue":
                                result = SendResult.Rethink("コード誤り");
                                break;

                            default:
                                result = SendResult.Rethink((string)json.result);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        return SendResult.Error(ex);
                    }
                    break;
            }

            return result;
        }
    }
}
