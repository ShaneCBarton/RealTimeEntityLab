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
        public float velocityX;
        public float velocityY;
        public string balloonKey;

        public BalloonData(float x, float y, float vx, float vy)
        {
            xPercent = x;
            yPercent = y;
            velocityX = vx;
            velocityY = vy;
            balloonKey = $"{x:F4},{y:F4}";
        }
    }

    void Start()
    {
        NetworkServerProcessing.SetGameLogic(this);
        networkServer = NetworkServerProcessing.GetNetworkServer();
        randomGenerator = new System.Random();

        StartCoroutine(CheckForClientsAndSpawnBalloons());
        StartCoroutine(UpdateBalloonPositions());
    }

    private IEnumerator UpdateBalloonPositions()
    {
        while (true)
        {
            if (networkServer.idToConnectionLookup.Count > 0)
            {
                List<string> keysToUpdate = new List<string>(activeBalloons.Keys);
                foreach (string key in keysToUpdate)
                {
                    BalloonData balloon = activeBalloons[key];

                    balloon.xPercent += balloon.velocityX * Time.deltaTime;
                    balloon.yPercent += balloon.velocityY * Time.deltaTime;

                    if (balloon.xPercent < 0 || balloon.xPercent > 1) balloon.velocityX *= -1;
                    if (balloon.yPercent < 0 || balloon.yPercent > 1) balloon.velocityY *= -1;

                    balloon.xPercent = Mathf.Clamp01(balloon.xPercent);
                    balloon.yPercent = Mathf.Clamp01(balloon.yPercent);

                    string newKey = $"{balloon.xPercent:F4},{balloon.yPercent:F4}";
                    activeBalloons.Remove(key);
                    balloon.balloonKey = newKey;
                    activeBalloons[newKey] = balloon;

                    string updateMessage = string.Format("{0},{1},{2}",
                        ServerToClientSignifiers.UPDATE_BALLOON_POSITION,
                        key,
                        newKey);

                    foreach (KeyValuePair<int, NetworkConnection> clientConnection in networkServer.idToConnectionLookup)
                    {
                        NetworkServerProcessing.SendMessageToClient(updateMessage, clientConnection.Key, TransportPipeline.ReliableAndInOrder);
                    }
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
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

        float velocityX = ((float)randomGenerator.NextDouble() * 0.4f - 0.2f);
        float velocityY = ((float)randomGenerator.NextDouble() * 0.4f - 0.2f);

        BalloonData newBalloon = new BalloonData(screenPositionXPercent, screenPositionYPercent, velocityX, velocityY);
        activeBalloons[newBalloon.balloonKey] = newBalloon;

        string spawnMessage = string.Format("{0},{1},{2},{3},{4}",
            ServerToClientSignifiers.SPAWN_BALLOON,
            screenPositionXPercent.ToString("F4"),
            screenPositionYPercent.ToString("F4"),
            velocityX.ToString("F4"),
            velocityY.ToString("F4"));

        foreach (KeyValuePair<int, NetworkConnection> clientConnection in networkServer.idToConnectionLookup)
        {
            NetworkServerProcessing.SendMessageToClient(spawnMessage, clientConnection.Key, TransportPipeline.ReliableAndInOrder);
        }

        foreach (KeyValuePair<int, NetworkConnection> clientConnection in networkServer.idToConnectionLookup)
        {
            NetworkServerProcessing.SendBalloonSpawnToClient(
                screenPositionXPercent,
                screenPositionYPercent,
                velocityX,
                velocityY,
                clientConnection.Key);
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
