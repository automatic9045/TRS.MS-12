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
    /// 要求画面を有し、アプリケーション内で参照可能な券種を表すインターフェイスです。<see cref="IPlugin"/> を継承しています。
    /// </summary>
    public interface ITicketPlugin : IPlugin
    {
        /// <summary>
        /// この券種の名称を設定・取得します。
        /// </summary>
        string TicketName { get; }

        /// <summary>
        /// 入力可能な要求画面です。画面左側に表示されます。
        /// </summary>
        UserControl InputControl { get; }

        /// <summary>
        /// 要求画面に入力するためのキーを有するガイド画面です。画面右側に表示されます。
        /// </summary>
        UserControl KeyControl { get; }

        /// <summary>
        /// この券種を発信した時の動作を定義する <see cref="ISender"/> です。
        /// </summary>
        ISender Sender { get; }

        /// <summary>
        /// ファンクションキーの有効・無効を設定・取得します。
        /// </summary>
        ObservableCollection<bool> FunctionKeysIsEnabled { get; }

        /// <summary>
        /// この券種を選択したときに呼び出されるメソッドです。ワンタッチメニューから初期値設定付で選択された場合は呼び出されません。
        /// </summary>
        /// <param name="option">初期表示値の指定などのための文字列。省略するか空の文字列を指定するとデフォルトの設定が使用されます。</param>
        void SetDefault(string option = "");

        /// <summary>
        /// 残消去ボタンを押したときに呼び出されるメソッドです。要求画面の、現在のフォーカス位置を含む以降の入力内容を消去します。
        /// </summary>
        void ClearFocusedAndLater();
    }
}
