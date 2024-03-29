﻿using System;
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

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public class TicketGroup
    {
        public string Name { get; private set; }
        public List<TicketButton> Contents { get; private set; }
        public TicketGroup(string name, List<TicketButton> contents)
        {
            Name = name;
            Contents = contents;
        }
    }

    public  partial class GroupMenuModel : BindableBase, IModel
    {
        public UserControlHost UserControlHost { get; set; }

        public ObservableCollection<bool> FIsEnabled { get; set; } = new ObservableCollection<bool>()
        {
            false,
            false, true, false, true, true, true, true, true, true, true, true, true, false, false, false,
            false, false, false, false,
            true, true, false,
        };

        private TicketGroup _CurrentGroup;
        public TicketGroup CurrentGroup
        {
            get { return _CurrentGroup; }
            set { SetProperty(ref _CurrentGroup, value); }
        }

        public void BeforeShown()
        {
        }
    }

    public class GroupMenuViewModel : BindableBase, IViewModel
    {
        private const int BUTTON_COUNT = 32;

        public GroupMenuViewModel()
        {
            Cancel = new DelegateCommand(() =>
            {
                m.UserControlHost.SetCurrentScreen(Screen.None);
                DoEvents();
                m.UserControlHost.SetCurrentScreen(Screen.MainMenu);
            });

            NotImplemented = new DelegateCommand(() =>
            {
                m.UserControlHost.DialogModel.ShowNotImplementedDialog();
            });
        }

        private void ModelSet()
        {
            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(m.CurrentGroup):
                        int i = 0;
                        List<string> contents = m.CurrentGroup.Contents.FindAll(content => content.TypeName != "").ConvertAll(content =>
                        {
                            string text = $"{string.Format("{0:D2}", i + 1)} {content.TypeName}";
                            i++;
                            return text;
                        });
                        contents.AddRange(Enumerable.Repeat(string.Empty, BUTTON_COUNT - contents.Count));

                        List<DelegateCommand> clicked = m.CurrentGroup.Contents.ConvertAll(c => new DelegateCommand(() =>
                        {
                            m.UserControlHost.SetCurrentScreen(Screen.None);
                            DoEvents();
                            m.UserControlHost.SetCurrentTicket(c.TicketPlugin, Screen.GroupMenu);
                            m.UserControlHost.SetCurrentScreen(Screen.Tickets);
                        }));

                        Type = m.CurrentGroup.Name;
                        Contents = contents;
                        Clicked = clicked;
                        break;
                }
            });
        }

        private GroupMenuModel m;
        public IModel Model
        {
            get { return m; }
            set
            {
                m = (GroupMenuModel)value;
                ModelSet();
            }
        }

        private Visibility _Visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        private string _GuideNumber = "";
        public string GuideNumber
        {
            get { return _GuideNumber; }
            set { SetProperty(ref _GuideNumber, value); }
        }

        private string _Type = "";
        public string Type
        {
            get { return _Type; }
            set { SetProperty(ref _Type, value); }
        }

        private List<string> _Contents;
        public List<string> Contents
        {
            get { return _Contents; }
            set { SetProperty(ref _Contents, value); }
        }

        private List<DelegateCommand> _Clicked;
        public List<DelegateCommand> Clicked
        {
            get { return _Clicked; }
            set { SetProperty(ref _Clicked, value); }
        }

        public DelegateCommand Cancel { get; set; }

        public DelegateCommand NotImplemented { get; set; }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class GroupMenu : UserControl
    {
        public GroupMenu()
        {
            InitializeComponent();
        }
    }
}