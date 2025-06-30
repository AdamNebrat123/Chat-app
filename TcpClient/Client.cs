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

using System.Xml.Linq;

namespace TcpClientApp
{
    public class Client
    {
        private static Connector _connector;
        private static ClientInputHandler _clientInputHandler;
        private static ClientReaderHandler _clientReaderHandler;
        private static TcpClient _client;
        private static string toWho;
        private static string nickName;

        private static MsgWriter _writer;
        private static MsgReader _reader;


        public static void Main()
        {
            CreateTheLogger();
            Opening();

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

            _clientInputHandler = ClientInputHandler.CreateInstance(_writer);
            _clientInputHandler.StartInputHandler();
            _clientReaderHandler = new ClientReaderHandler(_client);
            _clientReaderHandler.StartReaderHandler();


        }
        
        public static void CreateTheLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
            .WriteTo.File("ClientChatLog.txt", rollingInterval: RollingInterval.Infinite, shared: true)
            .CreateLogger();
        }

        public static void Opening()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("WELCOME TO ADAM'S CHAT!");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========== MENU ==========");
            Console.ResetColor();

            var options = new (int id, string title, string description)[]
            {
        (1, "Send simple string message", "Allows you to enter a short text message to send."),
        (2, "Create Student message", "Prompts for a student's name and age, and sends it."),
        (3, "Create Family message", "Asks for father's and mother's names, and sends family data."),
        (4, "Send an Image", "Lets you choose an image file path and sends the image."),
        (5, "Switch person (change recipient)", "Change the nickname of the user you want to talk to."),
        (6, "Send my name", "Send your nickname to identify yourself."),
        (7, "Create a Group", "Create a new group by entering a name and list of participants."),
            };

            foreach (var (id, title, description) in options)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{id} - ");
                Console.ResetColor();
                Console.WriteLine(title);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"    > {description}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==========================");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("to change the type of the message type exactly: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("/switch");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
