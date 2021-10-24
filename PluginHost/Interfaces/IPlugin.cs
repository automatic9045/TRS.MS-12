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
    /// アプリケーションとの連携を可能にするインターフェイスです。
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// アプリケーションとプラグインの間でやりとりするためのメソッド、プロパティを提供する <see cref="IPluginHost"/> を取得します。
        /// </summary>
        IPluginHost PluginHost { get; set; }
    }
}
