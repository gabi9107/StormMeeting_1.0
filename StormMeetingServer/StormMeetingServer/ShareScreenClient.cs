using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;

namespace StormMeetingServer
{
    class ShareScreenClient
    {
        private NetworkStream m_networkStream;
        private BinaryReader br;
        private BinaryWriter bw;
        public NetworkStream NetworkStream
        {
            get { return m_networkStream; }
            set { m_networkStream = value; }
        }
        public MemoryStream ms;
        
        private bool m_isPresenter;

        public bool IsPresenter
        {
            get { return m_isPresenter; }
            set { m_isPresenter = value; }
        }

        Socket m_socket;

        IFormatter formatter = new BinaryFormatter();
        public IPAddress IP
        {
            get
            {
                if (m_socket != null)
                    return ((IPEndPoint)this.m_socket.RemoteEndPoint).Address;
                else
                    return IPAddress.None;
            }
        }
        public ShareScreenClient(Socket socket, bool isPresenter)
        {
            m_isPresenter = isPresenter;

            m_socket = socket;

            m_networkStream = new NetworkStream(m_socket);

            #region bullshit
            //m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPAddress adress = IPAddress.Parse("127.0.0.1");
            //IPEndPoint iep = new IPEndPoint(adress, 5000);
            //EndPoint ep = (EndPoint)iep;
            //m_serverSocket.Bind(iep);
            //m_serverSocket.Listen(10);
            //m_clientSocket = m_serverSocket.Accept();
            //m_networkStream = new NetworkStream(m_clientSocket);

            //Thread oThread = new Thread(new ThreadStart(BeginReceive));
            //// Start the thread
            //oThread.Start();
            #endregion

            if (isPresenter)
            {
                Thread oThread = new Thread(new ThreadStart(StartReceive));
                //// Start the thread
                oThread.Start();
            }
        }

        private void StartReceive()
        {

            while (m_socket.Connected)
            {
                byte[] buffer = (byte[])formatter.Deserialize(m_networkStream);
                OnImageReceived(new ImageReceivedEventArgs(buffer));
            }
        }

        public void Write(byte[] buffer)
        {
            formatter.Serialize(m_networkStream, buffer);
        }

#region  events

        public event EventHandler ImageReceived;

        protected void OnImageReceived(EventArgs e)
        {
            if (ImageReceived != null)
                ImageReceived(this, e);
        }

#endregion

    }
}
