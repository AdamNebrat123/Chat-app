using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class ConversationsManager
    {
        private static object locker = new object();
        private static ConversationsManager _instance;
        private static int numOfClients = 0;
        private Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue;
        private BlockingCollection<int> exceptionQueue;
        private ConversationsManager(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue, BlockingCollection<int> exceptionQueue)
        {
            this.dictionaryOfmessageQueue = dictionaryOfmessageQueue;
        }

        public static ConversationsManager CreateInstance(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue, BlockingCollection<int> exceptionQueue)
        {
            // pattern to prevent race conidion between threads that are trying to create the class at the first time:
            // double check _instance == null to prevent the race condition
            if (_instance == null)
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new ConversationsManager(dictionaryOfmessageQueue, exceptionQueue);
                    }
                }
            }
            return _instance;
        }
    }
}
