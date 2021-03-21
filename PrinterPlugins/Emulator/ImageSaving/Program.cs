using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Static;

namespace TRS.TMS12.PrinterPlugins.Emulators.ImageSaving
{
    public class Printer58 : IPrinterPlugin
    {
        public string PrinterName { get; } = "Image-Saving Printer Emulator W58mm";
        public int PrintWidth { get; } = PrintWidthes.Width58;

        public IPluginHost PluginHost { get; set; }

        private readonly string processCode = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "-");

        public void Initialize(string printerName)
        {
        }

        public void Dispose()
        {
        }

        public void Print(List<TicketBase> tickets, int issueingNumber, Action<int> onPrint, Action<Exception, int> onError)
        {
            tickets.ForEach((t, i) =>
            {
                try
                {
                    Directory.CreateDirectory($"Images\\{processCode}");
                    t.Bitmap.Save($"Images\\{processCode}\\{string.Format("{0:D5}", issueingNumber)}-{string.Format("{0:D2}", i + 1)}.bmp", ImageFormat.Bmp);
                }
                catch (Exception ex)
                {
                    onError(ex, i);
                }

                onPrint(i);
            });
        }
    }
}
