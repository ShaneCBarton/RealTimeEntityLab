using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkServerProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        //if (signifier == ClientToServerSignifiers.asd)
        {

        }
        // else if (signifier == ClientToServerSignifiers.asd)
        // {

        // }

        //gameLogic.DoSomething();
    }
    static public void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log($"Server: New client connected with ID {clientConnectionID}");
        Debug.Log($"Total connected clients: {networkServer.idToConnectionLookup.Count}");
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log($"Server: Client {clientConnectionID} disconnected");
        Debug.Log($"Remaining connected clients: {networkServer.idToConnectionLookup.Count}");
    }

    #endregion

    #region Setup
    static NetworkServer networkServer;
    static GameLogic gameLogic;

    static public void SetNetworkServer(NetworkServer NetworkServer)
    {
        networkServer = NetworkServer;
    }
    static public NetworkServer GetNetworkServer()
    {
        return networkServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }


    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int BALLOON_POPPED = 1;
}

static public class ServerToClientSignifiers
{
    public const int SPAWN_BALLOON = 1;
    public const int BALLOON_POPPED = 2;
}

#endregion

