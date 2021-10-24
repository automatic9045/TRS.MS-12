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
    /// 発信結果の種別を指定します。
    /// </summary>
    public enum SendResultType
    {
        /// <summary>
        /// ＹＥＳ
        /// </summary>
        Yes,
        /// <summary>
        /// ＮＯ
        /// </summary>
        No,
        /// <summary>
        /// 再考
        /// </summary>
        Rethink,
        /// <summary>
        /// 再送
        /// </summary>
        Resend,
    }
}
