using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StormMeeting
{
    public class ServerConnection
    {
        Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private NetworkStream m_networkStream;
        private string m_name  = "Ion";
        private MemoryStream ms;
        private bool shareScreen = true;
        

        public ServerConnection()
        {
            IPAddress adress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEnd = new IPEndPoint(adress, 4000);
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                m_socket.Connect(ipEnd);
            }
            catch (SocketException e)
            {
                m_socket.Close();
            }
            m_networkStream = new NetworkStream(m_socket);
            //BackgroundWorker bwListener = new BackgroundWorker();
            //bwListener.DoWork += new DoWorkEventHandler(ReceiveMessage);
            //bwListener.RunWorkerAsync();


            Thread oThread = new Thread(new ThreadStart(ReceiveMessage));
            // Start the thread
            oThread.Start();
        }

        public bool SendMessage(string message)
        {
            try
            {
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] messageBytes;
                //command type
                messageBytes = BitConverter.GetBytes((int)CommandType.Message);

                //message type
                messageBytes = messageBytes.Concat(BitConverter.GetBytes((int)MessageType.Broadcast)).ToArray();

                //concat sender name size
                messageBytes = messageBytes.Concat(BitConverter.GetBytes(m_name.Length)).ToArray();

                //concat sender name
                messageBytes = messageBytes.Concat(encoding.GetBytes(m_name)).ToArray();

                //concat message size
                messageBytes = messageBytes.Concat(BitConverter.GetBytes(message.Length)).ToArray();

                //concat message
                messageBytes = messageBytes.Concat(encoding.GetBytes(message)).ToArray();


                messageBytes = BitConverter.GetBytes(messageBytes.Length).Concat(messageBytes).ToArray();



                //m_networkStream.Write(messageBytes, 0, messageBytes.Length);
                m_socket.Send(messageBytes);
                return true;
            }
            catch (SocketException e)
            {
                return false;
            }
        }

        private void ReceiveMessage()
        {
            while (m_socket.Connected)
            {
                Byte[] buffer = new Byte[4];
                m_socket.Receive(buffer);

                ms = new MemoryStream(buffer);
                int size = ReadNumber(4);
                buffer = new Byte[size];
                m_socket.Receive(buffer);
                ms = new MemoryStream(buffer);

                
                CommandType cmdType = (CommandType)(ReadNumber(4));
                switch (cmdType)
                {
                    case CommandType.ClientLogin:
                        //Read ClientName size
                        size = ReadNumber(4);

                        //Read ClientName
                        string clientName = ReadString(size);
                        ClientLogin();
                        break;
                    case CommandType.Message:
                        //Read message type.
                        MessageType msgType = (MessageType)(ReadNumber(4));
                        if (msgType == MessageType.Broadcast)
                        {
                            //read sender name size
                            size = ReadNumber(4);
                            if (size == 0)
                                break;
                            //Read sender name
                            string senderName = ReadString(size);

                            //read message size
                            size = ReadNumber(4);

                            //read message
                            string message = ReadString(size);

                            MessageReceivedEventArgs messageArgs = new MessageReceivedEventArgs(message);
                            OnMessageReceived(messageArgs);
                        }
                        break;
                }
            }
        }

        private void ClientLogin()
        {
           // throw new NotImplementedException();
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
            return System.Text.ASCIIEncoding.ASCII.GetString(buffer);
        }

        private byte[] ReadBytes(int size)
        {
            byte[] buffer = new byte[size];
            int readBytes = ms.Read(buffer, 0, 4);
            if (readBytes == 0)
                return null;
            return buffer;
        }


        #region Events

        public event EventHandler MessageReceived;

        protected void OnMessageReceived(EventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        #endregion
    }
}
