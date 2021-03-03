using System;
using System.IO;
using System.ComponentModel;
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
using Prism.Commands;
using Prism.Mvvm;

namespace TRS.TMS12
{
    internal class DialogInfo
    {
        public bool ButtonIsEnabled { get; set; } = true;
        public string Text { get; set; } = "";
        public string Caption { get; set; } = "";
        public ImageSource Icon { get; set; } = null;
    }

    public class DialogModel : BindableBase
    {
        private DialogInfo _DialogInfo;
        internal DialogInfo DialogInfo {
            get { return _DialogInfo; }
            set { SetProperty(ref _DialogInfo, value); }
        }

        public void ShowDialog(string text, string caption, ImageSource icon, bool buttonIsEnabled = true)
        {
            DialogInfo = new DialogInfo()
            {
                ButtonIsEnabled = buttonIsEnabled,
                Text = text,
                Caption = caption,
                Icon = icon,
            };
        }

        public void ShowDialog(string text, string caption = "", bool buttonIsEnabled = true)
        {
            ShowDialog(text, caption, null, buttonIsEnabled);
        }

        public void ShowErrorDialog(string text, bool buttonIsEnabled = true)
        {
            ShowDialog(text, "障害メッセージ", buttonIsEnabled);

            Task.Run(() => File.WriteAllText(@"Logs\" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".txt", text));
        }

        public void ShowWarningDialog(string text, bool buttonIsEnabled = true)
        {
            ShowDialog(text, "警告", buttonIsEnabled);
        }

        public void ShowInformationDialog(string text, bool buttonIsEnabled = true)
        {
            ShowDialog(text, "情報", buttonIsEnabled);
        }

        public void ShowNotImplementedDialog(string text = "", bool isCollectingInformation = true)
        {
            string message = isCollectingInformation ? "\n情報提供のご協力お待ちしています。" : "";
            ShowDialog("この機能は未実装です。" + message + "\n\n\n" + text, "情報");
        }

        public void ShowNotImplementedDialog(bool isCollectingInformation)
        {
            ShowNotImplementedDialog("", isCollectingInformation);
        }

        public void HideDialog()
        {
            DialogInfo = null;
        }
    }

    public class DialogViewModel : BindableBase
    {
        public DialogViewModel(DialogModel model)
        {
            m = model;

            ButtonClicked = new DelegateCommand(() =>
            {
                IsVisible = false;
            });

            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                if (m.DialogInfo == null)
                {
                    IsVisible = false;
                }
                else
                {
                    ButtonIsEnabled = m.DialogInfo.ButtonIsEnabled;
                    Text = m.DialogInfo.Text;
                    Caption = m.DialogInfo.Caption;
                    Icon = m.DialogInfo.Icon;
                    IsVisible = true;
                }
            });
        }

        private DialogModel m;

        private bool _IsVisible = false;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { SetProperty(ref _IsVisible, value); }
        }

        private bool _ButtonIsEnabled = true;
        public bool ButtonIsEnabled
        {
            get { return _ButtonIsEnabled; }
            set { SetProperty(ref _ButtonIsEnabled, value); }
        }

        private string _Text = "";
        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }

        private string _Caption = "";
        public string Caption
        {
            get { return _Caption; }
            set { SetProperty(ref _Caption, value); }
        }

        private ImageSource _Icon = null;
        public ImageSource Icon
        {
            get { return _Icon; }
            set { SetProperty(ref _Icon, value); }
        }

        public DelegateCommand ButtonClicked { get; set; }
    }

    /// <summary>
    /// ErrorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class Dialog : UserControl
    {
        public Dialog()
        {
            InitializeComponent();
        }
    }
}
