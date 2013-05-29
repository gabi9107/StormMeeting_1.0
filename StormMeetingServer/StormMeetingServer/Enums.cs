using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormMeetingServer
{
    public enum CommandType
    {
        Error,
        Message,
        ClientLogin,
        ClientLogOff,
        NameExists,
        ClientsList,
        Image
    }

    public enum MessageType
    {
        Error,
        Broadcast,
        Private
    }
}
