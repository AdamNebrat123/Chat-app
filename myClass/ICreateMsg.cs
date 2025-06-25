using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myClass
{
    public interface ICreateMsg
    {
        ReadAndWrite CreateMsg(); // creating the msg by creating a specific object that is ready to send ( the messege)
    }
}
