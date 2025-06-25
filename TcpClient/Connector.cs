using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientApp
{
    public class Connector
    {
        private readonly String _serverIP;
        private readonly int _port;
        private TcpClient client;
        public Connector(String serverIP, int port)
        {
            this._serverIP = serverIP;
            this._port = port;
        }

        public TcpClient Connect()
        {
            try
            {
                client = new TcpClient(_serverIP, _port);
                return client;
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
            return null;
        }
    }
}
