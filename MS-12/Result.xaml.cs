using System;
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

using IOPath = System.IO;

using Prism.Mvvm;
using Prism.Commands;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class ResultControlModel : BindableBase
    {
        public DialogModel DialogModel { get; set; }

        private SendResult _SendResult = null;
        internal SendResult SendResult
        {
            get { return _SendResult; }
            set { SetProperty(ref _SendResult, value); }
        }

        public bool IsVisible
        {
            get => SendResult != null;
        }

        public List<bool> FIsEnabled { get; set; } = new List<bool>()
        {
            false,
            false, true, false, true, true, true, true, true, true, true, true, true, false, false, false,
            false, false, false, false,
            true, true, false,
        };

        public void Show(SendResult result)
        {
            SendResult = result;
        }

        public void Hide()
        {
            SendResult = null;
        }
    }

    public class ResultControlViewModel : BindableBase
    {
        public ResultControlViewModel(ResultControlModel model)
        {
            m = model;
            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(m.SendResult):
                        if (m.SendResult is null)
                        {
                            IsVisible = false;
                        }
                        else
                        {
                            Result = m.SendResult.Result switch
                            {
                                SendResultType.Yes => "ＹＥＳ",
                                SendResultType.No => "ＮＯ",
                                SendResultType.Rethink => "再考",
                                _ => "",
                            };
                            ErrorCode = m.SendResult.Code;
                            Caption = m.SendResult.Message;
                            Text = m.SendResult.Text;

                            if (!(m.SendResult.Exception is null))
                            {
                                m.DialogModel.ShowErrorDialog("ＴＲＳとの通信でエラーが発生しました。\n\n\n回答：\n\n" + m.SendResult.JsonString + "\n\n\n詳細：\n\n" + m.SendResult.Exception.ToString());
                            }

                            IsVisible = true;
                        }
                        DoEvents();
                        break;
                }
            });
        }

        private ResultControlModel m;

        private bool _IsVisible = false;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { SetProperty(ref _IsVisible, value); }
        }

        private string _Result = "";
        public string Result
        {
            get { return _Result; }
            set { SetProperty(ref _Result, value); }
        }

        private string _Caption = "";
        public string Caption
        {
            get { return _Caption; }
            set { SetProperty(ref _Caption, value); }
        }

        private string _ErrorCode = "";
        public string ErrorCode
        {
            get { return _ErrorCode; }
            set { SetProperty(ref _ErrorCode, value); }
        }

        private string _Text = "";
        public string Text
        {
            get { return _Text; }
            set { SetProperty(ref _Text, value); }
        }
    }

    public partial class ResultControl
    {
        public ResultControlViewModel vm;

        public ResultControl()
        {
            InitializeComponent();
        }
    }
}
