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
    /// <see cref="IPluginHost.ModeEnabledChanged"/> で、有効・無効が変更されたモードを指定します。
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// 営業試験
        /// </summary>
        Test,

        /// <summary>
        /// 一件
        /// </summary>
        OneTime,

        /// <summary>
        /// クレジット
        /// </summary>
        Credit,

        /// <summary>
        /// 中継発売
        /// </summary>
        Relay,
    }
}
