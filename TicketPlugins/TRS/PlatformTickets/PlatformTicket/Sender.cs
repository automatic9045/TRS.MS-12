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

namespace TRS.TMS12.TicketPlugins.PlatformTickets.PlatformTicket
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
            int month = WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Month]);
            int day = WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Day]);

            CustomersInfo customer = new CustomersInfo(
                WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Adult]),
                0,
                WideStringToInt(m.TextBoxes[(int)InputControlTextBox.Child]),
                0);

            SendResult result = Connector.SendPlatformTicket(month, day, customer, Pay.Cash());
            return result;
        }
    }
}
