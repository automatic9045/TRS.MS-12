using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using StarMicronics.StarIO;
using StarMicronics.StarIOExtension;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Static;

namespace TRS.TMS12.PrinterPlugins.Star.TSP100I
{
    class Printer80 : IPrinterPlugin
    {
        public string PrinterName { get; } = "Star TSP100I W80mm";
        public int PrintWidth { get; } = PrintWidthes.Width80;

        public IPluginHost PluginHost { get; set; }

        private IPort port;

        public void Initialize(string printerName)
        {
            port = Factory.I.GetPort(printerName, "", 20000);
        }

        public void Dispose()
        {
            Factory.I.ReleasePort(port);
        }

        public void Print(List<TicketBase> tickets, int issueingNumber, Action<int> onPrint, Action<Exception, int> onError)
        {
            StarPrinterStatus status = port.BeginCheckedBlock();
            if (status.Offline)
            {

            }

            ICommandBuilder builder = StarIoExt.CreateCommandBuilder(Emulation.StarGraphic);
            builder.BeginDocument();

            tickets.ForEach((t, i) =>
            {
                try
                {
                    int height = 8 * 58 - 85 - 15;
                    // height = 8 * 58 - 25 - 15;

                    Bitmap bmp = t.Bitmap.Clone(new Rectangle((t.Bitmap.Width - 576) / 2, 70, 576, height), t.Bitmap.PixelFormat);

                    builder.AppendBitmapWithAbsolutePosition(bmp, false, 576, false, BitmapConverterRotation.Normal, 0);
                    builder.AppendCutPaper(CutPaperAction.PartialCutWithFeed);

                    bmp.Dispose();

                    onPrint(i);
                }
                catch (Exception ex)
                {
                    onError(ex, i);
                }
            });

            builder.EndDocument();

            byte[] command = builder.Commands;

            uint writtenLength = port.WritePort(command, 0, (uint)command.Length);
            status = port.EndCheckedBlock();
            if (status.Offline)
            {

            }
        }
    }
}
