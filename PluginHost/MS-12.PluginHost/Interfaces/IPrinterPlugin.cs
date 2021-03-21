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
    /// 券を印刷するプリンターのインターフェイスです。<see cref="IPlugin"/> を継承しています。
    /// </summary>
    public interface IPrinterPlugin : IPlugin
    {
        /// <summary>
        /// この券種の名称を取得します。
        /// </summary>
        string PrinterName { get; }

        /// <summary>
        /// このプリンターで印刷可能な紙幅を取得します。
        /// </summary>
        int PrintWidth { get; }

        /// <summary>
        /// プリンターを初期化します。
        /// </summary>
        /// <param name="printerName">App.xml で設定されたプリンター名。</param>
        void Initialize(string printerName);

        /// <summary>
        /// プリンターを開放します。
        /// </summary>
        void Dispose();

        /// <summary>
        /// 券を印刷します。
        /// </summary>
        /// <param name="tickets">印刷対象の <see cref="TicketBase"/> のリスト。</param>
        /// <param name="issueingNumber">各発行毎に附番されるユニークな番号。</param>
        /// <param name="onPrint"><see cref="TicketBase"/> の印刷が完了する毎に呼び出すアクション。引数には、印刷が完了した <see cref="TicketBase"/> の <paramref name="tickets"/> におけるインデックスをとります。</param>
        /// <param name="onError">引数には、発生した例外、例外発生時に印刷していた <see cref="TicketBase"/> の <paramref name="tickets"/> におけるインデックスをとります。</param>
        void Print(List<TicketBase> tickets, int issueingNumber, Action<int> onPrint, Action<Exception, int> onError);
    }

    public static class PrintWidthes
    {
        public const int Width58 = 58;
        public const int Width80 = 80;
        public const int WidthB5Shorter = 182 - 10 * 2;
        public const int WidthB5Longer = 257 - 10 * 2;
        public const int WidthA4Shorter = 210 - 10 * 2;
        public const int WidthA4Longer = 297 - 10 * 2;
    }
}
