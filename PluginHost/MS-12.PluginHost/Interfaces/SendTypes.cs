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
    /// 操作種別を指定します。発売／予約／照会。
    /// </summary>
    public enum SendTypes
    {
        /// <summary>
        /// 発売
        /// </summary>
        Sell,
        /// <summary>
        /// 予約
        /// </summary>
        Reserve,
        /// <summary>
        /// 照会
        /// </summary>
        Inquire,
    }
}
