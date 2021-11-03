using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Prism.Mvvm;
using Prism.Commands;
using System.Configuration;

namespace TRS.TMS12
{
    /// <summary>
    /// MainMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class MainMenuButton : UserControl
    {
        public MainMenuButton()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(MainMenuButton),
            new FrameworkPropertyMetadata(nameof(GroupName), new PropertyChangedCallback((d, e) =>
            {
                MainMenuButton control = (MainMenuButton)d;
                if (control != null)
                {
                    control.GroupNameLabel.Content = control.GroupName;
                }
            })));

        public static readonly DependencyProperty TypicalTicketNameProperty = DependencyProperty.Register(nameof(TypicalTicketName), typeof(string), typeof(MainMenuButton),
            new FrameworkPropertyMetadata(nameof(TypicalTicketName), new PropertyChangedCallback((d, e) =>
            {
                MainMenuButton control = (MainMenuButton)d;
                if (control != null)
                {
                    control.TypicalTicketNameLabel.Content = control.TypicalTicketName;
                }
            })));
        /*
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(DelegateCommand), typeof(MainMenuButton),
            new FrameworkPropertyMetadata(nameof(Command), new PropertyChangedCallback((d, e) =>
            {
                MainMenuButton control = (MainMenuButton)d;
                if (control != null)
                {
                    control.MainButton.Command = control.Command;
                }
            })));

        public static readonly DependencyProperty OpenTabCommandProperty = DependencyProperty.Register(nameof(OpenTabCommand), typeof(DelegateCommand), typeof(MainMenuButton),
            new FrameworkPropertyMetadata(nameof(OpenTabCommand), new PropertyChangedCallback((d, e) =>
            {
                MainMenuButton control = (MainMenuButton)d;
                if (control != null)
                {
                    control.TabButton.Command = control.OpenTabCommand;
                }
            })));
        */

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public string TypicalTicketName
        {
            get { return (string)GetValue(TypicalTicketNameProperty); }
            set { SetValue(TypicalTicketNameProperty, value); }
        }
        /*
        public DelegateCommand Command
        {
            get { return (DelegateCommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public DelegateCommand OpenTabCommand
        {
            get { return (DelegateCommand)GetValue(OpenTabCommandProperty); }
            set { SetValue(OpenTabCommandProperty, value); }
        }
        */
        public ICommand Command
        {
            get { return MainButton.Command; }
            set { MainButton.Command = value; }
        }

        public ICommand OpenTabCommand
        {
            get { return TabButton.Command; }
            set { TabButton.Command = value; }
        }
    }
}
