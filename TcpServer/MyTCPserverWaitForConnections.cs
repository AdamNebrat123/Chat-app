﻿using System;
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
            ClientNameToGroupName dstOfMsgManager = new ClientNameToGroupName();
            ClientNameToTcpClientDictionary nameToClientManager = new ClientNameToTcpClientDictionary();
            GroupsOfClientsDictionary groupOfClientManager = new GroupsOfClientsDictionary();


            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            TcpListener server = new TcpListener(localAddr, port);

            server.Start();
            while (true)
            {
                Log.Information("Waiting for a connection... ");
                // Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();

                // get the Ip and Port of the client that connected
                Log.Information("Connected! from : " + (IPEndPoint)client.Client.RemoteEndPoint); 
                numOfClients++;
                MyTCPserverConversation conversation = new MyTCPserverConversation(groupOfClientManager, dstOfMsgManager,
                                                                nameToClientManager, client, numOfClients, exceptionQueue);
                conversation.StartReadAndSend();
            }
        }
        public static void CreateTheLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
            .WriteTo.File("ServerChatLog.txt", rollingInterval: RollingInterval.Infinite, shared: true)
            .CreateLogger();
        }
    }
}
