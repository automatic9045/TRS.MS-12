using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace TRS.TMS12.Static
{
    public static class App
    {
        private static FileVersionInfo FileVersionInfo { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
        public static string ProductVersion { get; } = FileVersionInfo.ProductVersion;
        public static string Copyright { get; } = FileVersionInfo.LegalCopyright;
        public static string TicketPluginsNamespace { get; } = "TRS.TMS12.TicketPlugins.";
        public static string PrinterPluginsNamespace { get; } = "TRS.TMS12.PrinterPlugins.";
        public static string AppDirectory { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }

        public static int WideStringToInt(string wideString)
        {
            int number = 0;
            try
            {
                number = int.Parse("0" + Strings.StrConv(wideString, VbStrConv.Narrow));
            }
            catch { }
            return number;
        }
    }
}
