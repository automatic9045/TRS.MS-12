using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Plugins.TRS;
using TRS.Tickets;

namespace TRS.TMS12.PrinterPlugins.TRS
{
    public class EpsonTML90 : IPrinterPlugin
    {
        public string PrinterName { get; } = "EPSON TM-L90";

        public IPluginHost PluginHost { get; set; }

        public void Initialize(string printerName)
        {
            TicketDefinition.OpenPort(printerName, PrintSetting.PrintByEpsonTML90);
        }

        public void Dispose()
        {
            TicketDefinition.ClearPort();
        }

        public void Print(List<Ticket> tickets, Action<int> onPrint, Action<Exception, int> onThrowException, bool isStart = true, bool isEnd = true)
        {
            TicketDefinition.Print(tickets.ConvertAll(t => t.Bitmap), tickets.ConvertAll(t => t.PrintStartPosition), isStart, isEnd, PrintSetting.PrintByEpsonTML90, onPrint, onThrowException);
        }
    }
}
