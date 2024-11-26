using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private float durationUntilNextBalloon = 1f;
    private NetworkServer networkServer;
    private System.Random randomGenerator;

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

        string spawnMessage = string.Format("{0},{1},{2}",
            ServerToClientSignifiers.SPAWN_BALLOON,
            screenPositionXPercent.ToString("F4"),
            screenPositionYPercent.ToString("F4"));

        foreach (KeyValuePair<int, NetworkConnection> clientConnection in networkServer.idToConnectionLookup)
        {
            NetworkServerProcessing.SendMessageToClient(
                spawnMessage,
                clientConnection.Key,
                TransportPipeline.ReliableAndInOrder
            );
        }

        Debug.Log($"Spawned balloon at {screenPositionXPercent:F4}, {screenPositionYPercent:F4}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            NetworkServerProcessing.SendMessageToClient("2,Hello client's world, sincerely your network server", 0, TransportPipeline.ReliableAndInOrder);
    }

}
