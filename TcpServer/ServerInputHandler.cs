using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adam_s_TcpServer
{
    public class ServerInputHandler
    {
        
        private Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue;

        public ServerInputHandler(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue)
        {
            this.dictionaryOfmessageQueue = dictionaryOfmessageQueue; // A REFERENCE!
            new Thread(InputHandler).Start();
        }
        public void InputHandler()
        {
            Log.Information("Welcome to adams's chat **Server**! you can enter messages (type 'exit' to quit): ");
            while (true)
            {

                // Wait for user input
                string input = Console.ReadLine();
                // Add the input to all of the queues
                foreach (BlockingCollection<string> value in dictionaryOfmessageQueue.Values)
                {
                    value.Add(string.Format("from Server : {0}", input));
                }

                // Exit the loop if the user types "exit"
                if (input.ToLower() == "exit")
                {
                    break;
                }
            }
        }
    }
}
