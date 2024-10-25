using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPlacer : NetworkBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private Transform[] spawnPositions;
    List<Transform> potentialPositions;

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState != GameState.Game)
            return;

        if (!IsServer)
            return;

        potentialPositions = new List<Transform>(spawnPositions);

        foreach (KeyValuePair<ulong, NetworkClient> kvp in NetworkManager.Singleton.ConnectedClients)
            PlacePlayer(kvp.Key);
    }
    private void PlacePlayer(ulong clientId)
    {
        int positionIndex = Random.Range(0, potentialPositions.Count);
        Vector3 spawnPosition = potentialPositions[positionIndex].position;
        potentialPositions.RemoveAt(positionIndex);

        PlacePlayerRpc(spawnPosition, clientId);
    }

    [Rpc(SendTo.Everyone)]
    private void PlacePlayerRpc(Vector3 spawnPosition, ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = spawnPosition;
    }
}
