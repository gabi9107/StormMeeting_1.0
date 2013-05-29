using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDll
{
    public enum CommandType
    {
        Error,
        Message,
        ClientLogin,
        ClientLogOff,
        NameExists,
        ClientsList,
        Immage
    }

    public enum MessageType
    {
        Error,
        Broadcast,
        Private
    }
}
