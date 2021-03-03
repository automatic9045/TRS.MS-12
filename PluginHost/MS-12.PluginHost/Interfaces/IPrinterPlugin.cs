using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TRS.TMS12.Interfaces
{
    /// <summary>
    /// 券を印刷するためのインターフェイスです。<see cref="IPlugin"/> を継承しています。
    /// </summary>
    public interface IPrinterPlugin : IPlugin
    {
        /// <summary>
        /// この券種の名称を設定・取得します。
        /// </summary>
        string PrinterName { get; }

        void Initialize(string printerName);

        void Dispose();

        void Print(List<Ticket> tickets, Action<int> onPrint, Action<Exception, int> onThrowException, bool isStart = true, bool isEnd = true);
    }
}
