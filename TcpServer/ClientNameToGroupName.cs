using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class ClientNameToGroupName
    {
        private readonly ConcurrentDictionary<string, string> _senderToGroup
            = new ConcurrentDictionary<string, string>();

        public bool TryAddSenderGroup(string sender, string groupName)
        {
            return _senderToGroup.TryAdd(sender, groupName);
        }

        public string GetGroupBySender(string sender)
        {
            return _senderToGroup.TryGetValue(sender, out var groupName) ? groupName : string.Empty;
        }

        public bool TryRemoveSender(string sender)
        {
            return _senderToGroup.TryRemove(sender, out _);
        }
        public bool TryUpdateGroupForSender(string sender, string newGroupName)
        {
            if (_senderToGroup.TryGetValue(sender, out var currentGroupName))
            {
                return _senderToGroup.TryUpdate(sender, newGroupName, currentGroupName);
            }
            return false;
        }
    }
}
