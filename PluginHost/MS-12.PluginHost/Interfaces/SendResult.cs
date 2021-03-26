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
    /// 発信時の結果を表すクラスです。
    /// </summary>
    public class SendResult
    {
        public bool IsFullScreen { get; set; } = false;
        public SendResultType? Result { get; set; } = null;
        public string RethinkCode { get; set; } = "";
        public string Message { get; set; } = "";
        public string Text { get; set; } = "";
        public string JsonString { get; set; } = "";
        public Exception Exception { get; set; } = null;

        protected SendResult() { }

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
        public virtual List<TicketBase> Tickets { get; protected set; }

        public static IssuableSendResult Yes(List<TicketBase> tickets, string text, string message, bool isFullScreen)
        {
            IssuableSendResult result = new IssuableSendResult()
            {
                IsFullScreen = isFullScreen,
                Result = SendResultType.Yes,
                Text = text,
                Message = message,
                Tickets = tickets,
            };
            return result;
        }
    }

    public class IssueReservableSendResult : SendResult
    {
        private Func<int, int, List<TicketBase>> createTicketsFunc;

        public static IssueReservableSendResult Yes(Func<int, int, List<TicketBase>> createTicketsFunc, string text, string message, bool isFullScreen)
        {
            IssueReservableSendResult result = new IssueReservableSendResult()
            {
                IsFullScreen = isFullScreen,
                Result = SendResultType.Yes,
                Text = text,
                Message = message,
                createTicketsFunc = createTicketsFunc,
            };
            return result;
        }

        public List<TicketBase> CreateTickets(int issueNumber, int countStartNumber) => createTicketsFunc(issueNumber, countStartNumber);
    }
}
