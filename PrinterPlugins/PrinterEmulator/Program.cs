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

namespace TRS.TMS12.PrinterPlugins.Basic
{
    public class PrinterEmulator : IPrinterPlugin
    {
        public string PrinterName { get; } = "Printer Emulator";

        public IPluginHost PluginHost { get; set; }

        public void Initialize(string printerName)
        {
        }

        public void Dispose()
        {
        }

        public void Print(List<Ticket> tickets, Action<int> onPrint, Action<Exception, int> onError, bool isStart, bool isEnd)
        {
            tickets.ForEach((t, i) =>
            {
                try
                {
                    Bitmap bmp = new Bitmap(t.Bitmap);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.DrawLine(Pens.Red, 0, 0, bmp.Width, bmp.Height);
                        g.DrawLine(Pens.Red, 0, bmp.Height, bmp.Width, 0);
                    }

                    Directory.CreateDirectory("Images");
                    bmp.Save($"Images\\{DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "-")}.bmp", ImageFormat.Bmp);

                    onPrint(i);
                }
                catch (Exception ex)
                {
                    onError(ex, i);
                }
            });
        }
    }
}
