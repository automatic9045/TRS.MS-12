using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace TRS.TMS12.Plugins.TRS
{
    internal static class InputBox
    {
        public static string Show(string text, string caption, string defaultResponse)
        {
            return Interaction.InputBox(text, caption, defaultResponse);
        }

        public static string Show(string text, string caption) => Show(text, caption, "");
        public static string Show(string text) => Show(text, "");
    }
}
