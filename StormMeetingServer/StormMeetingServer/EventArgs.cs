using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormMeetingServer
{
    class MessageReceivedEventArgs : EventArgs
    {
        private MessageType m_type;

        public MessageType Type
        {
            get { return m_type; }
        }
        
        //private string m_sender;

        //public string Sender
        //{
        //    get { return m_sender; }
        //}
       
        //private string m_receiver;

        //public string Receiver
        //{
        //    get { return m_receiver; }
        //}
        
        //private string m_message;

        //public string Message
        //{
        //    get { return m_message; }
        //}

        //public MessageReceivedEventArgs(MessageType type, string sender, string receiver, string message)
        //{
        //    m_type = type;
        //    m_sender = sender;
        //    m_receiver = receiver;
        //    m_message = message;
        //}

        private byte[] m_messageBytes;

        public byte[] MessageBytes
        {
            get { return m_messageBytes; }
        }

        public MessageReceivedEventArgs(MessageType type, byte[] bytes)
        {
            m_messageBytes = bytes;
        }
        public MessageReceivedEventArgs(byte[] bytes)
        {
            m_messageBytes = bytes;
        }
    }

    class ImageReceivedEventArgs : EventArgs
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
}
