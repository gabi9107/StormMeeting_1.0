using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessageDll;

namespace StormMeeting
{
    public partial class MainScreen : Form
    {
        ServerConnection Server;
        public MainScreen(ServerConnection sc)
        {
            InitializeComponent();
            if (sc != null)
            {
                Server = sc;
                Server.MessageReceived += new EventHandler(MessageReceived);

            }
        }
        private void SendButton_Click(object sender, EventArgs e)
        {
            Server.SendMessage(SendMessageBox.Text.Trim());   
        }

        private void MessageReceived(object sender, EventArgs e)
        {
            MessageDll.Message msg = ((MessageReceivedEventArgs)e).Message;

            if (ReceivedMessageBox.InvokeRequired)
            {
                ReceivedMessageBox.Invoke(new MethodInvoker(delegate { ReceivedMessageBox.Text += msg.MessageText; }));
            }
    
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread shareScreen = new Thread(
            (ThreadStart)delegate()
            {
                ShareScreen sc = new ShareScreen(true);
                Application.Run(new PrintScreen(sc));
            });

            shareScreen.Start();
        }

        private void SendButton_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'c')
                SendButton_Click(sender, e);
        }

    }
}
