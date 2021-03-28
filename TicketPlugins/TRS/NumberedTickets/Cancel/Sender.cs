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
using static TRS.TMS12.Static.App;

namespace TRS.TMS12.TicketPlugins.TRSTickets.NumberedTickets.Cancel
{
    public class Sender : ISender
    {
        private PluginInfo m;

        private Connector _Connector = null;
        private Connector Connector
        {
            get
            {
                if (_Connector is null)
                {
                    _Connector = (Connector)m.PluginHost.Plugins.Find(p => p is Connector);
                }

                return _Connector;
            }
        }

        public Sender(PluginInfo m)
        {
            this.m = m;
        }

        public SendResult Send()
        {
            string value = m.TextBoxes[(int)InputControlTextBox.Service];
            Game game = value switch
            {
                "トレインシミュレーター" => Game.TrainSimulator,
                "模型体験運転" => Game.ModelTrainDriving,
                _ => (Game)WideStringToInt(value),
            };

            int timeCode = WideStringToInt(m.TextBoxes[(int)InputControlTextBox.TimeCode]);
            int seatCode = WideStringToInt(m.TextBoxes[(int)InputControlTextBox.SeatCode]);
            int cNumber = WideStringToInt(m.TextBoxes[(int)InputControlTextBox.CNumber]);

            SendResult result = Connector.CancelNumberedTicket(game, timeCode, seatCode, cNumber);
            return result;
        }

        public List<TimeInquiringInfo> InquireTimeNumbers(Game game)
        {
            return Connector.InquireTimeNumbers(game);
        }
    }
}
