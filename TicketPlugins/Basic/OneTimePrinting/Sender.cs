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
using static TRS.TMS12.Static.App;

namespace TRS.TMS12.TicketPlugins.OneTimePrinting
{
    public class Sender : ISender
    {
        private PluginInfo m;

        public Sender(PluginInfo m)
        {
            this.m = m;
            m.PropertyChanged += (sender, pe) =>
            {
                if (pe.PropertyName == nameof(m.PluginHost))
                {
                    m.PluginHost.ModeEnabledChanged += e =>
                    {
                        if (e.TargetMode == Mode.OneTime) m.FunctionKeysIsEnabled[(int)FunctionKeys.Send] = e.IsModeEnabled;
                    };
                }
            };
        }

        public SendResult Send()
        {
            List<TicketBase> ticketsToPrint = new List<TicketBase>();
            int issueNumber = m.PluginHost.GetIssueNumber();
            m.PluginHost.ReservedResults.ForEach(r =>
            {
                List<TicketBase> tickets = r.CreateTickets(issueNumber, ticketsToPrint.Count + 1);
                ticketsToPrint.AddRange(tickets);
            });

            m.PluginHost.ReservedResults.Clear();
            m.PluginHost.IsOneTimeMode = false;
            m.FunctionKeysIsEnabled[(int)FunctionKeys.Send] = false;

            return IssuableSendResult.Yes(ticketsToPrint, "", "", false);
        }
    }
}
