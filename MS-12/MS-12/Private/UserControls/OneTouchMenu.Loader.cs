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
    public class OneTouchMenuGroupPage
    {
        public string Name { get; set; }
        public List<TicketButton> Buttons { get; set; }
    }

    public class OneTouchMenuGroup
    {
        public string Header { get; set; } = "";
        public List<TicketButton> Shortcuts { get; set; }
        public List<OneTouchMenuGroupPage> Pages { get; set; }
    }

    public partial class OneTouchMenuModel : BindableBase, IModel
    {
        public void LoadFromFile(string path)
        {
            void ThrowError(string text, bool isInGroupMenu = false)
            {
                string menuName = isInGroupMenu ? "券種メニュー" : "メインメニュー";
                UserControlHost.Throw(text, menuName + "設定に関するエラー", ErrorType.Error);
            }

            void ThrowWarning(string text, bool isInGroupMenu = false)
            {
                string menuName = isInGroupMenu ? "券種メニュー" : "メインメニュー";
                UserControlHost.Throw(text, menuName + "設定に関する警告", ErrorType.Warning);
            }

            void ThrowInformation(string text, bool isInGroupMenu = false)
            {
                string menuName = isInGroupMenu ? "券種メニュー" : "メインメニュー";
                UserControlHost.Throw(text, menuName + "設定に関する情報", ErrorType.Warning);
            }


            XDocument oneTouchMenuLayoutFile;
            try
            {
                oneTouchMenuLayoutFile = XDocument.Load(path);
            }
            catch
            {
                ThrowError("ファイル \"" + path + "\" の読込に失敗しました。");
                return;
            }

            XElement oneTouchMenuLayout = oneTouchMenuLayoutFile.Element("OneTouchMenuLayout");
            if (oneTouchMenuLayout == null)
            {
                ThrowError("ファイル \"" + IOPath.GetFileName(path) + "\" はワンタッチメニューレイアウトファイルではありません。\n" +
                    "OneTouchMenuLayout タグが見つかりません。");
                return;
            }

            List<OneTouchMenuGroup> groups = new List<OneTouchMenuGroup>();

            foreach (XElement groupElement in oneTouchMenuLayout.Elements("Group"))
            {
                XDocument oneTouchMenuGroupFile;
                string groupPath = IOPath.Combine(IOPath.GetDirectoryName(path), (string)groupElement.Attribute("Source"));
                try
                {
                    oneTouchMenuGroupFile = XDocument.Load(groupPath);
                }
                catch
                {
                    ThrowWarning("ファイル \"" + groupPath + "\" の読込に失敗しました。");
                    continue;
                }

                XElement oneTouchMenuGroup = oneTouchMenuGroupFile.Element("OneTouchMenuGroup");
                if (oneTouchMenuGroup == null)
                {
                    ThrowWarning("ファイル \"" + IOPath.GetFileName(groupPath) + "\" はワンタッチメニューグループファイルではありません。\n" +
                        "OneTouchMenuGroup タグが見つかりません。");
                    continue;
                }

                List<TicketButton> shortcuts = new List<TicketButton>();
                XElement shortcutsElement = oneTouchMenuGroup.Element("Shortcuts");
                if (shortcutsElement == null)
                {
                    ThrowWarning("ワンタッチメニューグループファイル \"" + IOPath.GetFileName(groupPath) + "\" で Shortcuts タグが見つかりません。", true);
                }
                else
                {
                    foreach (XElement shortcutElement in shortcutsElement.Elements("Shortcut"))
                    {
                        string ticketPluginClassName = (string)shortcutElement.Attribute("Type") ?? "";
                        ITicketPlugin ticketPlugin = UserControlHost.TicketPlugins.Find(p => p.GetType().FullName == TicketPluginsNamespace + ticketPluginClassName);
                        if (ticketPluginClassName == "")
                        {

                        }
                        else if (ticketPlugin == null)
                        {
                            ThrowWarning("券種 \"" + TicketPluginsNamespace + ticketPluginClassName + "\" が見つかりません。", true);
                        }
                        else
                        {
                            shortcuts.Add(new TicketButton()
                            {
                                TypeName = (string)shortcutElement.Attribute("Name") ?? "",
                                TicketPlugin = ticketPlugin,
                            });
                        }
                    }
                    while (shortcuts.Count < 9)
                    {
                        shortcuts.Add(new TicketButton()
                        {
                            TypeName = "",
                            TicketPlugin = null,
                        });
                    }
                }

                List<OneTouchMenuGroupPage> pages = new List<OneTouchMenuGroupPage>();
                XElement pagesElement = oneTouchMenuGroup.Element("Pages");
                if (pagesElement == null)
                {
                    ThrowWarning("ワンタッチメニューグループファイル \"" + IOPath.GetFileName(groupPath) + "\" で Pages タグが見つかりません。", true);
                }
                else
                {
                    foreach (XElement pageElement in pagesElement.Elements("Page"))
                    {
                        List<TicketButton> buttons = new List<TicketButton>();
                        foreach (XElement buttonElement in pageElement.Elements("Content"))
                        {
                            string ticketPluginClassName = (string)buttonElement.Attribute("Type") ?? "";
                            ITicketPlugin ticketPlugin = UserControlHost.TicketPlugins.Find(p => p.GetType().FullName == TicketPluginsNamespace + ticketPluginClassName);
                            if (ticketPluginClassName == "")
                            {

                            }
                            else if (ticketPlugin == null)
                            {
                                ThrowWarning("券種 \"" + TicketPluginsNamespace + ticketPluginClassName + "\" が見つかりません。", true);
                            }
                            else
                            {
                                buttons.Add(new TicketButton()
                                {
                                    TypeName = (string)buttonElement.Attribute("Name") ?? "",
                                    TicketPlugin = ticketPlugin,
                                    Command = (string)buttonElement.Attribute("Command") ?? "",
                                });
                            }
                        }
                        while (buttons.Count < 90)
                        {
                            buttons.Add(new TicketButton()
                            {
                                TypeName = "",
                                TicketPlugin = null,
                            });
                        }

                        pages.Add(new OneTouchMenuGroupPage()
                        {
                            Name = (string)pageElement.Attribute("Name") ?? "",
                            Buttons = buttons,
                        });
                    }
                    while (pages.Count < 10)
                    {
                        pages.Add(new OneTouchMenuGroupPage()
                        {
                            Name = "",
                            Buttons = null,
                        });
                    }
                }

                groups.Add(new OneTouchMenuGroup()
                {
                    Header = (string)groupElement.Attribute("Header") ?? "",
                    Shortcuts = shortcuts,
                    Pages = pages,
                });
            }
            while (groups.Count < 9)
            {
                groups.Add(new OneTouchMenuGroup()
                {
                    Header = "",
                    Shortcuts = null,
                    Pages = null,
                });
            }

            Groups = groups;
        }


        private List<OneTouchMenuGroup> _Groups;
        public List<OneTouchMenuGroup> Groups
        {
            get { return _Groups; }
            set { SetProperty(ref _Groups, value); }
        }
    }
}