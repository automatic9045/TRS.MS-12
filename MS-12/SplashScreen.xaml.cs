using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Prism.Commands;
using Prism.Mvvm;

using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public enum Progress
    {
        LoadingBasicClasses = 0,
        LoadingPlugins,
        LoadingPages,
        LoadingMainMenuAndGroupMenuLayout,
        LoadingOneTouchMenuLayout,
        PreparingToDisplay,
    }

    public class SplashScreenViewModel : BindableBase
    {
        public SplashScreenViewModel()
        {
            Copyright = "Copyright © 2020-" + DateTime.Now.Year + "  Automatic9045";
            Version = ProductVersion;
            ProgressValueMax = Enum.GetValues(typeof(Progress)).Length;
        }

        private string _copyright = "";
        public string Copyright
        {
            get { return _copyright; }
            set { SetProperty(ref _copyright, value); }
        }

        private string _version = "";
        public string Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }

        private string _ProgressMessage = "基礎クラスを読み込んでいます";
        public string ProgressMessage
        {
            get { return _ProgressMessage; }
            set { SetProperty(ref _ProgressMessage, value); }
        }

        private int _ProgressValue = (int)Progress.LoadingBasicClasses;
        public int ProgressValue
        {
            get { return _ProgressValue; }
            set { SetProperty(ref _ProgressValue, value); }
        }

        public int ProgressValueMax { get; }

        private bool _IsErrorIgnorable = true;
        public bool IsErrorIgnorable
        {
            get { return _IsErrorIgnorable; }
            set { SetProperty(ref _IsErrorIgnorable, value); }
        }

        private bool _IsErrorShown = false;
        public bool IsErrorShown
        {
            get { return _IsErrorShown; }
            set { SetProperty(ref _IsErrorShown, value); }
        }

        public ObservableCollection<string> Errors { get; private set; } = new ObservableCollection<string>();

        private DelegateCommand _Stop;
        public DelegateCommand Stop
        {
            get => _Stop;
            set => SetProperty(ref _Stop, value);
        }

        public DelegateCommand Ignore { get; set; }
    }

    /// <summary>
    /// SplashScreen.xaml の相互作用ロジック
    /// </summary>
    public partial class SplashScreen : Window
    {
        private SplashScreenViewModel vm;

        public SplashScreen(SplashScreenViewModel vm)
        {
            InitializeComponent();
            this.vm = vm;
            DataContext = vm;
        }
    }
}
