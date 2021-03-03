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
using TRS.TMS12.Resources;

namespace TRS.TMS12.Interfaces
{
    public interface IPluginHost
    {
        /// <summary>
        /// アプリケーション独自のダイアログを表示するメソッドを提供します。
        /// </summary>
        IDialog Dialog { get; }

        /// <summary>
        /// アプリケーションでインスタンスを作成した <see cref="IPlugin"/> を取得します。
        /// </summary>
        List<IPlugin> Plugins { get; }

        /// <summary>
        /// 現在表示している券種を取得・設定します。
        /// </summary>
        ITicketPlugin CurrentTicket { get; }

        /// <summary>
        /// 現在使用しているプリンターを取得・設定します。
        /// </summary>
        IPrinterPlugin CurrentPrinter { get; set; }

        bool IsOneTimeMode { get; set; }

        List<List<TicketInfo>> Tickets { get; }
        List<Ticket> ReservedTickets { get; }

        /// <summary>
        /// データ読込時にエラーを表示します。実行中にエラーを表示させるには <see cref="IDialog.ShowError(string, bool)"/> メソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">エラーの概要を表すキャプション。</param>
        void ThrowError(string text, string caption);
        /// <summary>
        /// データ読込時に警告を表示します。実行中に警告を表示させるには <see cref="IDialog.ShowWarning(string, bool)"/> メソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">警告の概要を表すキャプション。</param>
        void ThrowWarning(string text, string caption);
        /// <summary>
        /// データ読込時に情報を表示します。実行中に情報を表示させるには <see cref="IDialog.ShowInformation(string, bool)"/> メソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">情報の概要を表すキャプション。</param>
        void ThrowInformation(string text, string caption);

        /// <summary>
        /// キーレイアウトを表現する <see cref="List<KeyInfo>"/> を XML ファイルから生成します。
        /// </summary>
        /// <param name="path">XML ファイルのパス。</param>
        /// <param name="keyCount">ボタンの個数。</param>
        /// <returns>キーレイアウトを表現する <see cref="List<KeyInfo>"/>。</returns>
        KeyTab GetKeyLayoutFromFile(string path, int keyCount = 60);

        void ChangeSendType(SendTypes? sendType);

        void GoToSideMenu();
    }
}
