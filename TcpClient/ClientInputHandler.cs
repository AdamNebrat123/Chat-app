using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TcpClientApp
{
    public class ClientInputHandler
    {
        private static object locker = new object();
        private static ClientInputHandler _instance;
        private BlockingCollection<string> msgQueue;
        private string nickName;
        private MsgWriter _sender;
        private ClientInputHandler(BlockingCollection<string> msgQueue, string nickName)
        {
            this.msgQueue = msgQueue;
            this.nickName = nickName;
        }
        public void StartInputHandler()
        {
            new Thread(InputHandler).Start();
        }
        private void InputHandler()
        {
            // NEED TO ADD FUNCTIONALITY FOR DIFFENT STRATEGIES!
            while (true)
            {
                // Wait for user input
                string input = Console.ReadLine();
                Log.Information(string.Format("from {0} : {1}", nickName, input));

                // Add the input to the queue
                msgQueue.Add(input);
                // Exit the loop if the user types "exit"
                if (input.ToLower() == "exit")
                {
                    break;
                }
            }
        }
        public static ClientInputHandler CreateInstance(BlockingCollection<string> msgQueue, string nickName)
        {
            // pattern to prevent race conidion between threads that are trying to create the class at the first time:
            // double check _instance == null to prevent the race condition
            if (_instance == null)
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new ClientInputHandler(msgQueue, nickName);
                    }
                }
            }
            return _instance;
        }
    }
}
