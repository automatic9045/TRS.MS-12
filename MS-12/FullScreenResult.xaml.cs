using System;
using System.ComponentModel;
using System.Collections.Generic;
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

using IOPath = System.IO;

using Prism.Mvvm;
using Prism.Commands;

using TRS.TMS12.Interfaces;

namespace TRS.TMS12
{
    public partial class FullScreenResultControl
    {
        public ResultControlViewModel vm;

        public FullScreenResultControl()
        {
            InitializeComponent();
        }
    }
}
