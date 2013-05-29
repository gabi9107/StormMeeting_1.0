using MessageDll;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormMeeting
{
    class MessageReceivedEventArgs : EventArgs
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

    class ImmageReceivedEventArgs : EventArgs
    {

        private Image m_image;

        public Image Image
        {
            get { return m_image; }
            set { m_image = value; }
        }

        public ImmageReceivedEventArgs(Image image)
        {
            m_image = image;
        }
    }
}
