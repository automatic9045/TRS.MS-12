using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
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

namespace TRS.TMS12.Resources
{
    public class ElementBehavior
    {
        #region ForceOverride

        ///<summary>
        /// DependencyProperty
        ///</summary>
        public static readonly DependencyProperty ForceOverrideProperty = DependencyProperty.RegisterAttached("ForceOverride", typeof(bool), typeof(ElementBehavior), new PropertyMetadata(false, (s, e) =>
        {
            if (s is TextBoxBase t)
            {
                if ((bool)e.NewValue)
                {
                    t.PreviewKeyDown += OnForceOverride;
                }
                else
                {
                    t.PreviewKeyDown -= OnForceOverride;
                }
            }
        }));

        ///<summary>
        /// Get
        ///</summary>
        ///<param name="target"><see cref="DependencyObject"/></param>
        ///<returns><see cref="bool"/></returns>
        public static bool GetForceOverride(DependencyObject target)
        {
            return (bool)target.GetValue(ForceOverrideProperty);
        }

        ///<summary>
        /// Set
        ///</summary>
        ///<param name="target"><see cref="DependencyObject"/></param>
        ///<param name="value"><see cref="bool"/></param>
        public static void SetForceOverride(DependencyObject target, bool value)
        {
            target.SetValue(ForceOverrideProperty, value);
        }

        private static void OnForceOverride(object sender, KeyEventArgs e)
        {
            Key[] BAD_KEYS = new Key[] { Key.Back, Key.Delete };
            Key[] WRK_KEYS = new Key[] { Key.Left, Key.Up, Key.Right, Key.Down, Key.Enter };
            if (BAD_KEYS.Contains(e.Key))
            {
                //e.Handled = true;
            }
            else if (!WRK_KEYS.Contains(e.Key))
            {
                if (sender is RichTextBox r)
                {
                    if (!string.IsNullOrEmpty(new TextRange(r.Document.ContentStart, r.Document.ContentEnd).Text))
                    {
                        r.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, (Action)delegate
                        {
                            TextPointer tp = r.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                            if (tp != null && tp != r.Document.ContentEnd)
                            {
                                r.Selection.Select(r.CaretPosition, tp);
                            }
                        });
                    }
                }
                else if (sender is TextBox t)
                {
                    if (!string.IsNullOrEmpty(t.Text))
                    {
                        t.Select(t.CaretIndex, 1);
                    }
                }
            }
        }
        #endregion
    }
}
