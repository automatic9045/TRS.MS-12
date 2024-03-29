﻿using System;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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
using System.Reflection;
using Microsoft.VisualBasic;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Gayak.Collections;

using TRS.TMS12.Interfaces;
using static TRS.TMS12.Static.App;

namespace TRS.TMS12
{
    public partial class MainMenuModel : BindableBase, IModel
    {
        public UserControlHost UserControlHost { get; set; }

        public ObservableCollection<bool> FIsEnabled { get; set; } = new ObservableCollection<bool>()
        {
            false,
            false, true, true, true, true, true, true, true, true, true, true, true, false, false, false,
            false, false, false, false,
            false, false, false,
        };

        public void BeforeShown()
        {
        }
    }

    public class MainMenuViewModel : BindableBase, IViewModel
    {
        public MainMenuViewModel()
        {
            OneTouchClicked = new DelegateCommand(() =>
            {
                m.UserControlHost.SetCurrentScreen(Screen.None);
                DoEvents();
                m.UserControlHost.SetCurrentScreen(Screen.OneTouchMenu);
            });

            MaintenanceClicked = new DelegateCommand(() =>
            {
                m.UserControlHost.SetCurrentScreen(Screen.None);
                DoEvents();
                ((GroupMenuModel)m.UserControlHost.Models[Screen.GroupMenu]).CurrentGroup = m.MaintenanceMenuContentGroup;
                m.UserControlHost.SetCurrentScreen(Screen.GroupMenu);
            });

            PowerOffClicked = new DelegateCommand(async () =>
            {
                if (await m.UserControlHost.DialogModel.ShowConfirmDialogAsync("ソフトウェアを終了します。\nよろしいですか？"))
                {
                    m.UserControlHost.DialogModel.ShowInformationDialog("プリンターの解放中", false);
                    DoEvents();
                    m.UserControlHost.CurrentPrinter.Dispose();
                    m.UserControlHost.DialogModel.ShowInformationDialog("シャットダウン中", false);
                    DoEvents();

                    Application.Current.Shutdown();
                }
            });
        }

        private void ModelSet()
        {
            m.PropertyChanged += new PropertyChangedEventHandler((sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(MainMenuModel.GroupBoxInfos):
                        GroupBoxes = m.GroupBoxInfos;
                        break;
                }
            });
        }

        private MainMenuModel m;
        public IModel Model
        {
            get { return m; }
            set
            {
                m = (MainMenuModel)value;
                ModelSet();
            }
        }

        private Visibility _Visibility = Visibility.Hidden;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { SetProperty(ref _Visibility, value); }
        }

        private List<List<MainMenuGroupBox>> _GroupBoxes;
        public List<List<MainMenuGroupBox>> GroupBoxes
        {
            get { return _GroupBoxes; }
            set { SetProperty(ref _GroupBoxes, value); }
        }

        public DelegateCommand OneTouchClicked { get; private set; }
        public DelegateCommand MaintenanceClicked { get; private set; }
        public DelegateCommand PowerOffClicked { get; private set; }
    }

    /// <summary>
    /// InputField.xaml の相互作用ロジック
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainMenu()
        {
            InitializeComponent();

            DataContextChanged += new DependencyPropertyChangedEventHandler(ViewModelPropertyChanged);
        }
    }
}