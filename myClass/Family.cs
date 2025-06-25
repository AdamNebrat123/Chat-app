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
    [Serializable]
    public class Family : InterfaceHandler, ReadAndWrite, ICreateMsg
    {
        private const int ID = 3;
        private string father;
        private string mother;
        public Family()
        {

        }
        public Family(string father, string mother)
        {
            this.father = father;
            this.mother = mother;
        }
        public override string ToString()
        {
            return string.Format("father name: {0}, mother name: {1}", father, mother);
        }
        public void CorrectOperationHandler(string nickname)
        {
            Log.Information(nickname + " Got: " + ToString());
        }
        public byte[] Write()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(ID);
            writer.Write(father);
            writer.Write(mother);
            return ms.ToArray();
        }
        public void Read(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            reader.ReadInt32();
            string father = reader.ReadString();
            string mother = reader.ReadString();

            this.father = father;
            this.mother = mother;
        }
        public ReadAndWrite CreateMsg()
        {
            Console.WriteLine("enter father's name: ");
            string father = Console.ReadLine();
            Console.WriteLine("enter mother's name: ");
            string mother = Console.ReadLine();
            return new Family(father, mother);
        }
    }
    
}
