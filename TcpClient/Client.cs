using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using myClass;

using myClass;
using System.Xml.Linq;

namespace TcpClientApp
{
    public class Client
    {
        private static Connector _connector;
        private static ClientInputHandler _clientInputHandler;
        private static TcpClient _client;
        private static string toWho;
        private static string nickName;

        private static MsgWriter _writer;
        private static MsgReader _reader;


        public static void Main()
        {
            CreateTheLogger();


            String serverIP = "127.0.0.1";
            Int32 port = 13000;
            _connector = new Connector(serverIP, port);
            _client = _connector.Connect();
            if (_client == null)
            {
                Console.WriteLine("could NOT connect to the server");
                return;
            }
            _writer = new MsgWriter(_client);
            _reader = new MsgReader(_client);

            Console.WriteLine("Whats your name?");
            string name = Console.ReadLine();
            //SendName(name);

            _clientInputHandler = ClientInputHandler.CreateInstance(name, _writer);
            _clientInputHandler.StartInputHandler();




        }
        public static void CreateTheLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
            .WriteTo.File("ClientChatLog.txt", rollingInterval: RollingInterval.Infinite, shared: true)
            .CreateLogger();
        }
        
        
    }
}
