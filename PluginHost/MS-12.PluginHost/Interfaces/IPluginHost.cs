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
    /// <summary>
    /// アプリケーションとプラグインの間でやりとりするためのメソッド、プロパティを提供します。
    /// </summary>
    public interface IPluginHost
    {
        /// <summary>
        /// アプリケーション独自のダイアログを表示する為のメソッドを提供する <see cref="IDialog"/> を取得します。
        /// </summary>
        IDialog Dialog { get; }

        /// <summary>
        /// アプリケーションでインスタンスを作成した <see cref="IPlugin"/> を取得します。
        /// </summary>
        List<IPlugin> Plugins { get; }

        /// <summary>
        /// 現在表示している券種を取得・設定します。
        /// </summary>
        ITicketPlugin CurrentTicket { get; set; }

        /// <summary>
        /// 現在使用しているプリンターを取得・設定します。
        /// </summary>
        IPrinterPlugin CurrentPrinter { get; set; }

        /// <summary>
        /// 現在一件操作中であるかを取得・設定します。
        /// </summary>
        bool IsOneTimeMode { get; set; }

        /// <summary>
        /// 現在営業試験中であるかを取得・設定します。
        /// </summary>
        bool IsTestMode { get; }

        /// <summary>
        /// 現在の操作種別を取得・設定します。
        /// </summary>
        SendTypes? SendType { get; set; }

        /// <summary>
        /// アプリケーションを起動してからこれまでに発信された全ての <see cref="TicketBase"/> の <see cref="TicketInfo"/> のリストを取得します。
        /// </summary>
        List<List<TicketInfo>> AllSentTickets { get; }

        /// <summary>
        /// 一括一件操作において予約され、まだ発券されていない <see cref="IssueReservableSendResult"/> のリストを取得します。
        /// </summary>
        List<IssueReservableSendResult> ReservedResults { get; }

        /// <summary>
        /// 各モードの有効・無効が変更されたときに発生します。
        /// </summary>
        event ModeEnabledChangedEventHandler ModeEnabledChanged;

        /// <summary>
        /// 操作種別が変更されたときに発生します。
        /// </summary>
        event SendTypeChangedEventHandler SendTypeChanged;

        /// <summary>
        /// データ読込時にエラーを表示します。<br />
        /// エラーが一つでも発生した場合、パラメーターで設定したメッセージが表示され、ユーザーが実行しているデータの読込は強制的に中止されます。<br />
        /// 実行中にエラーを表示させるには <see cref="IDialog.ShowErrorDialog(string, bool)"/> メソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">エラーの概要を表すキャプション。</param>
        void ThrowError(string text, string caption);
        /// <summary>
        /// データ読込時に警告を表示します。<br />
        /// 実行中に警告を表示させるには <see cref="IDialog.ShowWarningDialog(string, bool)"/> メソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">警告の概要を表すキャプション。</param>
        void ThrowWarning(string text, string caption);
        /// <summary>
        /// データ読込時に情報を表示します。<br />
        /// 実行中に情報を表示させるには <see cref="IDialog.ShowInformationDialog(string, bool)"/> メソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">情報の概要を表すキャプション。</param>
        void ThrowInformation(string text, string caption);

        /// <summary>
        /// サイドメニューに画面を遷移します。
        /// </summary>
        void GoToSideMenu();

        /// <summary>
        /// 次に発売する券の発行番号を取得します。
        /// </summary>
        /// <returns>発行番号。</returns>
        int GetIssueNumber();

        /// <summary>
        /// キーレイアウトを表現する <see cref="List<KeyInfo>"/> を XML ファイルから生成します。
        /// </summary>
        /// <param name="path">XML ファイルのパス。</param>
        /// <param name="keyCount">ボタンの個数。</param>
        /// <returns>キーレイアウトを表現する <see cref="List<KeyInfo>"/>。</returns>
        KeyTab CreateKeyTabFromFile(string path, int keyCount = 60);
    }
}
