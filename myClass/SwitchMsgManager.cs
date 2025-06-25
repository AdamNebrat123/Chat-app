using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myClass
{
    public static class SwitchMsgManager
    {
        private const string speacialCommand = "/switch";
        public static bool CheckIfNeededSwitch<T>(T msg)
        {
            if (msg is string)
                return msg.Equals(speacialCommand);
            return false;
        }
        public static int CreateSwitchID(string msg) 
        {
            while (true)
            {
                try
                {
                    Console.Write("Enter the ID of the msg type you want: ");
                    int id = int.Parse(Console.ReadLine());
                    return id;
                }
                catch(FormatException fe)
                {
                    Console.WriteLine("id is a number");
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("something went wrong");
                    continue;
                }
            }
        }
    }
}
