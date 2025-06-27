using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class GroupOfClientManager
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<TcpClient>> _groups
            = new ConcurrentDictionary<string, ConcurrentBag<TcpClient>>();
        private NameToClientManager _nameToClientManager;

        public GroupOfClientManager(NameToClientManager nameToClientManager)
        {
            _nameToClientManager = nameToClientManager;
        }

        public bool TryCreateGroup(string groupName)
        {
            return _groups.TryAdd(groupName, new ConcurrentBag<TcpClient>());
        }

        public ConcurrentBag<TcpClient> GetClientsInGroupOrNull(string groupName)
        {
            return _groups.TryGetValue(groupName, out var clients) ? clients : null;
        }

        public bool TryRemoveGroup(string groupName, out ConcurrentBag<TcpClient> clients)
        {
            return _groups.TryRemove(groupName, out clients);
        }
        public bool TryAddClientToGroup(string groupName, TcpClient client)
        {
            if (_groups.TryGetValue(groupName, out var clients))
            {
                clients.Add(client);
                return true;
            }
            return false;
        }
        public bool ContainsGroup(string groupName)
        {
            return _groups.ContainsKey(groupName);
        }
        // Check if a group exists


        // Remove a client from a group by client name
        public bool TryRemoveClientFromGroup(string groupName, string clientName)
        {
            if (!_groups.TryGetValue(groupName, out var clients))
                return false;

            // Get the TcpClient object from the name-to-client manager
            TcpClient targetClient = _nameToClientManager.GetClientOrNull(clientName);
            if (targetClient == null)
                return false;

            // ConcurrentBag does not support removal, so create a new bag without the client
            var newBag = new ConcurrentBag<TcpClient>(clients.Where(c => c != targetClient));
            _groups[groupName] = newBag;
            return true;
        }

        // Get all group names
        public IEnumerable<string> GetAllGroupNames()
        {
            return _groups.Keys;
        }
    }
}
