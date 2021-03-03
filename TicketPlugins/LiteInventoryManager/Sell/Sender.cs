using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;

using Prism.Mvvm;
using Prism.Commands;

using Gayak.Collections;

using TRS.TMS12.Static;
using TRS.TMS12.Interfaces;
using TRS.TMS12.Resources;
using TRS.TMS12.Plugins.TRS;
using RethinkCodes = TRS.TMS12.Plugins.TRS.RethinkCodes;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12.TicketPlugins.NumberedTickets.NumberedTicket
{
    public class Sender : ISender
    {
        private PluginInfo m;

        private Connector _Connector = null;
        private Connector Connector
        {
            get
            {
                if (_Connector == null)
                {
                    _Connector = (Connector)m.PluginHost.Plugins.Find(p => p as Connector != null);
                }

                return _Connector;
            }
        }

        public Sender(PluginInfo m)
        {
            this.m = m;
        }

        public void ModeChanged(Mode mode, object value) => Connector.ModeChanged(mode, value);

        public SendResult Send()
        {
            string value = m.TextBoxes[(int)InputControlTextBox.Service];
            Game game = value switch
            {
                "トレインシミュレーター" => Game.TrainSimulator,
                "模型体験運転" => Game.ModelTrainDriving,
                _ => (Game)WideStringToInt(value),
            };

            int timeCode = WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Number]);

            CustomersInfo customer = new CustomersInfo(
                WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Adult]),
                WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Student]),
                WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Child]),
                WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Preschooler]));

            Discount discount = m.Options.Any(p => p is Discount) ? (Discount)m.Options.First(p => p is Discount) : Discount.None;

            SendResult result = Connector.Reserve(game, timeCode, customer, Pay.Cash(), discount, m.Options.Where(p => p is Option).Select(p => (Option)p));
            return result;
        }

        public List<TimeInquiringInfo> InquireTimeNumbers(Game game)
        {
            return Connector.InquireTimeNumbers(game);
        }
    }
}
