using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Microsoft.PointOfService;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Static;

namespace TRS.TMS12.PrinterPlugins.Epson.TML90
{
    public class Printer58 : IPrinterPlugin
    {
        public string PrinterName { get; } = "EPSON TM-L90 W58mm";
        public int PrintWidth { get; } = PrintWidthes.Width58;

        public IPluginHost PluginHost { get; set; }

        private PosExplorer posExplorer;
        private PosPrinter posPrinter;

        public void Initialize(string printerName)
        {
            posExplorer = new PosExplorer();

            DeviceInfo deviceInfo = posExplorer.GetDevice(DeviceType.PosPrinter, printerName);
            posPrinter = (PosPrinter)posExplorer.CreateInstance(deviceInfo);

            posPrinter.Open();
            posPrinter.Claim(1000);
            posPrinter.DeviceEnabled = true;

            posPrinter.MapMode = MapMode.Dots;
            posPrinter.RecLetterQuality = true;
        }

        public void Dispose()
        {
            posPrinter.DeviceEnabled = false;
            posPrinter.Release();
            posPrinter.Close();
        }

        public void Print(List<TicketBase> tickets, int issueingNumber, Action<int> onPrint, Action<Exception, int> onError)
        {
            try
            {
                posPrinter.PrintNormal(PrinterStation.Receipt, $"{issueingNumber} {tickets.Count}枚");
            }
            catch (Exception ex)
            {
                onError(ex, 0);
                return;
            }

            List<(string, string)> bmpPaths = tickets.ConvertAll(t =>
            {
                Bitmap sourceBmp = (Bitmap)t.Bitmap.Clone();
                sourceBmp.RotateFlip(RotateFlipType.Rotate270FlipNone);

                Bitmap bmp1 = sourceBmp.Clone(new Rectangle((sourceBmp.Width - 364) / 2, 0, 364, 90), PixelFormat.Format1bppIndexed);
                Bitmap bmp2 = sourceBmp.Clone(new Rectangle((sourceBmp.Width - 364) / 2, 90, 364, sourceBmp.Height - 90), PixelFormat.Format1bppIndexed);

                string bmp1Path = Path.GetTempFileName();
                string bmp2Path = Path.GetTempFileName();

                bmp1.Save(bmp1Path, ImageFormat.Bmp);
                bmp2.Save(bmp2Path, ImageFormat.Bmp);

                return (bmp1Path, bmp2Path);
            });

            bmpPaths.ForEach((b, i) =>
            {
                try
                {
                    posPrinter.PrintNormal(PrinterStation.Receipt, "\u001b|45uF");
                    posPrinter.PrintBitmap(PrinterStation.Receipt, b.Item1, PosPrinter.PrinterBitmapAsIs, (420 - 364) / 2);
                    posPrinter.PrintNormal(PrinterStation.Receipt, "\u001b|P");
                    posPrinter.PrintBitmap(PrinterStation.Receipt, b.Item2, PosPrinter.PrinterBitmapAsIs, (420 - 364) / 2);
                }
                catch (Exception ex)
                {
                    onError(ex, i);
                    return;
                }

                onPrint(i);
            });

            try
            {
                posPrinter.PrintNormal(PrinterStation.Receipt, "\u001b|135uF");
                posPrinter.PrintNormal(PrinterStation.Receipt, "\u001b|P");
            }
            catch (Exception ex)
            {
                onError(ex, tickets.Count - 1);
                return;
            }
        }
    }
}
