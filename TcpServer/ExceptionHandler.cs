using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class ExceptionHandler
    {
        private static ExceptionHandler _instance;
        private BlockingCollection<int> exceptionQueue;
        private Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue;
        private ExceptionHandler(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue)
        {
            this.exceptionQueue = new BlockingCollection<int>();
            this.dictionaryOfmessageQueue = dictionaryOfmessageQueue;

        }
        public static ExceptionHandler CreateInstance(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue)
        {
            if (_instance == null)
            {
                _instance = new ExceptionHandler(dictionaryOfmessageQueue);
            }
            return _instance;
        }
        public BlockingCollection<int> GetExceptionQueue()
        {
            return this.exceptionQueue;
        }
        public void StartExceptionHandler()
        {
            new Thread(SearchingForException).Start();
        }
        public void SearchingForException()
        {
            while (true)
            {
                try
                {
                    int keyToRemove = exceptionQueue.Take(); // blocking method
                    this.dictionaryOfmessageQueue.Remove(keyToRemove);
                }
                catch (InvalidOperationException)
                {
                    break;
                }
            }
        }
    }
}
