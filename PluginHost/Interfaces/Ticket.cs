using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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

namespace TRS.TMS12.Interfaces
{
    public abstract class TicketBase
    {
        public Bitmap Bitmap { get; }

        public abstract TicketBase Resend();

        public TicketBase(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }

    public class TicketInfo
    {
        public TicketBase Ticket { get; }
        public bool HasPrinted { get; private set; } = false;

        public TicketInfo(TicketBase ticket)
        {
            Ticket = ticket;
        }

        public void OnPrint()
        {
            HasPrinted = true;
        }
    }
}
