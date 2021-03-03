using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRS.TMS12.Interfaces
{
    public interface ITicketPluginExtension : ITicketPlugin
    {
        int CurrentTextBox { get; set; }
        ObservableCollection<string> TextBoxes { get; }

        void AddOption(string value);

        void SetDiscount(string value);
    }
}
