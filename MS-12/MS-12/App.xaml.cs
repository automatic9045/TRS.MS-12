using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Prism.Commands;

using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public enum ErrorType
    {
        /// <summary>
        /// ユーザーに提供される情報。
        /// 続行可能。
        /// </summary>
        Information,
        /// <summary>
        /// レイアウトが崩れたり、実行時に例外・エラーが発生する恐れがあることを示す警告。
        /// 続行可能。
        /// </summary>
        Warning,
        /// <summary>
        /// 正しく実行することが出来ないことを示すエラー。
        /// 続行不可能。
        /// </summary>
        Error,
    }

    public class AppConnector
    {
        private SplashScreenViewModel splashVM;
        private static Dictionary<ErrorType, string> errorTypeStrings = new Dictionary<ErrorType, string>()
        {
            { ErrorType.Information, "情報" },
            { ErrorType.Warning, "警告" },
            { ErrorType.Error, "ｴﾗｰ " },
        };

        public bool ShowNotImplementedDialog { get; set; }

        public string PrinterClass { get; }
        public string PrinterName { get; }

        public string MainMenuLayoutSourcePath { get; }
        public string OneTouchMenuLayoutSourcePath { get; }

        public AppConnector(SplashScreenViewModel splashScreenVM, string printerClass, string printerName, string mainMenuLayoutSourcePath, string oneTouchMenuLayoutSourcePath)
        {
            splashVM = splashScreenVM;

            PrinterClass = printerClass;
            PrinterName = printerName;

            MainMenuLayoutSourcePath = mainMenuLayoutSourcePath;
            OneTouchMenuLayoutSourcePath = oneTouchMenuLayoutSourcePath;
        }

        public void OnError(string text, string caption, ErrorType type)
        {
            splashVM.Errors.Add("(" + errorTypeStrings[type] + ") " + caption + "：" + text);
        }

        public void ChangeProgressStatus(string message, Progress progress)
        {
            splashVM.ProgressMessage = message;
            splashVM.ProgressValue = (int)progress;
            DoEvents();
        }

        public void ChangeProgressStatus(string message) => ChangeProgressStatus(message, (Progress)splashVM.ProgressValue);
    }

    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private MainWindowViewModel MainWindowVM { get; set; } = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SplashScreen splash = null;
            MainWindow mainWindow = null;

            var splashVM = new SplashScreenViewModel()
            {
                Ignore = new DelegateCommand(() => Start()),
            };
            splash = new SplashScreen(splashVM);
            this.MainWindow = splash;
            splash.Show();

            AppConnector appConnector = null;
            bool canStart = true;

            try
            {
                XElement appSettings = XDocument.Load(@"Settings\App.xml").Element("ApplicationSettings");

                if ((bool)appSettings.Element("CatchUnobservedExceptions").Attribute("Enabled"))
                    DispatcherUnhandledException += AppDispatcherUnhandledException;

                XElement menuLayoutSources = XDocument.Load(@"Settings\MenuLayoutSources.xml").Element("MenuLayoutSources");

                appConnector = new AppConnector(
                    splashVM,
                    (string)appSettings.Element("Printer").Attribute("Class"), (string)appSettings.Element("Printer").Attribute("Name"),
                    Path.GetFullPath(Path.Combine(AppDirectory, "Settings", (string)menuLayoutSources.Element("MainMenu").Attribute("Source") ?? "")),
                    Path.GetFullPath(Path.Combine(AppDirectory, "Settings", (string)menuLayoutSources.Element("OneTouchMenu").Attribute("Source") ?? "")));
            }
            catch
            {
                splashVM.Errors.Add("基本設定を正常に読み込めませんでした。設定内容に誤りが無いか確認し、解消されない場合は再インストールを検討して下さい。");
                splashVM.IsErrorIgnorable = false;
                splashVM.IsErrorShown = true;
                canStart = false;
            }

            if (canStart)
            {
                MainWindowVM = new MainWindowViewModel(appConnector);
                mainWindow = new MainWindow(MainWindowVM);
                MainWindowVM.M.Loaded();

                if (splashVM.Errors.Count > 0)
                {
                    splashVM.IsErrorIgnorable = !splashVM.Errors.Any(err => err.StartsWith("(ｴﾗｰ ) "));
                    splashVM.IsErrorShown = true;
                }
                else
                {
                    Start();
                }
            }


            async void Start()
            {
                MainWindowVM.M.DialogModel.ShowInformationDialog("ソフトウェアのバージョンを確認しています...", false);

                this.MainWindow = mainWindow;
                mainWindow.Show();
                splash.Close();

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string latestVersion = await client.GetStringAsync("https://script.google.com/macros/s/AKfycbxXOLf46ZjE9WDUC4xQYn2itPOrKA5Qr3_uPWqcpWU9AFJr7QGgkF-KQQAYlu0kRvGX/exec");
                        if (latestVersion == ProductVersion)
                        {
                            MainWindowVM.M.DialogModel.ShowInformationDialog($"ご利用のバージョンは最新です。\n\nご利用のバージョン：　Version {ProductVersion}\n最新のバージョン　：　Version {latestVersion}");
                        }
                        else
                        {
                            MainWindowVM.M.DialogModel.ShowInformationDialog($"新しいバージョンがリリースされています。\n\nご利用のバージョン：　Version {ProductVersion}\n最新のバージョン　：　Version {latestVersion}");
                        }
                    }
                }
                catch
                {
                    MainWindowVM.M.DialogModel.ShowInformationDialog($"ご利用のバージョンが最新であるか確認出来ませんでした。\n\nご利用のバージョン：　Version {ProductVersion}\n最新のバージョン　：　不明");
                }
            }
        }

        private void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string exceptionString = e.Exception.ToString();
            string text = "アプリケーションでハンドルされていない例外が発生しました。\n\n例外のログが Log フォルダ内に保存されます。\n\n\n詳細：\n\n" + exceptionString;

            if (MainWindowVM is null)
                MessageBox.Show(text);
            else
                MainWindowVM.M.DialogModel.ShowErrorDialog(text);

            e.Handled = true;
        }
    }
}
