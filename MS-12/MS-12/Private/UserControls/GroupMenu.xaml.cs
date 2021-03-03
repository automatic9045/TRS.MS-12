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
        public UserControlsConnector UserControlsConnector { get; set; }
        public List<ITicketPlugin> TicketPlugins { get; set; }
        public Dictionary<UserControls, IModel> Models { get; set; }
        public DialogModel DialogModel { get; set; }

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
                m.UserControlsConnector.SetShowingUserControl(UserControls.None);
                DoEvents();
                m.UserControlsConnector.SetShowingUserControl(UserControls.MainMenu);
            });

            NotImplemented = new DelegateCommand(() =>
            {
                m.DialogModel.ShowNotImplementedDialog();
            });
        }

        private void ModelSet()
        {
            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(m.CurrentGroup):
                        List<string> contents = new List<string>();
                        for (int i = 0; i < BUTTON_COUNT; i++)
                        {
                            if (i < m.CurrentGroup.Contents.Count && m.CurrentGroup.Contents[i].TypeName != "")
                            {
                                contents.Add(String.Format("{0:D2}", i + 1) + " " + m.CurrentGroup.Contents[i].TypeName);
                            }
                            else
                            {
                                contents.Add(string.Empty);
                            }
                        }

                        List<DelegateCommand> clicked = new List<DelegateCommand>();
                        foreach (TicketButton c in m.CurrentGroup.Contents)
                        {
                            clicked.Add(new DelegateCommand(() =>
                            {
                                m.UserControlsConnector.SetShowingUserControl(UserControls.None);
                                DoEvents();
                                m.UserControlsConnector.SetCurrentTicket(c.TicketPlugin, UserControls.GroupMenu);
                                m.UserControlsConnector.SetShowingUserControl(UserControls.Tickets);
                            }));
                        }

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