using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public interface IConversationService
    {
        void CreateMyTCPserverConversationInstance(TcpClient client);
        void AddConversationQueue();
    }
}
