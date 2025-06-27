using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class ClientNameToTcpClientDictionary
    {
        private readonly ConcurrentDictionary<string, TcpClient> _nameToClient
            = new ConcurrentDictionary<string, TcpClient>();

        public bool TryAdd(string name, TcpClient client)
        {
            return _nameToClient.TryAdd(name, client);
        }

        public TcpClient GetClientOrNull(string name)
        {
            return _nameToClient.TryGetValue(name, out var client) ? client : null;
        }

        public bool TryRemove(string name)
        {
            return _nameToClient.TryRemove(name, out _);
        }
    }
}
