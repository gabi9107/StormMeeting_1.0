using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormMeetingServer
{
    [Serializable]
    class Message
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

        public Message(CommandType cmdType, MessageType msgType, string messageText)
        {
            m_cmdType = cmdType;
            m_msgType = msgType;
            m_messageText = messageText;
        }
    }
}
