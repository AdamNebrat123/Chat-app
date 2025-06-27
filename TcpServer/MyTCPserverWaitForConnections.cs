using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using Serilog;
using System.Net.Http;
using System.Collections.ObjectModel;

namespace Adam_s_TcpServer
{
    public class MyTCPserverWaitForConnections
    {
        public static void Main()
        {
            CreateTheLogger();
            int numOfClients = 0;
            BlockingCollection<int> exceptionQueue = new BlockingCollection<int>();
            Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue = new Dictionary<int, BlockingCollection<string>>();
            DstOfMsgManager dstOfMsgManager = new DstOfMsgManager();
            NameToClientManager nameToClientManager = new NameToClientManager();
            GroupOfClientManager groupOfClientManager = new GroupOfClientManager(nameToClientManager);


            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            TcpListener server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();
            //////////////////////////
            while (true)
            {
                Log.Information("Waiting for a connection... ");
                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();

                // get the Ip and Port of the client that connected
                Log.Information("Connected! from : " + (IPEndPoint)client.Client.RemoteEndPoint); 
                numOfClients++;
                MyTCPserverConversation conversation = new MyTCPserverConversation(groupOfClientManager, dstOfMsgManager,
                                                                nameToClientManager, client, numOfClients, exceptionQueue);
                conversation.StartReadAndSend("server");
            }
        }
        public static void CreateTheLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
            .WriteTo.File("ServerChatLog.txt", rollingInterval: RollingInterval.Infinite, shared: true)
            .CreateLogger();
        }
        private static void InputHandler(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue)
        {
            Log.Information("Welcome to adams's chat **Server**! you can enter messages (type 'exit' to quit): ");
            while (true)
            {

                // Wait for user input
                string input = Console.ReadLine();
                // Add the input to all of the queues
                foreach (BlockingCollection<string> value in dictionaryOfmessageQueue.Values)
                {
                    value.Add(string.Format("from Server : {0}" , input));
                }

                // Exit the loop if the user types "exit"
                if (input.ToLower() == "exit")
                {
                    break;
                }
            }
        }
        private static void SearchingForException(Dictionary<int, BlockingCollection<string>> dictionaryOfmessageQueue, BlockingCollection<int> ExceptionQueue)
        {
            while (true) 
            {
                
                if(ExceptionQueue.TryTake(out int keyToRemove))
                {
                    dictionaryOfmessageQueue.Remove(keyToRemove);
                }

                
            }
        }

    }
}
