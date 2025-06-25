using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myClass
{
    public class SendMyName : ReadAndWrite, InterfaceHandler, ICreateMsg
    {
        private const int ID = 6;
        private string _nickname;
        public SendMyName()
        {
            
        }
        public SendMyName(string nickname)
        {
            this._nickname = nickname;
        }
        public void CorrectOperationHandler(string nickname)
        {
            Log.Information(nickname + " sent his name to server");
        }

        public void Read(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            reader.ReadInt32();
            string nickname = reader.ReadString();

            this._nickname = nickname;
        }

        public byte[] Write()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(ID);
            writer.Write(_nickname);
            return ms.ToArray();
        }
        public ReadAndWrite CreateMsg()
        {
            Console.WriteLine("Enter a nickName: ");
            string name = Console.ReadLine();
            return new SendMyName(name);
        }
    }
}
