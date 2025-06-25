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
    public class SwitchPerson : ReadAndWrite, InterfaceHandler, ICreateMsg
    {
        private const int ID = 5;
        private string _toWho; // nickname
        public SwitchPerson()
        {
            
        }
        public SwitchPerson(string toWho)
        {
            this._toWho = toWho;
        }

        public void CorrectOperationHandler(string nickname)
        {
            Log.Information(nickname + " tried to switch chat to " + _toWho);
        }

        public void Read(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            reader.ReadInt32();
            string toWho = reader.ReadString();

            this._toWho = toWho;
        }

        public byte[] Write()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(ID);
            writer.Write(_toWho);
            return ms.ToArray();
        }
        public ReadAndWrite CreateMsg()
        {
            Console.Write("who you want to talk to: ");
            string nickname = Console.ReadLine();
            return new SwitchPerson(nickname);
        }
    }
}
