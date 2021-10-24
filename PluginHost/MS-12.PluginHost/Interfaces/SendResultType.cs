using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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
    /// 回答の種別を指定します。
    /// </summary>
    public enum SendResultType
    {
        /// <summary>
        /// ＹＥＳ。乗車券の発行や指定席の予約に成功した場合など、要求が正当であり、処理が成功したことを示します。
        /// </summary>
        Yes,
        /// <summary>
        /// ＮＯ。指定席の予約に満席の為失敗した場合など、要求は正当であったものの、処理に失敗したことを示します。
        /// </summary>
        No,
        /// <summary>
        /// 再考。不正な経由地や存在しない号数を指定した場合など、要求が不正であり、処理が実行できなかったことを示します。
        /// </summary>
        Rethink,
        /// <summary>
        /// 再送。システムがビジー状態であり、要求が棄却されたことを示します。
        /// </summary>
        Resend,
    }
}
