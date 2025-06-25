using myClass;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientApp
{
    public class MsgWriter
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private ReadAndWrite _msgSendStrategy; // what to send (SimpleStringMsg, Image , Student, Family...)
        public MsgWriter(TcpClient client) 
        {
            this._client = client;
            this._stream = client.GetStream();
            this._msgSendStrategy = new SwitchPerson();
        }
        public void SetMsgSendStrategy(ReadAndWrite msgSendStrategy)
        {
            this._msgSendStrategy = msgSendStrategy;
        }
        public void SendData()
        {
            byte[] data = _msgSendStrategy.Write();
            this._stream = _client.GetStream();
            _stream.Write(BitConverter.GetBytes(data.Length), 0, sizeof(int)); //send the number of bytes that need to be transfered
            _stream.Write(data, 0, data.Length); //send the object itself
        }
    }
}
