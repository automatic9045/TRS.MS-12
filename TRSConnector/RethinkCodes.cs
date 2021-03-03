using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Codeplex.Data;

using TRS.TMS12.Interfaces;
using TRS.TMS12.Static;
using TRS.Tickets;

namespace TRS.TMS12.Plugins.TRS.RethinkCodes
{
    public class CodeInfo
    {
        public static Dictionary<Type, string> Names = new Dictionary<Type, string>()
        {
            { typeof(CannotSend), "ﾊﾂ" },
        };
    }

    /// <summary>
    /// TRS への発信が不可能であることを表す再考コードです。
    /// コードグループ名：ﾊﾂ
    /// </summary>
    public enum CannotSend
    {
        [EnumDisplayName("種別名誤り")]
        InvalidServiceName = 1,


    }
}
