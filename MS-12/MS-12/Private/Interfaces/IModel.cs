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
using TRS.TMS12.Interfaces;

namespace TRS.TMS12
{
    public interface IModel
    {
        UserControlsConnector UserControlsConnector { get; set; }
        List<ITicketPlugin> TicketPlugins { get; set; }
        Dictionary<Screen, IModel> Models { get; set; }
        DialogModel DialogModel { get; set; }

        ObservableCollection<bool> FIsEnabled { get; set; }

        void BeforeShown();
    }
}
