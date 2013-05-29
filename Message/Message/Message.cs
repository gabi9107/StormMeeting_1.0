using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDll
{
    [Serializable]
    public class Message
    {
        private CommandType m_cmdType;

        public CommandType CmdType
        {
            get { return m_cmdType; }
            set { m_cmdType = value; }
        }

        private MessageType m_msgType;

        public MessageType MsgType
        {
            get { return m_msgType; }
            set { m_msgType = value; }
        }

        private string m_messageText;

        public string MessageText
        {
            get { return m_messageText; }
            set { m_messageText = value; }
        }

        private string m_from;

        public string From
        {
            get { return m_from; }
            set { m_from = value; }
        }

        private string m_to;

        public string To
        {
            get { return m_to; }
            set { m_to = value; }
        }

        public Message(CommandType cmdType, MessageType msgType, string messageText, string from, string to = null)
        {
            m_cmdType = cmdType;
            m_msgType = msgType;
            m_messageText = messageText;
            m_from = from;
            m_to = to;
        }
    }
}
