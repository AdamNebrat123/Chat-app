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

        private void SendResponse(string message, TcpClient dstClient)
        {
            ReadAndWrite response = new ServerMsg(message);
            _writer.SetClient(dstClient);
            _writer.SendData(response);
            Log.Information("Sent to {0}: {1}", _nickName, response.ToString());
        }
        public bool AddToClientNameToTcpClientDictionary()
        {
            if (_clientNameToTcpClientDictionary.TryAdd(_nickName, _theClientConnectedTo))
                return true;
            else
            {
                SendResponse(_nickName + " is already used. pick different name", _theClientConnectedTo);
                return false;
            }
        }
        public bool TryCreateGroup(string groupName)
        {
            if (_groupsOfClientsDictionary.TryCreateGroup(groupName))
                return true;
            else
            {
                SendResponse(groupName + " - Group/Clinet name is already used. pick other name", _theClientConnectedTo);
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
                SendResponse(_nickName + " - No such client, pls enter other client name", _theClientConnectedTo);
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
                    Log.Warning(dstGroup + " - Could not switch to this group/client, pls enter other destination");
                    SendResponse(dstGroup + " - Could not switch to this group/client, pls enter other destination", _theClientConnectedTo);
                }
            }
            else
            {
                Log.Warning(dstGroup + " - No such group/client, pls enter other destination");
                SendResponse(dstGroup + " - No such group/client, pls enter other destination", _theClientConnectedTo);
            }
        }
        private void RegisterClient()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        ReadAndWrite readObj = _reader.Read(); // read data
                        _nickName = ((SendMyName)readObj).GetNickName();
                        Log.Information("From {0}: {1}", _nickName, readObj.ToString());
                        if (TryCreateGroup(_nickName))
                        {
                            if (AddToClientNameToTcpClientDictionary())
                            {
                                SetTalkToYourself();

                                return;
                            }
                        }
                        else
                        {
                            _theClientConnectedTo?.Close();
                            return;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        SendResponse("You must enter your name first.", _theClientConnectedTo);
                    }
                }
            }
            catch (IOException ex)
            {

            }
            catch (SocketException ex)
            {

            }
            catch (ObjectDisposedException ex)
            {

            }
            catch (Exception ex)
            {

            }
        }
        private bool IsClientInGroup(string groupName)
        {
            if (_groupsOfClientsDictionary.TryRemoveClientFromGroup(groupName, _theClientConnectedTo))
            {
                return true;
            }
            return false;
        }
        private void RemoveClientFromEveryGroup()
        {
            foreach (string groupName in _groupsOfClientsDictionary.GetAllGroupNames())
            {
                if (IsClientInGroup(groupName))
                {
                    ConcurrentBag<TcpClient> clients = _groupsOfClientsDictionary.GetClientsInGroupOrNull(groupName);
                    if (clients != null)
                    {
                        foreach (TcpClient client in clients)
                        {
                            if (client != _theClientConnectedTo)
                            {
                                this._writer.SetClient(client);
                                string response = string.Format("===============From: *SERVER*      {0} was REMOVED from the group: {1}===============", _nickName, groupName);
                                SendResponse(response, client);
                            }
                        }
                    }
                }
            }
        }
        private void RemoveClientFromClientNameToTcpClientDictionary()
        {
            if (_clientNameToTcpClientDictionary.TryRemove(_nickName))
                Log.Information("Successfully removed {0} from ClientNameToTcpClientDictionary." , _nickName);
            else
                Log.Error("{0} IS NOT IN ClientNameToTcpClientDictionary?!?!?!" , _nickName);

        }
        public void UnregisterClient()
        {
            try
            {
                RemoveClientFromEveryGroup();
                RemoveClientFromClientNameToTcpClientDictionary();
            }
            catch (Exception e)
            {
            }
        }
        public bool AddClientToGroup(string groupName, TcpClient client)
        {
            return _groupsOfClientsDictionary.TryAddClientToGroup(groupName, client);
        }
        public void CreateNewGroupWithParticipants(string groupName, List<string> ExpectedParticipants)
        {
            List<string> addedParticipants = new List<string>();
            List<string> notAddedParticipants = new List<string>();
            if (TryCreateGroup(groupName))
            {
                foreach (string participant in ExpectedParticipants)
                {
                    TcpClient client = _clientNameToTcpClientDictionary.GetClientOrNull(participant);
                    if (client == null || !AddClientToGroup(groupName, client))
                    {
                        notAddedParticipants.Add(participant);
                    }
                    else
                    {
                        addedParticipants.Add(participant);
                        SendResponse(string.Format("From: *SERVER*      {0} Added you to the group: {1}" , _nickName, groupName), client);
                    }
                }
                if(addedParticipants.Count == 0)
                {
                    _groupsOfClientsDictionary.TryRemoveGroup(groupName, out var _);
                }
                else
                {
                    string response = "Created: \n";
                    response += "Group name: " + groupName + "\n";
                    response += "participants: " + string.Join(",", addedParticipants) + "\n";
                    response += "Could NOT add: " + string.Join(",", notAddedParticipants) + "\n";
                    SendResponse(response, _theClientConnectedTo);
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
            try
            {
                while (true)
                {
                    ReadAndWrite readObj = _reader.Read(); // read data
                    if (readObj is SwitchPerson)
                    {
                        SwitchDestination((SwitchPerson)readObj);
                    }
                    else if (readObj is CreateGroup)
                    {
                        string groupName = ((CreateGroup)readObj).GetGroupName();
                        List<string> expectedParticipants = ((CreateGroup)readObj).GetParticipants();
                        CreateNewGroupWithParticipants(groupName, expectedParticipants);
                    }
                    else
                    {
                        string groupDst = _clientNameToGroupNameDictionary.GetGroupBySender(_nickName);
                        Log.Information("Origin: {0}, sent to {1}: {2}", _nickName, groupDst, readObj.ToString());
                        ConcurrentBag<TcpClient> clients = _groupsOfClientsDictionary.GetClientsInGroupOrNull(groupDst);
                        if (clients != null)
                        {
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
                        else
                            SendResponse(groupDst + " - No such group/client, pls enter other destination", _theClientConnectedTo);
                    }
                }
            }
            catch (IOException ex)
            {

            }
            catch (SocketException ex)
            {
                
            }
            catch (ObjectDisposedException ex)
            {
                
            }
            catch (Exception ex)
            {
                
            }
            finally
            {

                //=================
                UnregisterClient();
                //=================


                try
                {
                    Log.Information("Closing client connection for: {0}", _nickName);
                    _theClientConnectedTo?.Close();
                }
                catch (Exception closeEx)
                {
                    Log.Error("Error while closing socket: {0}", closeEx.Message);
                }
            }
        }
    }
}
