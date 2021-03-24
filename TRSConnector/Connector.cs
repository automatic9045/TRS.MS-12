using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

using Codeplex.Data;

using TRS.TMS12.Interfaces;
using TRS.Tickets;
using TRS.TMS12.Static;

namespace TRS.TMS12.Plugins.TRS
{
    public class CustomersInfo
    {
        public int Adult { get; private set; }
        public int Student { get; private set; }
        public int Child { get; private set; }
        public int Preschooler { get; private set; }

        public int Total
        {
            get => Adult + Student + Child + Preschooler;
        }

        public CustomersInfo(int adult, int student, int child, int preschooler)
        {
            Adult = adult;
            Student = student;
            Child = child;
            Preschooler = preschooler;
        }
    }

    public enum PayType
    {
        Cash,
        IC,
        Credit,
    }

    public class Pay
    {
        public PayType PayType { get; private set; }
        public string ICIDm { get; private set; } = null;

        public static Pay Cash() => new Pay()
        {
            PayType = PayType.Cash,
        };

        public static Pay IC(string idm) => new Pay()
        {
            PayType = PayType.IC,
            ICIDm = idm,
        };
    }

    public enum Game
    {
        [EnumDisplayName("トレインシミュレーター")]
        TrainSimulator = 1,

        [EnumDisplayName("模型体験運転")]
        ModelTrainDriving = 2,
    }

    public enum Discount
    {
        None = -1,

        [EnumDisplayName("割引種別：社用")]
        CompanyUse,

        [EnumDisplayName("割引種別：交‐社")]
        Member,

        [EnumDisplayName("割引種別：学内")]
        School,
    }

    public enum Option
    {
        None = -1,

        [EnumDisplayName("時間変更")]
        Changed,

        [EnumDisplayName("案内省略")]
        OmitGuidePrinting,

        [EnumDisplayName("操作補助不要")]
        NoHelp,
    }

    public partial class Connector : IPlugin
    {
        /// <summary>
        /// 会社ごとに割り振られる数字を取得します。
        /// </summary>
        public int CompanyNumber { get; private set; } = 0;

        /// <summary>
        /// 駅名を取得します。
        /// </summary>
        public string StationName { get; private set; } = "名無駅";

        /// <summary>
        /// 端末名を取得します。
        /// </summary>
        /// <example>@1、-1、MS1 など。</example>
        public string TerminalName { get; private set; } = "＠１";

        public PrintSetting PrintSetting { get; private set; }

        /// <summary>
        /// 現在の発信モード（発売／予約／照会）を取得・設定します。
        /// </summary>
        public SendTypes? SendType { get; private set; } = null;

        /// <summary>
        /// 営業試験モードであるかを取得・設定します。
        /// </summary>
        public bool IsTestMode { get; private set; } = false;

        /// <summary>
        /// 中継モードであるかを取得・設定します。
        /// </summary>
        public bool IsRelayMode { get; private set; } = false;

        /// <summary>
        /// 一件モードであるかを取得・設定します。
        /// </summary>
        public bool IsOneTimeMode { get; private set; }

        public void Initialize(int companyNumber, string stationName, string terminalName, string printSetting)
        {
            CompanyNumber = companyNumber;
            StationName = Strings.StrConv(stationName, VbStrConv.Wide);
            TerminalName = Strings.StrConv(terminalName, VbStrConv.Wide);
            PrintSetting = printSetting switch
            {
                "StarTSP100I" => PrintSetting.PrintByStarTSP100,
                "StarTSP100II" => PrintSetting.PrintByStarTSP100II,
                "EpsonTML90" => PrintSetting.PrintByEpsonTML90,
                _ => PrintSetting.SaveTicketAsPicture,
            };
        }

        public void ModeChanged(Mode mode, object value)
        {
            switch (mode)
            {
                case Mode.Test:
                    IsTestMode = (bool)value;
                    break;

                case Mode.OneTime:
                    IsOneTimeMode = (bool)value;
                    break;

                case Mode.Relay:
                    IsRelayMode = (bool)value;
                    break;
            }
        }

        public void SendTypeChanged(SendTypes? sendType)
        {
            SendType = sendType;
        }

        private SendResult ParseResult(dynamic json, bool isFullScreen = false)
        {
            SendResultType resultType = ResultTypeStringToEnum(json);
            return resultType switch
            {
                SendResultType.Yes => SendResult.Yes((string)json.text.Replace("\n", "\n\n"), (string)json.message, isFullScreen),
                SendResultType.No => SendResult.No((string)json.text.Replace("\n", "\n\n"), (string)json.message),
                SendResultType.Rethink => SendResult.Rethink((string)json.message, (string)json.rethinkCode),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private SendResult ParseResult(dynamic json, Func<List<TicketBase>> createTicketsFunc, bool isFullScreen = false)
        {
            SendResultType resultType = ResultTypeStringToEnum(json);
            return resultType switch
            {
                SendResultType.Yes => IsOneTimeMode ?
                IssueReservableSendResult.Yes(createTicketsFunc, (string)json.text.Replace("\n", "\n\n"), (string)json.message, isFullScreen) :
                IssuableSendResult.Yes(createTicketsFunc(), (string)json.text.Replace("\n", "\n\n"), (string)json.message, isFullScreen),
                SendResultType.No => SendResult.No((string)json.text.Replace("\n", "\n\n"), (string)json.message),
                SendResultType.Rethink => SendResult.Rethink((string)json.message, (string)json.rethinkCode),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private static SendResultType ResultTypeStringToEnum(dynamic json) => ((string)json.result) switch
        {
            "yes" => SendResultType.Yes,
            "no" => SendResultType.No,
            "rethink" => SendResultType.Rethink,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
