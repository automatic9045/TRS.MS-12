using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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

using IOPath = System.IO.Path;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Gayak.Collections;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class TicketButton
    {
        public string TypeName { get; set; } = "（名前無し）";
        public ITicketPlugin TicketPlugin { get; set; }
        public string Command { get; set; } = "";
    }

    public class MainMenuContent
    {
        public string TicketGroupName { get; set; } = "";
        public List<TicketButton> TicketGroup { get; set; }
        public string TypicalTicketName { get; set; } = "";
        public ITicketPlugin TypicalTicket { get; set; }
        public DelegateCommand Command { get; set; }
        public DelegateCommand OpenTabCommand { get; set; }
    }

    public class MainMenuGroupBox
    {
        public string Header { get; set; } = "";
        public int Row { get; set; } = 0;
        public List<MainMenuContent> Contents { get; set; }
    }

    public partial class MainMenuModel : BindableBase, IModel
    {
        private string GetErrorTypeString(ErrorType errorType)
        {
            switch (errorType)
            {
                case ErrorType.Information: return "情報";
                case ErrorType.Warning: return "警告";
                case ErrorType.Error: return "エラー";
                default: throw new ArgumentException();
            }
        }

        private string GetMenuTypeString(Screen menuType)
        {
            switch (menuType)
            {
                case Screen.MainMenu: return "メインメニュー";
                case Screen.GroupMenu: return "券種メニュー";
                case Screen.MaintenanceMenu: return "メインテナンスメニュー";
                default: throw new ArgumentException();
            }
        }

        private void Throw(string text, ErrorType errorType, Screen menuType = Screen.MainMenu)
        {
            UserControlHost.Throw(text, GetMenuTypeString(menuType) + "設定に関する" + GetErrorTypeString(errorType), errorType);
        }


        public void LoadGroupBoxesFromFile(string path, Action<string> changeProgressStatusAction)
        {
            path = IOPath.GetFullPath(path);

            changeProgressStatusAction.Invoke($"メインメニューレイアウトファイルの読込中：　{path.Replace(AppDirectory + @"\", "")}");
            XDocument xDocument;
            try
            {
                xDocument = XDocument.Load(path);
            }
            catch
            {
                Throw($"ファイル \"{path}\" の読込に失敗しました。", ErrorType.Error);
                return;
            }

            XElement mainMenuLayout = xDocument.Element("MainMenuLayout");
            if (mainMenuLayout == null)
            {
                Throw($"ファイル \"{IOPath.GetFileName(path)}\" はメインメニューレイアウトファイルではありません。\nMainMenuLayout タグが見つかりません。", ErrorType.Error);
                return;
            }

            XElement left = mainMenuLayout.Element("Left");
            if (left == null)
            {
                Throw("Left タグが見つかりません。", ErrorType.Error);
                return;
            }

            XElement right = mainMenuLayout.Element("Right");
            if (right == null)
            {
                Throw("Right タグが見つかりません。", ErrorType.Error);
                return;
            }

            List<List<MainMenuGroupBox>> groupBoxInfos = new List<List<MainMenuGroupBox>>();

            int leftGroupBoxCount = 0;
            int leftTotalRow = 0;
            XElement currentSide = left;
            for (int m = 0; m < 2; m++)
            {
                List<MainMenuGroupBox> groupBoxInfosChild = new List<MainMenuGroupBox>();
                int groupBoxCount = 0;
                int totalRow = 0;

                int i = 0;
                IEnumerable<XElement> groupBoxes = currentSide.Elements("GroupBox");
                foreach (XElement groupBox in groupBoxes)
                {
                    string header = (string)groupBox.Attribute("Header") ?? "";

                    int row = (int?)groupBox.Attribute("Row") ?? -1;
                    if (row == -1)
                    {
                        Throw($"グループボックス \"{header}\" に Height 属性が見つかりません。", ErrorType.Error);
                        return;
                    }
                    else if (row == 0)
                    {
                        Throw($"グループボックス \"{header}\" の高さ \"{row}\" は非推奨の値です。", ErrorType.Warning);
                    }

                    List<MainMenuContent> contentInfos = new List<MainMenuContent>();

                    int n = 0;
                    IEnumerable<XElement> contents = groupBox.Elements("Content");
                    foreach (XElement content in contents)
                    {
                        List<TicketButton> tickets = new List<TicketButton>();

                        string groupSource = IOPath.GetFullPath(IOPath.Combine(IOPath.GetDirectoryName(path), (string)content.Attribute("GroupSource") ?? ""));
                        changeProgressStatusAction.Invoke($"券種メニューレイアウトファイルの読込中：　{groupSource.Replace(AppDirectory, "")}");
                        if (!File.Exists(groupSource))
                        {
                            Throw($"ファイル \"{groupSource}\" の読込に失敗しました。", ErrorType.Warning);
                        }
                        else
                        {
                            XDocument groupDocument = XDocument.Load(groupSource);

                            XElement groupMenuLayout = groupDocument.Element("GroupMenuLayout");
                            if (groupMenuLayout == null)
                            {
                                Throw($"ファイル \"{IOPath.GetFileName(groupSource)}\" は券種メニューレイアウトファイルではありません。\nGroupMenuLayout タグが見つかりません。", ErrorType.Warning, Screen.GroupMenu);
                            }
                            else
                            {
                                IEnumerable<XElement> groupMenuContents = groupMenuLayout.Elements("Content");
                                foreach (XElement groupMenuContent in groupMenuContents)
                                {
                                    string ticketPluginClassName = (string)groupMenuContent.Attribute("Type") ?? "";
                                    ITicketPlugin ticketPlugin = UserControlHost.TicketPlugins.Find(p => p.GetType().FullName == TicketPluginsNamespace + ticketPluginClassName);
                                    if (ticketPluginClassName == "")
                                    {

                                    }
                                    else if (ticketPlugin == null)
                                    {
                                        Throw($"券種 \"{TicketPluginsNamespace}{ticketPluginClassName}\" が見つかりません。", ErrorType.Warning, Screen.GroupMenu);
                                    }
                                    else
                                    {
                                        string typeName = (string)groupMenuContent.Attribute("Name") ?? "";
                                        tickets.Add(new TicketButton()
                                        {
                                            TypeName = typeName,
                                            TicketPlugin = ticketPlugin,
                                        });
                                    }
                                }
                                while (tickets.Count % 60 != 0)
                                {
                                    tickets.Add(new TicketButton()
                                    {
                                        TypeName = "",
                                        TicketPlugin = null,
                                    });
                                }
                            }
                        }

                        string typicalTicketPluginClassName = (string)content.Attribute("TypicalTicketType") ?? "";
                        ITicketPlugin typicalTicketPlugin = UserControlHost.TicketPlugins.Find(p => p.GetType().FullName == TicketPluginsNamespace + typicalTicketPluginClassName);
                        if (typicalTicketPluginClassName == "")
                        {

                        }
                        else if (typicalTicketPlugin == null)
                        {
                            Throw($"券種 \"{TicketPluginsNamespace}{typicalTicketPluginClassName}\" が見つかりません。", ErrorType.Warning);
                        }

                        string ticketGroupName = (string)content.Attribute("GroupName") ?? "";
                        contentInfos.Add(new MainMenuContent()
                        {
                            TicketGroupName = ticketGroupName,
                            TicketGroup = tickets,
                            TypicalTicketName = (string)content.Attribute("TypicalTicketName") ?? "",
                            TypicalTicket = typicalTicketPlugin,
                            Command = new DelegateCommand(() =>
                            {
                                UserControlHost.SetCurrentScreen(Screen.None);
                                DoEvents();
                                UserControlHost.SetCurrentTicket(typicalTicketPlugin, Screen.MainMenu);
                                UserControlHost.SetCurrentScreen(Screen.Tickets);
                            }),
                            OpenTabCommand = new DelegateCommand(() =>
                            {
                                UserControlHost.SetCurrentScreen(Screen.None);
                                DoEvents();
                                ((GroupMenuModel)UserControlHost.Models[Screen.GroupMenu]).CurrentGroup = new TicketGroup(ticketGroupName, tickets);
                                UserControlHost.SetCurrentScreen(Screen.GroupMenu);
                            }),
                        });

                        n++;
                    }

                    groupBoxInfosChild.Add(new MainMenuGroupBox()
                    {
                        Header = header,
                        Row = row,
                        Contents = contentInfos,
                    });

                    groupBoxCount++;
                    totalRow += row;

                    i++;
                }

                changeProgressStatusAction.Invoke("");

                groupBoxInfos.Add(groupBoxInfosChild);

                if (currentSide == left)
                {
                    if (totalRow > 5 && !((bool?)left.Attribute("IgnoreRowExcess") ?? false))
                    {
                        Throw("左側について、行数が多すぎる ( " + totalRow + "行 ) ため、レイアウトが崩れる恐れがあります。\n" +
                            "この警告を表示しないようにするには、Left タグの属性に「IgnoreRowExcess=\"True\"」を追加して下さい。", ErrorType.Warning);
                    }
                    else if (totalRow < 5 && !((bool?)left.Attribute("IgnoreRowShortage") ?? false))
                    {
                        Throw("左側について、行数が少なすぎる ( " + totalRow + "行 ) ため、" + (5 - totalRow) + " 行分の空白が表示されます。\n" +
                            "この警告を表示しないようにするには、Left タグの属性に「IgnoreRowShortage=\"True\"」を追加して下さい。", ErrorType.Warning);
                    }

                    leftTotalRow = totalRow;
                    leftGroupBoxCount = groupBoxCount;

                    currentSide = right;
                }
                else
                {
                    totalRow++;

                    if (totalRow > 5 && !((bool?)right.Attribute("IgnoreRowExcess") ?? false))
                    {
                        Throw("右側について、行数が多すぎる ( " + totalRow + "行 ) ため、レイアウトが崩れる恐れがあります。\n" +
                            "この警告を表示しないようにするには、Right タグの属性に「IgnoreRowExcess=\"True\"」を追加して下さい。", ErrorType.Warning);
                    }
                    else if (totalRow < 5 && !((bool?)right.Attribute("IgnoreRowShortage") ?? false))
                    {
                        Throw("右側について、行数が少なすぎる ( " + totalRow + "行 ) ため、" + (5 - totalRow) + " 行分の空白が表示されます。\n" +
                            "この警告を表示しないようにするには、Right タグの属性に「IgnoreRowShortage=\"True\"」を追加して下さい。", ErrorType.Warning);
                    }
                    else if (leftGroupBoxCount != groupBoxCount)
                    {
                        Throw("グループボックスの個数が左側と右側で異なる ( 左： " + leftGroupBoxCount + "個、右： " + groupBoxCount + "個 ) ため、ボタンの高さが不均一になります。", ErrorType.Warning);
                    }
                    else if (leftTotalRow != totalRow)
                    {
                        Throw("行数の合計が左側と右側で異なる ( 左： " + leftTotalRow + "行、右： " + totalRow + " 行 ) ため、ボタンの高さが不均一になります。", ErrorType.Warning);
                    }
                }
            }

            changeProgressStatusAction.Invoke("メインメニューレイアウトの登録中");
            GroupBoxInfos = groupBoxInfos;
        }

        public void LoadMaintenanceMenuFromFile(string path, Action<string> changeProgressStatusAction)
        {
            List<TicketButton> tickets = new List<TicketButton>();

            path = IOPath.GetFullPath(path);
            changeProgressStatusAction.Invoke($"メインテナンスメニューレイアウトファイルの読込中：　{path.Replace(AppDirectory, "")}");
            if (!File.Exists(path))
            {
                Throw($"ファイル \"{path}\" の読込に失敗しました。", ErrorType.Warning);
            }
            else
            {
                XDocument groupDocument = XDocument.Load(path);

                XElement groupMenuLayout = groupDocument.Element("GroupMenuLayout");
                if (groupMenuLayout == null)
                {
                    Throw($"ファイル \"{IOPath.GetFileName(path)}\" はメインテナンスメニューレイアウトファイルではありません。\nGroupMenuLayout タグが見つかりません。", ErrorType.Warning, Screen.GroupMenu);
                }
                else
                {
                    IEnumerable<XElement> groupMenuContents = groupMenuLayout.Elements("Content");
                    foreach (XElement groupMenuContent in groupMenuContents)
                    {
                        string ticketPluginClassName = (string)groupMenuContent.Attribute("Type") ?? "";
                        ITicketPlugin ticketPlugin = UserControlHost.TicketPlugins.Find(p => p.GetType().FullName == TicketPluginsNamespace + ticketPluginClassName);
                        if (ticketPluginClassName == "")
                        {

                        }
                        else if (ticketPlugin == null)
                        {
                            Throw($"メインテナンス項目 \"{TicketPluginsNamespace}{ticketPluginClassName}\" が見つかりません。", ErrorType.Warning, Screen.GroupMenu);
                        }
                        else
                        {
                            string typeName = (string)groupMenuContent.Attribute("Name") ?? "";
                            tickets.Add(new TicketButton()
                            {
                                TypeName = typeName,
                                TicketPlugin = ticketPlugin,
                            });
                        }
                    }
                    while (tickets.Count % 60 != 0)
                    {
                        tickets.Add(new TicketButton()
                        {
                            TypeName = "",
                            TicketPlugin = null,
                        });
                    }
                }
            }

            MaintenanceMenuContentGroup = new TicketGroup("メインテナンス", tickets);
        }


        private List<List<MainMenuGroupBox>> _GroupBoxInfos;
        public List<List<MainMenuGroupBox>> GroupBoxInfos
        {
            get { return _GroupBoxInfos; }
            set { SetProperty(ref _GroupBoxInfos, value); }
        }

        public TicketGroup MaintenanceMenuContentGroup { get; private set; }
    }

    public partial class MainMenu : UserControl
    {
        private void ViewModelPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext.GetType() != typeof(MainMenuViewModel)) return;

            MainMenuViewModel vm = (MainMenuViewModel)DataContext;
            vm.PropertyChanged += new PropertyChangedEventHandler((sender2, e2) =>
            {
                switch (e2.PropertyName)
                {
                    case nameof(vm.GroupBoxes):
                        Grid currentGrid = LeftGrid;
                        for (int n = 0; n < 2; n++)
                        {
                            for (int i = 0; i < vm.GroupBoxes[n].Count; i++)
                            {
                                currentGrid.RowDefinitions.Add(new RowDefinition()
                                {
                                    Height = new GridLength(vm.GroupBoxes[n][i].Row, GridUnitType.Star),
                                });
                                currentGrid.RowDefinitions.Add(new RowDefinition()
                                {
                                    Height = new GridLength(70),
                                });


                                UniformGrid uniformGrid = new UniformGrid();
                                for (int m = 0; m < vm.GroupBoxes[n][i].Contents.Count; m++)
                                {
                                    MainMenuContent content = vm.GroupBoxes[n][i].Contents[m];
                                    uniformGrid.Children.Add(new MainMenuButton()
                                    {
                                        GroupName = content.TicketGroupName,
                                        TypicalTicketName = content.TypicalTicketName,
                                        Command = content.Command,
                                        OpenTabCommand = content.OpenTabCommand,
                                    });
                                }
                                for (int m = vm.GroupBoxes[n][i].Contents.Count; m < vm.GroupBoxes[n][i].Row * 2; m++)
                                {
                                    uniformGrid.Children.Add(new MainMenuButton()
                                    {
                                        IsEnabled = false,
                                    });
                                }

                                GroupBox groupBox = new GroupBox()
                                {
                                    Header = vm.GroupBoxes[n][i].Header,
                                    Content = uniformGrid,
                                };
                                Grid.SetRow(groupBox, i * 2);
                                Grid.SetRowSpan(groupBox, 2);

                                currentGrid.Children.Add(groupBox);
                            }

                            if (currentGrid == LeftGrid) currentGrid = RightGrid;
                        }
                        RightGrid.RowDefinitions.Add(new RowDefinition()
                        {
                            Height = new GridLength(1, GridUnitType.Star),
                        });
                        Grid.SetRow(CoreButtons, RightGrid.RowDefinitions.Count - 1);
                        break;
                }
            });
        }
    }
}