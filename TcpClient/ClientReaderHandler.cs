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
        public void StartReaderHandler(string nickname)
        {
            new Thread(() => ReaderHandler(nickname)).Start();
        }
        private void ReaderHandler(string nickname)
        {
            while (true)
            {
                _reader.Read(nickname);
            }
        }
    }
}
