using myClass;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TcpClientApp
{
    public class MsgReader
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private int _idOfReadOperation;
        private byte[] _fullObjectBytes;
        private ReadAndWrite readObj;

        public MsgReader(TcpClient client)
        {
            this._client = client;
            this._stream = client.GetStream();
        }
        public void ReadRawData()
        {
            _stream = _client.GetStream();

            byte[] lengthBuffer = new byte[sizeof(int)];

            int offset = 0; // an offset to know where to add the data and stop

            _stream.Read(lengthBuffer, 0, sizeof(int));


            int ObjectdataLength = BitConverter.ToInt32(lengthBuffer, 0);
            //create the array for the full objectD
            _fullObjectBytes = new byte[ObjectdataLength];
            while (offset < ObjectdataLength)
            {
                int numOfBytedRead = _stream.Read(_fullObjectBytes, offset, ObjectdataLength);
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
                idBuffer[i] = _fullObjectBytes[i];
            }
            _idOfReadOperation = BitConverter.ToInt32(idBuffer, 0); // the ID of the class
            
        }
        public void ReadAndDeserialize(string nickname)
        {
            readObj = DeserializationHelper.CreateObjectById(_idOfReadOperation);
            readObj.Read(_fullObjectBytes);
            ((InterfaceHandler)readObj).CorrectOperationHandler(nickname);
        }
    }
}
