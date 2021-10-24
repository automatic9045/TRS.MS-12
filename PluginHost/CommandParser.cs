using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;

using TRS.TMS12.Static;
using TRS.TMS12.Interfaces;
using TRS.TMS12.Resources;

namespace TRS.TMS12.Resources
{
    public class CommandParser
    {
        private ITicketPluginExtension m;
        private Type textBoxesEnum;

        private int month;
        private int day;

        public CommandParser(ITicketPluginExtension m, Type textBoxesEnum)
        {
            this.m = m;
            this.textBoxesEnum = textBoxesEnum;

            try
            {
                month = (int)GetTargetKey("Month");
                day = (int)GetTargetKey("Day");
            }
            catch { }
        }

        public void Parse(string param)
        {
            string[] parameters = param.Split('/');
            if (parameters[0] == "") return;

            for (int i = 0; i < parameters.Length; i++)
            {
                string command = parameters[i];

                string[] splittedParam = command.Split(';');
                string[] args = splittedParam[1].Split(',');
                int? key;
                int? focusTargetKey;
                switch (splittedParam[0])
                {
                    case "FOCUS":
                        focusTargetKey = GetTargetKey(args[0]);
                        if (focusTargetKey != null) m.CurrentTextBox = (int)focusTargetKey;
                        break;

                    case "SET":
                        key = GetTargetKey(args[0]);
                        if (!(key is null))
                        {
                            m.TextBoxes[(int)key] = args[1];

                            if (i == parameters.Length - 1)
                            {
                                focusTargetKey = Enum.IsDefined(textBoxesEnum, key + 1) ? key + 1 : key;
                                m.CurrentTextBox = (int)focusTargetKey;
                            }
                        }
                        break;

                    case "CLEAR":
                        key = GetTargetKey(args[0]);
                        if (!(key is null))
                        {
                            m.TextBoxes[(int)key] = "";

                            if (i == parameters.Length - 1)
                            {
                                m.CurrentTextBox = (int)key;
                            }
                        }
                        break;

                    case "ADD":
                        if (Keyboard.FocusedElement is TextBox)
                        {
                            TextBox textBox = (TextBox)Keyboard.FocusedElement;

                            int selectionStart = textBox.SelectionStart;

                            int selectionLength = 1;
                            if (textBox.SelectionLength != 0) selectionLength = textBox.SelectionLength;
                            else if (selectionStart == textBox.Text.Length) selectionLength = 0;

                            textBox.Text.Remove(selectionStart, selectionLength);
                            textBox.Text = textBox.Text.Insert(selectionStart, args[0]);
                            textBox.CaretIndex = selectionStart + args[0].Length;
                        }

                        /*
                            for (int n = 0; n < args[0].Length; n++) Keyboard.FocusedElement.RaiseEvent(
                                new KeyEventArgs(
                                    Keyboard.PrimaryDevice,
                                    PresentationSource.FromVisual((Visual)Keyboard.FocusedElement),
                                    0,
                                    Key.Delete)
                                { RoutedEvent = Keyboard.KeyDownEvent }
                            );

                            Keyboard.FocusedElement.RaiseEvent(
                                new TextCompositionEventArgs(
                                    InputManager.Current.PrimaryKeyboardDevice,
                                    new TextComposition(InputManager.Current, Keyboard.FocusedElement, args[0]))
                                { RoutedEvent = TextCompositionManager.TextInputEvent }
                            );
                        */
                        break;

                    case "SET_DATE":
                        int months = int.Parse("0" + args[0]);
                        int days = int.Parse("0" + args[1]);
                        DateTime date = DateTime.Today.AddMonths(months).AddDays(days);
                        m.TextBoxes[month] = Strings.StrConv(date.Month.ToString(), VbStrConv.Wide);
                        m.TextBoxes[day] = Strings.StrConv(date.Day.ToString(), VbStrConv.Wide);

                        if (i == parameters.Length - 1)
                        {
                            focusTargetKey = Enum.IsDefined(textBoxesEnum, day + 1) ? day + 1 : day;
                            m.CurrentTextBox = (int)focusTargetKey;
                        }
                        break;

                    case "ADD_OPTION":
                        m.AddOption(args[0]);
                        break;

                    case "SET_DISCOUNT":
                        m.SetDiscount(args[0]);
                        break;

                    case "OPEN":
                        m.PluginHost.Dialog.ShowNotImplementedDialog("OPEN コマンド");
                        break;

                    default:
                        m.PluginHost.Dialog.ShowWarningDialog("指定されたコマンドが見つかりませんでした：\n\n" + param);
                        break;
                }
            }
        }

        private int GetTargetKey(string arg)
        {
            int currentKey;

            if (arg[0] == '~') currentKey = m.CurrentTextBox + int.Parse("0" + arg.TrimStart('~'));
            else if (arg[0] == '#') currentKey = int.Parse("0" + arg.TrimStart('#'));
            else currentKey = (int)Enum.Parse(textBoxesEnum, arg);

            return currentKey;
        }
    }
}
