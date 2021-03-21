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
    }

    public abstract class TicketBase
    {
        public Bitmap Bitmap { get; }

        public abstract TicketBase Resend();

        public TicketBase(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }

    public class TicketInfo
    {
        public TicketBase Ticket { get; }
        public bool HasPrinted { get; private set; } = false;

        public TicketInfo(TicketBase ticket)
        {
            Ticket = ticket;
        }

        public void OnPrint()
        {
            HasPrinted = true;
        }
    }

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

    /// <summary>
    /// 発信時の結果を表すクラスです。
    /// </summary>
    public class SendResult
    {
        public bool IsFullScreen { get; protected set; } = false;
        public SendResultType? Result { get; protected set; } = null;
        public string RethinkCode { get; protected set; } = "";
        public string Message { get; protected set; } = "";
        public string Text { get; protected set; } = "";
        public string JsonString { get; protected set; } = "";
        public Exception Exception { get; protected set; } = null;

        /// <summary>
        /// ＹＥＳの <see cref="SendResult"/> を作成します。
        /// </summary>
        /// <param name="text">結果の詳細。</param>
        /// <param name="message">メッセージ。</param>
        public static SendResult Yes(string text, string message, bool isFullScreen) => new SendResult()
        {
            IsFullScreen = isFullScreen,
            Result = SendResultType.Yes,
            Text = text,
            Message = message,
        };

        /// <summary>
        /// ＮＯの <see cref="SendResult"/> を作成します。
        /// </summary>
        /// <param name="text">結果の詳細。</param>
        /// <param name="message">メッセージ。</param>
        public static SendResult No(string text, string message = "") => new SendResult()
        {
            Result = SendResultType.No,
            Text = text,
            Message = message,
        };

        /// <summary>
        /// 再考の <see cref="SendResult"/> を作成します。
        /// </summary>
        /// <param name="text">結果の詳細。</param>
        /// <param name="message">メッセージ。</param>
        /// <param name="rethinkCode">再考コード。</param>
        public static SendResult Rethink(string message = "", string rethinkCode = "??0000") => new SendResult()
        {
            Result = SendResultType.Rethink,
            Message = message,
            RethinkCode = rethinkCode,
        };


        public static SendResult Error(Exception exception, string json = "", SendResultType? type = null) => new SendResult()
        {
            Result = type,
            Exception = exception,
            JsonString = json,
        };
    }

    public class IssuableSendResult : SendResult
    {
        private bool isTicketCreated = false;
        public Func<List<TicketBase>> _CreateTicketsMethod;
        public Func<List<TicketBase>> CreateTicketsFunc
        {
            get
            {
                if (!isTicketCreated)
                {
                    List<TicketBase> tickets = _CreateTicketsMethod.Invoke();
                    CreateTicketsFunc = () => tickets;
                    isTicketCreated = true;
                }

                return _CreateTicketsMethod;
            }
            protected set => _CreateTicketsMethod = value;
        }

        public static IssuableSendResult Yes(Func<List<TicketBase>> createTicketsFunc, string text, string message, bool isFullScreen)
        {
            IssuableSendResult result = new IssuableSendResult()
            {
                IsFullScreen = isFullScreen,
                Result = SendResultType.Yes,
                Text = text,
                Message = message,
                CreateTicketsFunc = createTicketsFunc,
            };
            return result;
        }
    }

    public interface ISender
    {
        /// <summary>
        /// 発信時の処理を定義します。
        /// </summary>
        /// <returns>発信結果を表す <see cref="SendResult"/>。</returns>
        SendResult Send();
    }
}
