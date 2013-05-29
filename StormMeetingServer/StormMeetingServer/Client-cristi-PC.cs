using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StormMeetingServer
{
    class Client
    {
        private string m_clientName;
        private BackgroundWorker bwReceiver;
        private NetworkStream m_networkStream;
        private MemoryStream ms;
        IFormatter formatter = new BinaryFormatter();

        public NetworkStream NetworkStream
        {
            get { return m_networkStream; }
        }

        public string ClientName
        {
            get { return m_clientName; }
            set { m_clientName = value; }
        }
        private Socket m_socket;

        public Socket ClientSocket
        {
            get { return m_socket; }
            set { m_socket = value; }
        }

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

        public Client(Socket socket)
        {
            m_socket = socket;
            
            m_networkStream = new NetworkStream(m_socket);

            //bwReceiver = new BackgroundWorker();
            //bwReceiver.DoWork += new DoWorkEventHandler(StartReceive);
            //bwReceiver.RunWorkerAsync();

            Thread oThread = new Thread(new ThreadStart(StartReceive));

            // Start the thread
            oThread.Start();
        }

        private int ReadNumber(int size)
        {
            byte[] buffer = new byte[size];
            int readBytes = ms.Read(buffer, 0, size);
            if (readBytes == 0)
                return 0;
            else
                return BitConverter.ToInt32(buffer, 0);
        }

        private string ReadString(int size)
        {
            byte[]  buffer = new byte[size];
            int readBytes = ms.Read(buffer, 0, size);
            if (readBytes == 0)
                return "";
            return BitConverter.ToString(buffer);
        }

        private byte[] ReadBytes(int size)
        {
            byte[] buffer = new byte[size];
            int readBytes = ms.Read(buffer, 0, size);
            if (readBytes == 0)
                return null;
            return buffer;
        }
        
        private void StartReceive()
        {

            while (m_socket.Connected)
            {
                //Bitmap buffer = (Bitmap)formatter.Deserialize(m_networkStream);
                byte[] buffer = (byte[])formatter.Deserialize(m_networkStream);
                Byte[] buffer = new Byte[4];
                m_socket.Receive(buffer);

                int size = BitConverter.ToInt32(buffer, 0);
                buffer = new Byte[size];
                m_socket.Receive(buffer);

                ms = new MemoryStream(buffer);
                MessageReceivedEventArgs messageArgs;

                //Read the command's Type.
                CommandType cmdType = (CommandType)(ReadNumber(4));
                switch (cmdType)
                {
                    case CommandType.ClientLogin:
                        //Read ClientName size
                        size = ReadNumber(4);

                        //Read ClientName
                        m_clientName = ReadString(size);
                        ClientLogin(this, EventArgs.Empty);
                        break;
                    case CommandType.Message:
                        //Read message type.
                        MessageType msgType = (MessageType)(ReadNumber(4));
                        if (msgType == MessageType.Broadcast)
                        {
                            messageArgs = new MessageReceivedEventArgs(msgType, BitConverter.GetBytes(size).Concat(buffer).ToArray());
                            OnMessageReceived(messageArgs);
                        }
                        break;
                   
                }
            }
        }

        #region Events

        public event EventHandler ClientLogin;

        protected void OnClientLogin(EventArgs e)
        {
            if (ClientLogin != null)
                ClientLogin(this, e);
        }

        public event EventHandler MessageReceived;

        protected void OnMessageReceived(EventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        #endregion

    }
}
