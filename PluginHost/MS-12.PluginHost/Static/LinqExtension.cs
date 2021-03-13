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
    public static class LinqExtension
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> selector)
        {
            foreach (T e in source)
            {
                selector(e);
            }
        }

        public static void ForEach<T>(this List<T> source, Action<T, int> selector)
        {
            for (int i = 0; i < source.Count; i++)
            {
                selector(source[i], i);
            }
        }
    }
}
