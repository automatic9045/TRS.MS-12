using System;
using System.ComponentModel;
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

using Prism.Mvvm;
using Prism.Commands;

using Gayak.Collections;

namespace TRS.TMS12
{
    public enum SendingType
    {
        /// <summary>
        /// 発売
        /// </summary>
        Sell,
        /// <summary>
        /// 予約
        /// </summary>
        Reserve,
        /// <summary>
        /// 照会
        /// </summary>
        Inquire,
    }

    public enum FunctionKeys
    {
        /// <summary>
        /// ﾒﾆｭｰ
        /// </summary>
        Menu = 0,

        /// <summary>
        /// 営試
        /// </summary>
        F1,
        /// <summary>
        /// 回復
        /// </summary>
        F2,
        /// <summary>
        /// 切替
        /// </summary>
        F3,
        /// <summary>
        /// 保存
        /// </summary>
        F4,
        /// <summary>
        /// 開始
        /// </summary>
        F5,
        /// <summary>
        /// 終了
        /// </summary>
        F6,
        /// <summary>
        /// 中断
        /// </summary>
        F7,
        /// <summary>
        /// 再開１
        /// </summary>
        F8,
        /// <summary>
        /// 再開２
        /// </summary>
        F9,
        /// <summary>
        /// 一括開始
        /// </summary>
        F10,
        /// <summary>
        /// 一括発券
        /// </summary>
        F11,
        /// <summary>
        /// 応答
        /// </summary>
        F12,
        /// <summary>
        /// 残消去
        /// </summary>
        F13,
        /// <summary>
        /// ＩＣ
        /// </summary>
        F14,
        /// <summary>
        /// 連加算
        /// </summary>
        F15,

        /// <summary>
        /// 発売
        /// </summary>
        Sell,
        /// <summary>
        /// 予約
        /// </summary>
        Reserve,
        /// <summary>
        /// 照会
        /// </summary>
        Inquire,
        /// <summary>
        /// 中継
        /// </summary>
        Relay,

        /// <summary>
        /// 保持
        /// </summary>
        Hold,
        /// <summary>
        /// 解放
        /// </summary>
        Release,
        /// <summary>
        /// 発信
        /// </summary>
        Send,
    }
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel VM { get; private set; }
        private Dictionary<UserControls, UserControl> userControls;

        public MainWindow(MainWindowViewModel vm)
        {
            InitializeComponent();
            VM = vm;
            DataContext = VM;

            Dialog.DataContext = VM.DialogViewModel;
            ResultControl.DataContext = VM.ResultControlViewModel;
            FullScreenResultControl.DataContext = VM.FullScreenResultControlViewModel;

            userControls = new Dictionary<UserControls, UserControl>()
            {
                {UserControls.PowerOff, PowerOff },
                {UserControls.MainMenu, MainMenu },
                {UserControls.GroupMenu, GroupMenu },
                {UserControls.OneTouchMenu, OneTouchMenu },
            };
            foreach (KeyValuePair<UserControls, UserControl> p in userControls)
            {
                p.Value.DataContext = VM.ViewModels[p.Key];
            }

            VM.TicketInputControls.ForEach(c => TicketInputControlsGrid.Children.Add(c));
            VM.TicketKeyControls.ForEach(c => TicketKeyControlsGrid.Children.Add(c));
        }
    }
}
