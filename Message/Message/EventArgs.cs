using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDll
{
    public class MessageReceivedEventArgs : EventArgs
    {
        private Message m_message;

        public Message Message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        public MessageReceivedEventArgs(Message message)
        {
            m_message = message;
        }
    }

    public class ClientLoginEventArgs : EventArgs
    {
        string m_name;

        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public ClientLoginEventArgs(string name)
        {
            m_name = name;
        }
    }
    public class ImageReceivedEventArgs : EventArgs
    {
        private byte[] m_bytes;

        public byte[] Bytes
        {
            get { return m_bytes; }
            set { m_bytes = value; }
        }

        public ImageReceivedEventArgs(byte[] bytes)
        {
            m_bytes = bytes;
        }

    }

    public class ClientImageReceivedEventArgs : EventArgs
    {

        private Image m_image;

        public Image Image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        public ClientImageReceivedEventArgs(Image image)
        {
            m_image = image;
        }
    }
}
