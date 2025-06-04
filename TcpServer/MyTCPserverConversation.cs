using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using Serilog;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;


using myClass;
namespace Adam_s_TcpServer
{
    public class MyTcpListener
    {
        private BlockingCollection<string> messageQueue;
        private BlockingCollection<int> exceptionQueue;

        private NetworkStream stream;
        private int numOfClient;
        public MyTcpListener(BlockingCollection<string> messageQueue, TcpClient client, int numOfClient, BlockingCollection<int> ExceptionQueue)
        {
            this.exceptionQueue = ExceptionQueue;
            this.numOfClient = numOfClient;
            this.messageQueue = messageQueue;

            //StartConversation(client);

            //Object DeSerialization

            while (true)
            {
                NetworkStream stream = client.GetStream();

                byte[] lengthBuffer = new byte[sizeof(int)];

                int offset = 0; // an offset to know where to add the data and stop

                stream.Read(lengthBuffer, 0, sizeof(int));


                int ObjectdataLength = BitConverter.ToInt32(lengthBuffer, 0);
                //create the array for the full objectD
                byte[] fullObjectBytes = new byte[ObjectdataLength];
                while (offset < ObjectdataLength)
                {
                    int numOfBytedRead = stream.Read(fullObjectBytes, offset, ObjectdataLength);
                    if (numOfBytedRead == 0)
                    {
                        Log.Warning("some of the objec's data is missing...");
                        break;
                    }
                    offset += numOfBytedRead;
                }
                byte[] idBuffer = new byte[sizeof(int)];
                for (int i = 0; i < 4; i++)
                {
                    idBuffer[i] = fullObjectBytes[i];
                }
                Console.WriteLine("the buffer: " + string.Join(",", fullObjectBytes));
                int id = BitConverter.ToInt32(idBuffer, 0); // the ID of the class
                Console.WriteLine(id );
                ReadAndWrite myObject = DeserializationHelper.CreateObjectById(id);
                myObject.Read(fullObjectBytes);
                ((InterfaceHandler)myObject).CorrectOperationHandler();


                }
            }
            private void StartConversation( TcpClient client)
            {
            try
            {
                //create the intput thread and the reading thread
                Thread readingThread = new Thread(() => ReadTheIncomingData(client));

                //run the Reading thread

                //readingThread.Start();


                // the message string

                // Loop to send the msg that we got from the input thread while thre is a connection.
                while (true)
                {

                    // Get a stream object for reading and writing
                    stream = client.GetStream();


                    if (messageQueue.TryTake(out string message))
                    {
                        string messageStr = message;

                        byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(messageStr);

                        // Send the msg.
                        stream.Write(messageBytes, 0, messageBytes.Length);
                    }

                }
            }
            catch (ArgumentNullException e)
            {
                Log.Error("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Log.Error("SocketException: {0}", e);
            }
            catch (Exception e)
            {
                
            }

        }

        private void ReadTheIncomingData(TcpClient client)
        {
            try
            {
                while (true)
                {
                    // array to store the response bytes.
                    Byte[] messageBytesRecieved = new Byte[256];

                    // String to store the response ASCII representation.
                    string responseData;
                    int i;
                    NetworkStream ns = client.GetStream();
                    // Loop to receive all the data sent by the client.
                    responseData = String.Empty;
                    while ((i = ns.Read(messageBytesRecieved, 0, messageBytesRecieved.Length)) != 0)
                    {
                        // Read the first batch of the TcpServer response bytes.
                        responseData = System.Text.Encoding.ASCII.GetString(messageBytesRecieved, 0, i);
                        Log.Information(responseData);

                    }

                }
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
                Log.Information("Disconected! from : " + (IPEndPoint)client.Client.RemoteEndPoint); // ip and port
                exceptionQueue.Add(numOfClient);
            }
        }
    }
}
