using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myClass
{
    public static class DeserializationHelper
    {
        private static readonly Dictionary<int, Func<ReadAndWrite>> objectByIdDictionary = new Dictionary<int, Func<ReadAndWrite>>
        {
            { 1,  CreateSimpleStringMsg},
            { 2, CreateStudent },
            { 3, CreateFamily },
            { 4, CreateImage },
            { 5, CreateSwitchPerson },
            { 6, CreateSendMyName }
        };

        private static ReadAndWrite CreateSimpleStringMsg()
        {
            ReadAndWrite readAndWrite = new SimpleStringMsg();
            return readAndWrite;
        }
        private static ReadAndWrite CreateStudent()
        {
            ReadAndWrite readAndWrite = new Student();
            return readAndWrite;
        }
        private static ReadAndWrite CreateFamily()
        {
            ReadAndWrite readAndWrite = new Family();
            return readAndWrite;
        }
        private static ReadAndWrite CreateImage()
        {
            ReadAndWrite readAndWrite = new Image();
            return readAndWrite;
        }
        private static ReadAndWrite CreateSwitchPerson()
        {
            ReadAndWrite readAndWrite = new SwitchPerson();
            return readAndWrite;
        }
        private static ReadAndWrite CreateSendMyName()
        {
            ReadAndWrite readAndWrite = new SendMyName();
            return readAndWrite;
        }

        public static ReadAndWrite CreateObjectById(int id)
        {
            return objectByIdDictionary[id]();
        }
    }
}
