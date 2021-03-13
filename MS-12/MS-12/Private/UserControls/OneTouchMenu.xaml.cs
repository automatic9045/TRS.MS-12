using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using System.Reflection;
using System.ComponentModel;
using Microsoft.VisualBasic;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Gayak.Collections;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;
using System.Globalization;

namespace TRS.TMS12
{
    public partial class OneTouchMenuModel : BindableBase, IModel
    {
        public UserControlsConnector UserControlsConnector { get; set; }
        public List<ITicketPlugin> TicketPlugins { get; set; }
        public Dictionary<Screen, IModel> Models { get; set; }
        public DialogModel DialogModel { get; set; }

        public ObservableCollection<bool> FIsEnabled { get; set; } = new ObservableCollection<bool>()
        {
            false,
            false, true, false, true, true, true, true, true, true, true, true, true, false, false, false,
            false, false, false, false,
            true, true, false,
        };

        private int _CurrentGroup = 0;
        public int CurrentGroup
        {
            get => _CurrentGroup;
            set => SetProperty(ref _CurrentGroup, value);
        }

        private int _CurrentPage = 0;
        public int CurrentPage
        {
            get => _CurrentPage;
            set => SetProperty(ref _CurrentPage, value);
        }

        public void BeforeShown()
        {

        }
    }

    public class OneTouchMenuViewModel : BindableBase, IViewModel
    {
        public OneTouchMenuViewModel()
        {
            ShortcutCommand = new DelegateCommand<string>(param =>
            {
                m.UserControlsConnector.SetCurrentScreen(Screen.None);
                DoEvents();

                TicketButton button = m.Groups[m.CurrentGroup].Shortcuts[int.Parse(param)];
                m.UserControlsConnector.SetCurrentTicket(button.TicketPlugin, Screen.OneTouchMenu, button.Command);

                m.UserControlsConnector.SetCurrentScreen(Screen.Tickets);
            });

            Command = new DelegateCommand<string>(param =>
            {
                m.UserControlsConnector.SetCurrentScreen(Screen.None);
                DoEvents();

                TicketButton button = m.Groups[m.CurrentGroup].Pages[m.CurrentPage].Buttons[int.Parse(param)];
                m.UserControlsConnector.SetCurrentTicket(button.TicketPlugin, Screen.OneTouchMenu, button.Command);

                m.UserControlsConnector.SetCurrentScreen(Screen.Tickets);
            });

            Previous = new DelegateCommand(() => m.CurrentPage--);
            Next = new DelegateCommand(() => m.CurrentPage++);

            GroupChanged = new DelegateCommand<string>(i => UpdateGroup(int.Parse(i)));
            PageChanged = new DelegateCommand<string>(i => UpdatePage(int.Parse(i)));
        }

        private void ModelSet()
        {
            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(m.Groups):
                        GroupNames = m.Groups.ConvertAll(g => g.Header);
                        UpdateGroup(0);
                        UpdatePage(0);

                        m.PropertyChanged += new PropertyChangedEventHandler((sender2, e2) =>
                        {
                            switch (e2.PropertyName)
                            {
                                case nameof(m.CurrentGroup):
                                    GroupsIsChecked[m.CurrentGroup] = true;
                                    break;

                                case nameof(m.CurrentPage):
                                    PagesIsChecked[m.CurrentPage] = true;
                                    break;
                            }
                        });
                        break;
                }
            });

            GroupsIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                m.CurrentGroup = GroupsIsChecked.IndexOf(true);
            });

            PagesIsChecked.CollectionChanged += new NotifyCollectionChangedEventHandler((sender, e) =>
            {
                m.CurrentPage = PagesIsChecked.IndexOf(true);
            });
        }

        private void UpdateGroup(int index)
        {
            m.CurrentGroup = index;
            OneTouchMenuGroup group = m.Groups[index];
            ShortcutNames = group.Shortcuts.ConvertAll(s => s.TypeName);
            PageNames = group.Pages.ConvertAll(p => p.Name);
            UpdatePage(0);
        }

        private void UpdatePage(int index)
        {
            m.CurrentPage = index;
            Keys = m.Groups[m.CurrentGroup].Pages[index].Buttons.ConvertAll(k => k.TypeName);
            IsNotLast = m.CurrentPage + 1 < PageNames.IndexOf("");
            IsNotFirst = 0 < m.CurrentPage;
        }

        private OneTouchMenuModel m;
        public IModel Model
        {
            get => m;
            set
            {
                m = (OneTouchMenuModel)value;
                ModelSet();
            }
        }

        private Visibility _Visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        private bool _IsNotFirst;
        public bool IsNotFirst
        {
            get { return _IsNotFirst; }
            set { SetProperty(ref _IsNotFirst, value); }
        }

        private bool _IsNotLast;
        public bool IsNotLast
        {
            get { return _IsNotLast; }
            set { SetProperty(ref _IsNotLast, value); }
        }

        private List<string> _ShortcutNames;
        public List<string> ShortcutNames
        {
            get { return _ShortcutNames; }
            set { SetProperty(ref _ShortcutNames, value); }
        }

        private List<string> _GroupNames;
        public List<string> GroupNames
        {
            get { return _GroupNames; }
            set { SetProperty(ref _GroupNames, value); }
        }

        private List<string> _Keys;
        public List<string> Keys
        {
            get { return _Keys; }
            set { SetProperty(ref _Keys, value); }
        }

        private List<string> _PageNames;
        public List<string> PageNames
        {
            get { return _PageNames; }
            set { SetProperty(ref _PageNames, value); }
        }

        public ObservableCollection<bool> GroupsIsChecked { get; private set; } = new ObservableCollection<bool>()
        {
            true, false, false, false, false, false, false, false, false,
        };

        public ObservableCollection<bool> PagesIsChecked { get; private set; } = new ObservableCollection<bool>()
        {
            true, false, false, false, false, false, false, false, false, false,
        };

        public DelegateCommand<string> ShortcutCommand { get; private set; }
        public DelegateCommand<string> Command { get; private set; }
        public DelegateCommand Previous { get; private set; }
        public DelegateCommand Next { get; private set; }
        public DelegateCommand<string> GroupChanged { get; private set; }
        public DelegateCommand<string> PageChanged { get; private set; }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class OneTouchMenu : UserControl
    {
        public OneTouchMenu()
        {
            InitializeComponent();
        }
    }
}