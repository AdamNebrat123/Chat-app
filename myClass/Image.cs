using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace myClass
{
    [Serializable]
    public class Image : InterfaceHandler, ReadAndWrite, ICreateMsg
    {
        private const int ID = 4;
        private byte[] imgBytes;
        private string name;
        public Image()
        {

        }
        public Image(byte[] imgBytes, string name)
        {
            this.imgBytes = imgBytes;
            this.name = name;
        }

        public byte[] GetBytes() 
        {
            return imgBytes;
        }
        public string GetName()
        {
            return name;
        }
        public void CorrectOperationHandler(string nickname)
        {
            try
            {
                string pathToWriteTo = @"D:\temp\" + name;
                // Write the byte array to the file, effectively creating the image
                File.WriteAllBytes(pathToWriteTo, imgBytes);
                Log.Information("The image {0} successfully saved to: {1}" , name, pathToWriteTo);
                Log.Information(nickname + " Got the img: " + this.name);
            }
            catch (Exception e)
            {
                Log.Error("Error: " + e.Message);
                Log.Information(nickname + "did NOT Got the img: " + this.name);
            }
        }
        public byte[] Write()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            writer.Write(ID);
            writer.Write(imgBytes.Length);
            writer.Write(imgBytes);
            writer.Write(name);
            return ms.ToArray();
        }
        public void Read(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms);
            reader.ReadInt32();
            int imgLength = reader.ReadInt32();
            byte[] dataBytes = reader.ReadBytes(imgLength);
            string name = reader.ReadString();

            this.imgBytes = dataBytes;
            this.name = name;
        }
        public ReadAndWrite CreateMsg()
        {
            while(true) 
            {
                Console.WriteLine("enter a path to an img file: ");
                string imgPath = Console.ReadLine();
                try
                {
                    // Read the image file as a byte array
                    byte[] imgBytes = File.ReadAllBytes(imgPath);

                    string imgName = Path.GetFileName(imgPath);
                    return new Image(imgBytes, imgName);
                }
                catch (Exception e)
                {
                    Log.Error("An error occurred: " + e.Message);
                }
            }
        }
    }
}
