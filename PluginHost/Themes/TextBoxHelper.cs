using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Threading;

namespace TRS.TMS12.Resources
{
    public class TextBoxHelper
    {
        public static int GetSelectionStart(DependencyObject obj)
        {
            return (int)obj.GetValue(SelectionStartProperty);
        }

        public static void SetSelectionStart(DependencyObject obj, int value)
        {
            obj.SetValue(SelectionStartProperty, value);
        }

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.RegisterAttached(
            "SelectionStart", typeof(int), typeof(TextBoxHelper), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextChanged));

        private static void SelectedTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = obj as TextBox;
            if (textBox != null)
            {
                if (e.OldValue == null && e.NewValue != null)
                {
                    textBox.SelectionChanged += TextBoxSelectionChanged;
                }
                else if (e.OldValue != null && e.NewValue == null)
                {
                    textBox.SelectionChanged -= TextBoxSelectionChanged;
                }

                int? newValue = e.NewValue as int?;
                if (newValue != null && newValue != textBox.SelectionStart)
                {
                    textBox.SelectionStart = (int)newValue;
                }
            }
        }

        private static void TextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                SetSelectionStart(textBox, textBox.SelectionStart);
            }
        }
    }
}
