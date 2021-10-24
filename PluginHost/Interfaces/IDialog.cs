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
        /// 汎用的なダイアログを表示します。<br />
        /// 原則 <see cref="ShowErrorDialog(string, bool)"/> メソッド、<see cref="ShowWarningDialog(string, bool)"/> メソッド、
        /// <see cref="ShowInformationDialog(string, bool)"/> メソッド、<see cref="ShowNotImplementedDialog(string, bool)"/> メソッド、
        /// <see cref="ShowConfirmDialogAsync(string)"/> メソッドを使用し、これらのメソッドで実現できない場合のみこのメソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">タイトル バーに表示するキャプション。</param>
        /// <param name="icon">タイトル バーに表示するアイコン。</param>
        /// <param name="isButtonEnabled">「確認」ボタンを有効にするか。</param>
        void Show(string text, string caption, ImageSource icon, bool isButtonEnabled = true);

        /// <summary>
        /// 汎用的なダイアログを表示します。<br />
        /// 原則 <see cref="ShowErrorDialog(string, bool)"/> メソッド、<see cref="ShowWarningDialog(string, bool)"/> メソッド、
        /// <see cref="ShowInformationDialog(string, bool)"/> メソッド、<see cref="ShowNotImplementedDialog(string, bool)"/> メソッド、
        /// <see cref="ShowConfirmDialogAsync(string)"/> メソッドを使用し、これらのメソッドで実現できない場合のみこのメソッドを使用して下さい。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="caption">タイトル バーに表示するキャプション。</param>
        /// <param name="isButtonEnabled">「確認」ボタンを有効にするか。</param>
        void Show(string text, string caption, bool isButtonEnabled = true);


        /// <summary>
        /// エラーダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="isButtonEnabled">「確認」ボタンを有効にするか。</param>
        void ShowErrorDialog(string text, bool isButtonEnabled = true);

        /// <summary>
        /// 警告ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="isButtonEnabled">「確認」ボタンを有効にするか。</param>
        void ShowWarningDialog(string text, bool isButtonEnabled = true);

        /// <summary>
        /// 情報ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="isButtonEnabled">「確認」ボタンを有効にするか。</param>
        void ShowInformationDialog(string text, bool isButtonEnabled = true);

        /// <summary>
        /// 未実装であることを示すダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="isCollectingInformation">この機能について情報提供を受け付けているか。<see cref="true"/> に設定すると「情報提供お待ちしております。」というメッセージが表示されます。</param>
        void ShowNotImplementedDialog(string text, bool isCollectingInformation = false);


        /// <summary>
        /// 確認ボタンを押すまで待機するエラーダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        Task ShowErrorDialogAsync(string text);

        /// <summary>
        /// 確認ボタンを押すまで待機する警告ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        Task ShowWarningDialogAsync(string text);

        /// <summary>
        /// 確認ボタンを押すまで待機する情報ダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        Task ShowInformationDialogAsync(string text);

        /// <summary>
        /// 確認ボタンを押すまで待機する、未実装であることを示すダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        /// <param name="isCollectingInformation">この機能について情報提供を受け付けているか。<see cref="true"/> に設定すると「情報提供お待ちしております。」というメッセージが表示されます。</param>
        Task ShowNotImplementedDialogAsync(string text, bool isCollectingInformation = false);


        /// <summary>
        /// 中止、確認の 2 つのボタンをもち、どちらかのボタンを押すまで待機するダイアログを表示します。
        /// </summary>
        /// <param name="text">表示するテキスト。</param>
        Task<bool> ShowConfirmDialogAsync(string text);

        void Hide();
    }
}
