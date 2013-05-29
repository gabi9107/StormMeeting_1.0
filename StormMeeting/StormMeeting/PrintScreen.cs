using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StormMeeting
{
    public partial class PrintScreen : Form
    {
        bool first = true;
        ShareScreen Server;
        public PrintScreen(ShareScreen sc)
        {
            InitializeComponent();
            if (sc != null)
            {
                Server = sc;
                Server.ImmageReceived += new EventHandler(ImmageReceived);

            }
        }
        private void ImmageReceived(object sender, EventArgs e)
        {
            ImmageReceivedEventArgs ea = (ImmageReceivedEventArgs)e;
            //ea.Image.SetResolution(this.Size.Width, this.Size.Height);

                if (screen.InvokeRequired)
                    screen.Invoke(new MethodInvoker(delegate { screen.Image = ea.Image; }));
                else
                    screen.Image = ea.Image;
                first = false;


        }



    }
}
