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
    public class StarTSP100II : IPrinterPlugin
    {
        public string PrinterName { get; } = "Star TSP100II ECO (TSP143)";

        public IPluginHost PluginHost { get; set; }

        public void Initialize(string printerName)
        {
            TicketDefinition.OpenPort(printerName, PrintSetting.PrintByStarTSP100II);
        }

        public void Dispose()
        {
            TicketDefinition.ClearPort();
        }

        public void Print(List<Ticket> tickets, Action<int> onPrint, Action<Exception, int> onThrowException, bool isStart = true, bool isEnd = true)
        {
            TicketDefinition.Print(tickets.ConvertAll(t => t.Bitmap), tickets.ConvertAll(t => t.PrintStartPosition), isStart, isEnd, PrintSetting.PrintByStarTSP100II, onPrint, onThrowException);
        }
    }
}
