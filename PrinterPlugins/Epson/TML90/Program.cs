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
            if (deviceInfo is null)
            {
                PluginHost.ThrowWarning($"指定された論理デバイス名 \"{printerName}\" は不正です。OPOS ADK でこの名前のプリンターが登録されているか確認して下さい。", "プリンター GetDevice エラー");
                return;
            }

            try
            {
                posPrinter = (PosPrinter)posExplorer.CreateInstance(deviceInfo);
            }
            catch (Exception ex)
            {
                PluginHost.ThrowWarning($"不明なエラーが発生しました： {ex.Message}", "プリンター CreateInstance エラー");
            }

            try
            {
                posPrinter.Open();
            }
            catch (PosControlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCode.Illegal:
                        if (ex.Message.Contains("invalid parameter value"))
                        {
                            PluginHost.ThrowWarning($"指定された論理デバイス名 \"{printerName}\" は不正です。OPOS ADK でこの名前のプリンターが登録されているか確認して下さい。", "プリンター Open エラー");
                        }
                        else if (ex.Message.Contains("already open"))
                        {
                            PluginHost.ThrowWarning($"接続処理に失敗しました。プリンターが他のアプリケーションで使用されている可能性があります。", "プリンター Open エラー");
                        }
                        else
                        {
                            PluginHost.ThrowWarning($"不明なエラーが発生しました：【ErrorCode: Illegal】 {ex.Message}", "プリンター Open エラー");
                        }
                        break;

                    case ErrorCode.NoService:
                        PluginHost.ThrowWarning($"接続処理に失敗しました。お使いのプリンターの不具合の可能性があります。", "プリンター Open エラー");
                        break;

                    default:
                        PluginHost.ThrowWarning($"不明なエラーが発生しました：【ErrorCode: {ex.ErrorCode}】 {ex.Message}", "プリンター Open エラー");
                        break;
                }
                return;
            }

            try
            {
                posPrinter.Claim(5000);
            }
            catch (PosControlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case ErrorCode.Closed:
                        PluginHost.ThrowWarning($"ソフトウェアの不具合です。お手数ですが開発者へご連絡下さい。ご連絡の際にはこちらのエラーコードをお伝え下さい：【CA-CO】", "プリンター Claim エラー");
                        break;

                    case ErrorCode.Illegal:
                        PluginHost.ThrowWarning($"プリンターに接続出来ませんでした。", "プリンター Claim エラー");
                        break;

                    case ErrorCode.Timeout:
                        PluginHost.ThrowWarning($"時間内に接続出来ませんでした。プリンターが他のアプリケーションで使用されている可能性があります。", "プリンター Claim エラー");
                        break;

                    case ErrorCode.Failure:
                        PluginHost.ThrowWarning($"不明なエラーが発生しました：【ErrorCode: Failure】 {ex.Message}", "プリンター Claim エラー");
                        break;

                    default:
                        PluginHost.ThrowWarning($"不明なエラーが発生しました：【ErrorCode: {ex.ErrorCode}】 {ex.Message}", "プリンター Claim エラー");
                        break;
                }
                return;
            }

            try
            {
                posPrinter.DeviceEnabled = true;

                posPrinter.MapMode = MapMode.Dots;
                posPrinter.RecLetterQuality = true;
            }
            catch (Exception ex)
            {
                PluginHost.ThrowWarning($"不明なエラーが発生しました：【{ex.GetType().Name}】 {ex.Message}", "プリンターセットアップエラー");
            }
        }

        public void Dispose()
        {
            try
            {
                posPrinter.DeviceEnabled = false;
                posPrinter.Release();
                posPrinter.Close();
            }
            catch { } // 握り潰し
        }

        public void Print(List<TicketBase> tickets, int issuingNumber, Action<int> onPrint, Action<Exception, int> onError)
        {
            string tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectoryPath);

            try
            {
                posPrinter.PrintNormal(PrinterStation.Receipt, $"{issuingNumber} {tickets.Count}枚");
            }
            catch (Exception ex)
            {
                onError(ex, 0);
                DeleteTempFiles();
                return;
            }

            List<(string, string)> bmpPaths = tickets.ConvertAll((t, i) =>
            {
                Bitmap sourceBmp = (Bitmap)t.Bitmap.Clone();
                sourceBmp.RotateFlip(RotateFlipType.Rotate270FlipNone);

                Bitmap bmp1 = sourceBmp.Clone(new Rectangle((sourceBmp.Width - 364) / 2, 0, 364, 90), PixelFormat.Format1bppIndexed);
                Bitmap bmp2 = sourceBmp.Clone(new Rectangle((sourceBmp.Width - 364) / 2, 90, 364, sourceBmp.Height - 90), PixelFormat.Format1bppIndexed);

                string bmp1Path = Path.Combine(tempDirectoryPath, $"{i}-1.bmp");
                string bmp2Path = Path.Combine(tempDirectoryPath, $"{i}-2.bmp");

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
                    DeleteTempFiles();
                    return;
                }

                if (i != tickets.Count - 1) onPrint(i);
            });

            try
            {
                posPrinter.PrintNormal(PrinterStation.Receipt, "\u001b|135uF");
                posPrinter.PrintNormal(PrinterStation.Receipt, "\u001b|P");
            }
            catch (Exception ex)
            {
                onError(ex, tickets.Count - 1);
                DeleteTempFiles();
                return;
            }

            onPrint(tickets.Count - 1);
            DeleteTempFiles();


            void DeleteTempFiles()
            {
                Task.Run(() => Directory.Delete(tempDirectoryPath, true));
            }
        }
    }
}
