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
        private GroupOfClientManager _groupOfClientManager;
        private DstOfMsgManager _dstOfMsgManager;
        private NameToClientManager _nameToClientManager;
        private NetworkStream _stream;
        private readonly BlockingCollection<int> _exceptionQueue;
        private MsgWriter _writer;
        private readonly MsgReader _reader;
        private string _nickName;
        private int _numOfClient;
        private readonly TcpClient _theClientConnectedTo;
        public MyTCPserverConversation(GroupOfClientManager groupOfClientManager, DstOfMsgManager dstOfMsgManager,
            NameToClientManager nameToClientManager, TcpClient client,
            int numOfClient, BlockingCollection<int> ExceptionQueue)
        {
            this._exceptionQueue = ExceptionQueue;
            this._numOfClient = numOfClient;
            this._groupOfClientManager = groupOfClientManager;
            this._nameToClientManager = nameToClientManager;
            this._theClientConnectedTo = client;
            this._dstOfMsgManager = dstOfMsgManager;
            this._reader = new MsgReader(_theClientConnectedTo);
            this._writer = new MsgWriter(_theClientConnectedTo);
        }

        //// Tries to register a client with the given nickname and TcpClient instance.
        //// Returns true if registration is successful; otherwise, sends an error message and returns false.
        //private bool TryRegisterClient(string nickname, TcpClient client)
        //{
        //    // Check if a group already exists with the same name as the nickname
        //    if (_groupOfClientManager.ContainsGroup(nickname))
        //    {
        //        SendError("The name is already used. Please pick a different name.");
        //        return false;
        //    }

        //    // Try to add the nickname and TcpClient to the name-to-client dictionary
        //    if (!_nameToClientManager.TryAdd(nickname, client))
        //    {
        //        SendError("The name is already used. Please pick a different name.");
        //        return false;
        //    }

        //    // Verify that the client was actually added and can be retrieved
        //    var clientToTalkTo = _nameToClientManager.GetClientOrNull(nickname);
        //    if (clientToTalkTo == null)
        //    {
        //        SendError("No such client, please enter a different name.");
        //        return false;
        //    }

        //    // Add the TcpClient to the group corresponding to the nickname
        //    _groupOfClientManager.TryAddClientToGroup(nickname, client);

        //    // Register the sender's destination group as their own group (for message routing)
        //    _dstOfMsgManager.TryAddSenderGroup(nickname, nickname);

        //    return true;
        //}

        //// Sends an error message back to the connected client
        //private void SendError(string message)
        //{
        //    var response = new ServerMsg(message);
        //    _writer.SetClient(_theClientConnectedTo);
        //    _writer.SendData(response);
        //}

        //// Continuously tries to read a nickname from the client and register them.
        //// Keeps looping until a valid registration happens.
        //private void AddToNameToClientManagerDict(string nickname)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            // Read data from the client; nickname parameter is ignored internally but kept for signature consistency
        //            ReadAndWrite readObj = _reader.Read(nickname);
        //            _nickName = ((SendMyName)readObj).GetNickName();

        //            // Attempt to register the client with the extracted nickname
        //            if (TryRegisterClient(_nickName, _theClientConnectedTo))
        //                return;  // Registration succeeded; exit the loop
        //        }
        //        catch (Exception)
        //        {
        //            // If reading fails (e.g., client didn't send a name), notify client to enter their name first
        //            SendError("You must enter your name first.");
        //        }
        //    }
        //}
        //public bool UnregisterClient(string clientName)
        //{
        //    // 1. Retrieve the TcpClient object for the client name (if exists)
        //    TcpClient client = _nameToClientManager.GetClientOrNull(clientName);
        //    if (client == null)
        //        return false; // Client not registered

        //    // 2. Remove the client from the main name-to-client dictionary
        //    if (!_nameToClientManager.TryRemove(clientName))
        //    {
        //        // Optional: log or handle unexpected failure here
        //    }

        //    // 3. Remove the client from their private group (if it exists)
        //    if (_groupOfClientManager.ContainsGroup(clientName))
        //    {
        //        _groupOfClientManager.TryRemoveClientFromGroup(clientName, clientName); // Remove client by name
        //        _groupOfClientManager.TryRemoveGroup(clientName, out _); // Remove the private group itself
        //    }

        //    // 4. Remove the client from any other groups they might be part of
        //    foreach (string groupName in _groupOfClientManager.GetAllGroupNames())
        //    {
        //        _groupOfClientManager.TryRemoveClientFromGroup(groupName, clientName);
        //    }

        //    // 5. Remove the client’s destination mapping (where their messages are sent)
        //    _dstOfMsgManager.TryRemoveSender(clientName);

        //    return true;
        //}

        private void AddToNameToClientManagerDict(string nickname)
        {
            while (true)
            {
                try
                {
                    ReadAndWrite readObj = _reader.Read(nickname); // read data
                    _nickName = ((SendMyName)readObj).GetNickName();
                    if (_nameToClientManager.TryAdd(_nickName, _theClientConnectedTo))
                    {
                        if (_groupOfClientManager.TryCreateGroup(_nickName))
                        {
                            TcpClient clientToTalkTo = _nameToClientManager.GetClientOrNull(_nickName); // name needs to be a name of a client i 
                                                                                                        // want to to talk to but for now lets make me
                            if (clientToTalkTo != null)
                            {
                                _groupOfClientManager.TryAddClientToGroup(_nickName, _theClientConnectedTo);
                                _dstOfMsgManager.TryAddSenderGroup(_nickName, _nickName);
                                return;

                            }
                            else
                            {
                                // send no such client, pls enter other client name
                                ReadAndWrite response = new ServerMsg("no such client, pls enter other client name");
                                this._writer.SetClient(_theClientConnectedTo);
                                _writer.SendData(response); // send data
                            }
                        }
                        else
                        {
                            ReadAndWrite response = new ServerMsg("Group name is already exist. pick other name");
                            this._writer.SetClient(_theClientConnectedTo);
                            _writer.SendData(response); // send data
                        }
                    }
                    else
                    {
                        ReadAndWrite response = new ServerMsg("the name is already exist. pick other name");
                        this._writer.SetClient(_theClientConnectedTo);
                        _writer.SendData(response); // send data
                    }
                }
                catch (Exception e)
                {
                    ReadAndWrite response = new ServerMsg("You must enter your name first.");
                    this._writer.SetClient(_theClientConnectedTo);
                    _writer.SendData(response); // send data
                }
            }
        }
        public void StartReadAndSend(string nickname)
        {
            new Thread(() => ReadAndSend(nickname)).Start();
        }
        private void ReadAndSend(string nickname)
        {
            // after the client connection adds *SUCCESSFULLY* himself to the dictionaries,
            // only then we can can messeges from him.
            AddToNameToClientManagerDict(nickname);

            while (true)
            {
                ReadAndWrite readObj = _reader.Read(nickname); // read data

                string nameToSendTo = _dstOfMsgManager.GetGroupBySender(_nickName);
                ConcurrentBag<TcpClient> clients = _groupOfClientManager.GetClientsInGroupOrNull(_nickName);
                foreach (TcpClient client in clients)
                {
                    //if (client != _theClientConnectedTo)
                    //{
                        this._writer.SetClient(client);
                        _writer.SendData(readObj); // send data
                    //}

                }
            }
        }
    }
}
