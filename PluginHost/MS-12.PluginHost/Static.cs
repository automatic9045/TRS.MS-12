using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace TRS.TMS12.Static
{
    public enum FunctionKeys
    {
        /// <summary>
        /// ﾒﾆｭｰ
        /// </summary>
        Menu = 0,

        /// <summary>
        /// 営試
        /// </summary>
        F1,
        /// <summary>
        /// 回復
        /// </summary>
        F2,
        /// <summary>
        /// 切替
        /// </summary>
        F3,
        /// <summary>
        /// 保存
        /// </summary>
        F4,
        /// <summary>
        /// 開始
        /// </summary>
        F5,
        /// <summary>
        /// 終了
        /// </summary>
        F6,
        /// <summary>
        /// 中断
        /// </summary>
        F7,
        /// <summary>
        /// 再開１
        /// </summary>
        F8,
        /// <summary>
        /// 再開２
        /// </summary>
        F9,
        /// <summary>
        /// 一括開始
        /// </summary>
        F10,
        /// <summary>
        /// 一括発券
        /// </summary>
        F11,
        /// <summary>
        /// 応答
        /// </summary>
        F12,
        /// <summary>
        /// 残消去
        /// </summary>
        F13,
        /// <summary>
        /// ＩＣ
        /// </summary>
        F14,
        /// <summary>
        /// 連加算
        /// </summary>
        F15,

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
        /// <summary>
        /// 中継
        /// </summary>
        Relay,

        /// <summary>
        /// 保持
        /// </summary>
        Hold,
        /// <summary>
        /// 解放
        /// </summary>
        Release,
        /// <summary>
        /// 発信
        /// </summary>
        Send,
    }

    public class EnumDisplayNameAttribute : Attribute
    {
        public string Name { get; set; }
        public EnumDisplayNameAttribute(string name)
        {
            Name = name;
        }
    }

    public static class EnumExtension
    {
        public static string GetEnumDisplayName<T>(this T enumValue)
        {
            var field = typeof(T).GetField(enumValue.ToString());
            return ((EnumDisplayNameAttribute)Attribute.GetCustomAttribute(field, typeof(EnumDisplayNameAttribute))).Name;
        }
    }

    public static class App
    {
        private static FileVersionInfo FileVersionInfo { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        public static string ProductVersion { get; } = FileVersionInfo.ProductVersion;
        public static string Copyright { get; } = FileVersionInfo.LegalCopyright;
        public static string TicketPluginsNamespace { get; } = "TRS.TMS12.TicketPlugins.";
        public static string PrinterPluginsNamespace { get; } = "TRS.TMS12.PrinterPlugins.";
        public static string AppDirectory { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }

        public static int WideStringToInt(string wideString)
        {
            int number = 0;
            try
            {
                number = int.Parse("0" + Strings.StrConv(wideString, VbStrConv.Narrow));
            }
            catch { }
            return number;
        }
    }
}
