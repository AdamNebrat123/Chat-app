using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace myClass
{
    public class SimpleStringMsg : ReadAndWrite, InterfaceHandler, ICreateMsg
    {
        private const int ID = 1;
        private string _msg;
        public SimpleStringMsg()
        {
            
        }
        public SimpleStringMsg(string msg)
        {
            this._msg = msg;
        }
        public byte[] Write()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(ID);
            writer.Write(_msg);
            return ms.ToArray();
        }
        public void Read(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            reader.ReadInt32();
            string msg = reader.ReadString();

            this._msg = msg;
        }

        public void CorrectOperationHandler(string nickname)
        {
            Log.Information(nickname + " Got: " + ToString());
        }
        public override string ToString()
        {
            return _msg;
        }
        public ReadAndWrite CreateMsg()
        {
            Console.Write("msg: ");
            string msg = Console.ReadLine();
            return new SimpleStringMsg(msg);
        }
    }
}
