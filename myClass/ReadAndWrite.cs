using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myClass
{
    public interface ReadAndWrite
    {
        byte[] Write();
        void Read(byte[] data);
    }
}
