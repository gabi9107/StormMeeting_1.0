using MessageDll;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StormMeetingServer
{
    class Server
    {
        private Socket m_serverSocket;
        private Socket m_serverShareScreenSocket;

        private System.Collections.Generic.List<Client> m_clients;
        private List<ShareScreenClient> m_shareScreenClients;

        internal List<ShareScreenClient> ShareScreenClients
        {
            get { return m_shareScreenClients; }
            set { m_shareScreenClients = value; }
        }

        IFormatter formatter = new BinaryFormatter();

        internal System.Collections.Generic.List<Client> Clients
        {
            get { return m_clients; }
            set { m_clients = value; }
        }
       
        public Server()
        {
            m_clients = new List<Client>();

            m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress adress = IPAddress.Any;
            IPEndPoint iep = new IPEndPoint(adress, 4000);
            EndPoint ep = (EndPoint)iep;
            m_serverSocket.Bind(iep);
            m_serverSocket.Listen(10);

            if (true)
            {
                m_shareScreenClients = new List<ShareScreenClient>();
                m_serverShareScreenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                iep = new IPEndPoint(adress, 5000);
                ep = (EndPoint)iep;
                m_serverShareScreenSocket.Bind(iep);
                m_serverShareScreenSocket.Listen(10);

                Thread fThread = new Thread(new ThreadStart(StartToListenForShareScreen));
                // Start the thread
                fThread.Start();
            }

            Thread sThread = new Thread(new ThreadStart(StartToListen));
            // Start the thread
            sThread.Start();
        }

        private void StartToListen()
        {
            while (true)
                CreateNewClient(m_serverSocket.Accept());
        }

        private void StartToListenForShareScreen()
        {
            while (true)
            {
                CreateNewShareScreenClient(m_serverShareScreenSocket.Accept());
            }
        }

        private void CreateNewClient(Socket socket)
        {
            Client client = new Client(socket);
            client.ClientLogin += new EventHandler(ClientLogin);
            client.MessageReceived += new EventHandler(MessageReceived);
            Clients.Add(client);

            Console.WriteLine("S-a conectat : " + client.IP);

        }

        private void CreateNewShareScreenClient(Socket socket)
        {
            bool isPresenter = (ShareScreenClients.Count() == 0); 

            ShareScreenClient client = new ShareScreenClient(socket, isPresenter);

            client.ImageReceived += new EventHandler(ImageReceived);
            ShareScreenClients.Add(client);

            Console.WriteLine("S-a conectat la share screen : " + client.IP);

        }

        private void BroadcastMessage(Message message)
        {
            foreach (Client client in Clients)
            {
                formatter.Serialize(client.NetworkStream, message);
            }
        }

        #region events

        private void ClientLogin(object sender, EventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        private void MessageReceived(object sender, EventArgs e)
        {
            Message message = ((MessageReceivedEventArgs)e).Message;
            if (message.MsgType == MessageType.Broadcast)
                BroadcastMessage(message);
        }

        private void ImageReceived(object sender, EventArgs e)
        {
            ImageReceivedEventArgs imageArgs = (ImageReceivedEventArgs)e;
            
            //daca va fi necesar se va implemnta paralel
            foreach (ShareScreenClient client in ShareScreenClients)
            {
                client.Write(imageArgs.Bytes);

                //formatter.Serialize(client.NetworkStream, imageArgs.Image);
            }

        }

        #endregion
    }
}
