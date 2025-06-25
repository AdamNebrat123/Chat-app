using myClass;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TcpClientApp
{
    public class ClientInputHandler
    {
        private static object locker = new object();
        private static ClientInputHandler _instance;
        private string nickName;
        private MsgWriter _writer;
        private ClientInputHandler(string nickName, MsgWriter writer)
        {
            this.nickName = nickName;
            _writer = writer;
        }
        public void StartInputHandler()
        {
            new Thread(InputHandler).Start();
        }
        private void InputHandler()
        {
            // NEED TO ADD FUNCTIONALITY FOR DIFFENT STRATEGIES!
                SwitchMsgType();

            while (true)
            {
                ReadAndWrite msgObj = _writer.CreateMsgObj();
                if (msgObj != null)
                {
                    _writer.SendData(msgObj);
                }
                else
                {
                    SwitchMsgType();
                }
            }
        }
        public static ClientInputHandler CreateInstance(string nickName, MsgWriter writer)
        {
            // pattern to prevent race conidion between threads that are trying to create the class at the first time:
            // double check _instance == null to prevent the race condition
            if (_instance == null)
            {
                lock (locker)
                {
                    if (_instance == null)
                    {
                        _instance = new ClientInputHandler(nickName, writer);
                    }
                }
            }
            return _instance;
        }

        private void SwitchMsgType()
        {
            ICreateMsg typeOfMsg = SwitchMsgManager.GetCorrectTypeOfMsg();
            _writer.SetMsgSendStrategy(typeOfMsg);

        }
    }
}
