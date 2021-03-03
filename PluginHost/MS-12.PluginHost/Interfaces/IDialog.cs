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
    /// アプリケーション独自のダイアログを表示するメソッドを提供します。
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// 汎用的なダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">タイトル バーに表示するキャプション。</param>
        /// <param name="icon">タイトル バーに表示するアイコン。</param>
        /// <param name="buttonIsEnabled">「確認」ボタンが有効かどうかを示す値。</param>
        void Show(string text, string caption, ImageSource icon, bool buttonIsEnabled = true);
        /// <summary>
        /// 汎用的なダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">タイトル バーに表示するキャプション。</param>
        /// <param name="buttonIsEnabled">「確認」ボタンが有効かどうかを示す値。</param>
        void Show(string text, string caption, bool buttonIsEnabled = true);

        /// <summary>
        /// エラー ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="buttonIsEnabled">「確認」ボタンが有効かどうかを示す値。</param>
        void ShowError(string text, bool buttonIsEnabled = true);
        /// <summary>
        /// 警告ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="buttonIsEnabled">「確認」ボタンが有効かどうかを示す値。</param>
        void ShowWarning(string text, bool buttonIsEnabled = true);
        /// <summary>
        /// 情報ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="buttonIsEnabled">「確認」ボタンが有効かどうかを示す値。</param>
        void ShowInformation(string text, bool buttonIsEnabled = true);
        /// <summary>
        /// 未実装であることを示すダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="isCollectingInformation">この機能について、情報提供を受け付けているかを示す値。<see cref="true"/> に設定すると「情報提供お待ちしております。」というメッセージが表示されます。</param>
        void ShowNotImplementedDialog(string text, bool isCollectingInformation = false);
    }
}
