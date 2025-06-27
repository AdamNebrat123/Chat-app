using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using Serilog;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;


using myClass;
using TcpClientApp;
using System.Runtime.Remoting.Messaging;
namespace Adam_s_TcpServer
{
    public class MyTCPserverConversation
    {
        private GroupsOfClientsDictionary _groupsOfClientsDictionary;
        private ClientNameToGroupName _clientNameToGroupNameDictionary;
        private ClientNameToTcpClientDictionary _clientNameToTcpClientDictionary;
        private NetworkStream _stream;
        private readonly BlockingCollection<int> _exceptionQueue;
        private MsgWriter _writer;
        private readonly MsgReader _reader;
        private string _nickName;
        private int _numOfClient;
        private readonly TcpClient _theClientConnectedTo;
        public MyTCPserverConversation(GroupsOfClientsDictionary groupOfClientManager, ClientNameToGroupName dstOfMsgManager,
            ClientNameToTcpClientDictionary nameToClientManager, TcpClient client,
            int numOfClient, BlockingCollection<int> ExceptionQueue)
        {
            this._exceptionQueue = ExceptionQueue;
            this._numOfClient = numOfClient;
            this._groupsOfClientsDictionary = groupOfClientManager;
            this._clientNameToTcpClientDictionary = nameToClientManager;
            this._theClientConnectedTo = client;
            this._clientNameToGroupNameDictionary = dstOfMsgManager;
            this._reader = new MsgReader(_theClientConnectedTo);
            this._writer = new MsgWriter(_theClientConnectedTo);
        }

        private void SendError(string message)
        {
            ReadAndWrite response = new ServerMsg(message);
            _writer.SetClient(_theClientConnectedTo);
            _writer.SendData(response);
            Log.Information("Sent to {0}: {1}", _nickName, response.ToString());
        }

        public bool AddToClientNameToTcpClientDictionary()
        {
            if (!_groupsOfClientsDictionary.ContainsGroup(_nickName) && _clientNameToTcpClientDictionary.TryAdd(_nickName, _theClientConnectedTo))
                return true;
            else
            {
                SendError("The name is already used. pick different name");
                return false;
            }
        }
        public bool TryCreateGroup()
        {
            if (_groupsOfClientsDictionary.TryCreateGroup(_nickName))
                return true;
            else
            {
                SendError("Group name is already used. pick other name");
                return false;
            }
        }
        public void SetTalkToYourself()
        {
            TcpClient clientToTalkTo = _clientNameToTcpClientDictionary.GetClientOrNull(_nickName); // name needs to be a name of a client i 
                                                                                        // want to to talk to, initially i put myself here
            if (clientToTalkTo != null)
            {
                _groupsOfClientsDictionary.TryAddClientToGroup(_nickName, _theClientConnectedTo);
                _clientNameToGroupNameDictionary.TryAddSenderGroup(_nickName, _nickName);
                return;

            }
            else
            {
                // send no such client, pls enter other client name
                SendError("No such client, pls enter other client name");
            }
        }
        public void SwitchDestination(SwitchPerson readObj)
        {
            string dstGroup = readObj.GetToWho();
            if (_groupsOfClientsDictionary.ContainsGroup(dstGroup))
            {
                if (_clientNameToGroupNameDictionary.TryUpdateGroupForSender(_nickName, dstGroup))
                {
                    Log.Information("from {0}: {1}", _nickName, readObj.ToString());
                }
                else
                {
                    Log.Error("Could not switch to this group/client, pls enter other destination");
                    SendError("Could not switch to this group/client, pls enter other destination");
                }
            }
            else
            {
                Log.Warning("No such group/client, pls enter other destination");
                SendError("No such group/client, pls enter other destination");
            }
        }
        private void RegisterClient()
        {
            while (true)
            {
                try
                {
                    ReadAndWrite readObj = _reader.Read(); // read data
                    _nickName = ((SendMyName)readObj).GetNickName();
                    Log.Information("From {0}: {1}", _nickName, readObj.ToString());
                    if (AddToClientNameToTcpClientDictionary())
                    {
                        if(TryCreateGroup())
                        {
                            SetTalkToYourself();
                            
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    SendError("You must enter your name first.");
                }
            }
        }
        public void StartReadAndSend()
        {
            new Thread(() => ReadAndSend()).Start();
        }
        private void ReadAndSend()
        {
            // after the client connection adds *SUCCESSFULLY* himself to the dictionaries,
            // only then we can can messeges from him.
            RegisterClient();

            while (true)
            {
                ReadAndWrite readObj = _reader.Read(); // read data
                if (readObj is SwitchPerson)
                {
                    SwitchDestination( (SwitchPerson)readObj );
                }
                else
                {
                    string groupDst = _clientNameToGroupNameDictionary.GetGroupBySender(_nickName);
                    Log.Information("Origin: {0}, sent to {1}: {2}", _nickName, groupDst, readObj.ToString());
                    ConcurrentBag<TcpClient> clients = _groupsOfClientsDictionary.GetClientsInGroupOrNull(groupDst);
                    foreach (TcpClient client in clients)
                    {
                        if (client != _theClientConnectedTo)
                        {
                            this._writer.SetClient(client);
                            ReadAndWrite header = new ServerMsg("From: " + _nickName + ",");
                            _writer.SendData(header); // send header
                            _writer.SendData(readObj); // send data
                        }

                    }
                }
            }
        }
    }
}
