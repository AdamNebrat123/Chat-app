using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class GroupsOfClientsDictionary
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<TcpClient>> _groups
            = new ConcurrentDictionary<string, ConcurrentBag<TcpClient>>();

        public GroupsOfClientsDictionary()
        {
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
                // Inefficient but functional...  check manually if client already exists
                bool alreadyExists = clients.Any(existingClient => existingClient == client);

                if (!alreadyExists)
                {
                    clients.Add(client);
                }

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
        public bool TryRemoveClientFromGroup(string groupName, TcpClient targetClient)
        {
            if (!_groups.TryGetValue(groupName, out var clients))
                return false;

            // if the client dont in the group, return false
            bool clientExists = clients.Contains(targetClient);
            if (!clientExists)
                return false;
            // ConcurrentBag does not support removal, so create a new bag without the client
            var newBag = new ConcurrentBag<TcpClient>(clients.Where(c => c != targetClient));

            // if the group is empty i will delete the group
            bool isEmpty = !newBag.Any();
            if (isEmpty)
            {
                TryRemoveGroup(groupName, out clients);
            }
            else
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
