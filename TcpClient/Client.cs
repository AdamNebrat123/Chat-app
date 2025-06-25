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
        //public static void SendName(string name)
        //{
        //    ICreateMsg sendName = new SendMyName(name);
        //    _writer.SetMsgSendStrategy(sendName);
        //    _writer.SendData();
        //    ReadAndWrite simpleStringMsg = new SimpleStringMsg();
        //    _writer.SetMsgSendStrategy(simpleStringMsg);

        //}
        static void Connect(String server)
        {

            try
            {
                // Create a TcpClient.
                // Note, for this _client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                

                // Prefer a using declaration to ensure the instance is Disposed later. 


                string messageStr = string.Empty;

                //create the intput thread and the reading thread


                //**************************************************Thread inputThread = new Thread(ClientInputHandler);*****************************


                //Thread readingThread = new Thread(ReadTheIncomingData);

                //run the input thread

                //**************************************************inputThread.Start()****************************;

                //run the Reading thread
                //readingThread.Start();



                //Console.WriteLine("Hello! how do you want to be called?");
                //nickName = Console.ReadLine();
                //Log.Information("Welcome to adams's chat **{0}**, you can enter messages (type 'exit' to quit): ", name);
                //ReadAndWrite obj = null;



                //for (int j = 3; j < 4; j++)
                //{
                //    if (j == 1)
                //    {
                //        Console.WriteLine("enter a name: ");
                //        string name = Console.ReadLine();
                //        Console.WriteLine("enter an age: ");
                //        int age = int.Parse(Console.ReadLine());
                //        obj = new Student(name, age);
                //    }
                //    else if (j == 2) 
                //    {
                //        Console.WriteLine("enter father's name: ");
                //        string father = Console.ReadLine();
                //        Console.WriteLine("enter mother's name: ");
                //        string mother = Console.ReadLine();
                //        obj = new Family(father, mother);
                //    }
                //    else
                //    {
                //        bool flag = true;
                //        while (flag)
                //        {
                //            Console.WriteLine("enter a path to an img file: ");
                //            string imgPath = Console.ReadLine();
                //            try
                //            {
                //                // Read the image file as a byte array
                //                byte[] imgBytes = File.ReadAllBytes(imgPath);

                //                string imgName = Path.GetFileName(imgPath);
                //                obj = new Image(imgBytes, imgName);
                //                flag = false;
                //            }
                //            catch (Exception e)
                //            {
                //                Log.Error("An error occurred: " + e.Message);
                //            }
                //        }
                //    }
                //    for (int i = 0; i < 5; i++)
                //    {
                //        Console.WriteLine(i);
                //        Thread.Sleep(1000);
                //    }

                //    stream = client.GetStream();


                //    //serialize 
                //    byte[] data = obj.Write();

                //    stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int)); //send the number of bytes that need to be transfered
                //    stream.Write(data, 0, data.Length); //send the object itself
                //    Console.WriteLine(string.Join(", ", data));
                //    }
                











                //writing a msg to the _stream

                // Loop to send the msg that we got from the input thread while thre is a connection.
                //while (true)
                //{
                //    // Get a _stream object for reading and writing

                //    _stream = _client.GetStream();


                //    if (messageQueue.TryTake(out string message))
                //    {
                //        messageStr = message;
                //        // Translate the passed message into ASCII and store it as a Byte array.
                //        Byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(messageStr);

                //        // Send the message to the connected TcpServer.
                //        _stream.Write(messageBytes, 0, messageBytes.Length);

                //        // Explicit close is not necessary since TcpClient.Dispose() will be
                //        // called automatically.
                //        // _stream.Close();
                //        // _client.Close();
                //    }


                //    Thread.Sleep(100);// זמן המתנה קצר לפני בדיקה מחדש
                //}
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                Log.Information("Disconected from the server");
            }
        }
        public static void CreateTheLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
            .WriteTo.File("ClientChatLog.txt", rollingInterval: RollingInterval.Infinite, shared: true)
            .CreateLogger();
        }
        
        //private static void ReadTheIncomingData()
        //{
        //    try
        //    {
        //        while (true)
        //        {
        //            // array to store the response bytes.
        //            Byte[] messageBytesRecieved = new Byte[256];

        //            // String to store the response ASCII representation.
        //            string responseData;
        //            int i;
        //            NetworkStream ns = _client.GetStream();
        //            // Loop to receive all the data sent by the _client.
        //            responseData = String.Empty;
        //            while ((i = ns.Read(messageBytesRecieved, 0, messageBytesRecieved.Length)) != 0)
        //            {
        //                // Read the first batch of the TcpServer response bytes.
        //                responseData = System.Text.Encoding.ASCII.GetString(messageBytesRecieved, 0, i);
        //                Log.Information(responseData);

        //            }

        //        }
        //    }
        //    catch (ArgumentNullException e)
        //    {
        //        Console.WriteLine("ArgumentNullException: {0}", e);
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("SocketException: {0}", e);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Information("Disconected from the server");
        //    }
        //}
    }
}
