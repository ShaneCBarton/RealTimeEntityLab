using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Networking.Transport;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private float durationUntilNextBalloon = 2f;
    private NetworkServer networkServer;
    private System.Random randomGenerator;
    private Dictionary<string, BalloonData> activeBalloons = new Dictionary<string, BalloonData>();

    private struct BalloonData
    {
        public float xPercent;
        public float yPercent;
        public string balloonKey;

        public BalloonData(float x, float y)
        {
            xPercent = x;
            yPercent = y;
            balloonKey = $"{x:F4},{y:F4}";
        }
    }

    void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
        networkServer = NetworkServerProcessing.GetNetworkServer();
        randomGenerator = new System.Random();

        StartCoroutine(CheckForClientsAndSpawnBalloons());
    }

    private IEnumerator CheckForClientsAndSpawnBalloons()
    {
        while (true)
        {
            if (networkServer.idToConnectionLookup.Count > 0)
            {
                SpawnBalloonForClients();
            }
            yield return new WaitForSeconds(durationUntilNextBalloon);
        }
    }

    private void SpawnBalloonForClients()
    {
        float screenPositionXPercent = (float)randomGenerator.NextDouble();
        float screenPositionYPercent = (float)randomGenerator.NextDouble();

        BalloonData newBalloon = new BalloonData(screenPositionXPercent, screenPositionYPercent);
        activeBalloons[newBalloon.balloonKey] = newBalloon;

        string spawnMessage = string.Format("{0},{1},{2}",
            ServerToClientSignifiers.SPAWN_BALLOON,
            screenPositionXPercent.ToString("F4"),
            screenPositionYPercent.ToString("F4"));

        foreach (KeyValuePair<int, NetworkConnection> clientConnection in networkServer.idToConnectionLookup)
        {
            NetworkServerProcessing.SendMessageToClient(spawnMessage, clientConnection.Key, TransportPipeline.ReliableAndInOrder);
        }
    }

    public void SendExistingBalloonsToClient(int clientID)
    {
        foreach (BalloonData balloon in activeBalloons.Values)
        {
            string spawnMessage = string.Format("{0},{1},{2}",
                ServerToClientSignifiers.SPAWN_BALLOON,
                balloon.xPercent.ToString("F4"),
                balloon.yPercent.ToString("F4"));

            NetworkServerProcessing.SendMessageToClient(spawnMessage, clientID, TransportPipeline.ReliableAndInOrder);
        }
    }

    public void HandleBalloonPopped(string balloonKey)
    {
        if (activeBalloons.ContainsKey(balloonKey))
        {
            activeBalloons.Remove(balloonKey);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && networkServer.idToConnectionLookup.Count > 0)
        {
            int firstClientID = networkServer.idToConnectionLookup.Keys.First();
            NetworkServerProcessing.SendMessageToClient("2,Hello client's world, sincerely your network server", firstClientID, TransportPipeline.ReliableAndInOrder);
        }
    }

}
