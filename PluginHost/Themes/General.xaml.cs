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
    public partial class General
    {
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).Select(0, 1);
        }

        private void TextBox_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            ((TextBox)sender).Clear();
        }

        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (textBox.IsFocused) return;
            textBox.Focus();
            e.Handled = true;
        }


        private Button pressedButton = null;

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pressedButton != null)
            {
                pressedButton.SetValue(Button.BackgroundProperty, Brushes.White);
            }

            pressedButton = sender as Button;
            if (pressedButton != null)
            {
                pressedButton.SetValue(Button.BackgroundProperty, Brushes.Aqua);
            }
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (pressedButton != null)
            {
                if (VisualTreeHelper.HitTest(pressedButton, e.GetPosition(pressedButton)) == null)
                {
                    pressedButton.SetValue(Button.BackgroundProperty, Brushes.White);
                    pressedButton = null;
                }
            }
        }

        private void RadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (pressedButton != null)
            {
                pressedButton.SetValue(Button.BackgroundProperty, Brushes.White);
            }

            pressedButton = null;
        }
    }
}
