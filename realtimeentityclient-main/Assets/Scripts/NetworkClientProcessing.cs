using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkClientProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received = " + msg + ", from pipeline = " + pipeline);
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.SPAWN_BALLOON)
        {
            float xPercent = float.Parse(csv[1]);
            float yPercent = float.Parse(csv[2]);
            gameLogic.SpawnNewBalloon(xPercent, yPercent);
        }
        else if (signifier == ServerToClientSignifiers.BALLOON_POPPED)
        {
            float xPercent = float.Parse(csv[1]);
            float yPercent = float.Parse(csv[2]);
            gameLogic.HandleBalloonPopped(xPercent, yPercent);
        }
    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    #endregion

    #region Connection Related Functions and Events
    static public void ConnectionEvent()
    {
        Debug.Log("Client: Successfully connected to server!");
    }

    static public void DisconnectionEvent()
    {
        Debug.Log("Client: Disconnected from server");
    }
    static public bool IsConnectedToServer()
    {
        return networkClient.IsConnected();
    }
    static public void ConnectToServer()
    {
        networkClient.Connect();
    }
    static public void DisconnectFromServer()
    {
        networkClient.Disconnect();
    }

    #endregion

    #region Setup
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }
    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
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

