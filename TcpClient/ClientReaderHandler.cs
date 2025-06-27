using myClass;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TcpClientApp
{
    public class ClientReaderHandler
    {
        private readonly MsgReader _reader;
        public ClientReaderHandler(TcpClient client)
        {
            _reader = new MsgReader(client);
        }
        public void StartReaderHandler()
        {
            new Thread(() => ReaderHandler()).Start();
        }
        private void ReaderHandler()
        {
            while (true)
            {
                ReadAndWrite readObj = _reader.Read();
                Console.WriteLine(); // go down a line
                Log.Information(readObj.ToString());
            }
        }
    }
}
