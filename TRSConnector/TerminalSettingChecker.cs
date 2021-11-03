using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace TRS.TMS12.Plugins.TRS
{
    internal static class TerminalSettingChecker
    {
        private static List<string> invalidStationNames = new List<string>()
        {
            "テスト", "名無", "試", "開発", "環境", "＊"
        };

        private static bool IsAlphabet(char c)
        {
            return c >= 'A' && c <= 'z';
        }

        public static bool IsValidStationName(this string stationName)
        {
            if (stationName == "") return false;
            if (stationName.Length > 6) return false;
            if (stationName == "駅") return false;
            if (stationName.Any(c => char.IsDigit(c) || IsAlphabet(c) || char.IsWhiteSpace(c))) return false;

            return !invalidStationNames.Any(invalidName => stationName.Contains(invalidName));
        }
    }
}
