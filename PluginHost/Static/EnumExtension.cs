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
}
