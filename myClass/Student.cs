using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace myClass
{
    [Serializable]
    public class Student : InterfaceHandler, ReadAndWrite, ICreateMsg
    {
        private const int ID = 2;
        private string name;
        private int age;
        
        public Student()
        {

        }
        public Student(string name, int age)
        {
            this.name = name;
            this.age = age;
            
        }

        public string GetName() { return name; }
        public int GetAge() { return age; }

        public override string ToString()
        {
            return string.Format("name : {0}, age : {1}", name, age);
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
            writer.Write(name);
            writer.Write(age);
            return ms.ToArray();
        }
        public void Read(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            reader.ReadInt32();
            string name = reader.ReadString();
            int age = reader.ReadInt32();

            this.name = name;
            this.age = age;
        }

        public ReadAndWrite CreateMsg()
        {
            // if there is an FormatException it will ask again to choose a type of msg
            Console.WriteLine("enter a name: ");
            string name = Console.ReadLine();
            Console.WriteLine("enter an age: ");
            int age = int.Parse(Console.ReadLine());
            return new Student(name, age);
        }
    }
}
