using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPlacer : NetworkBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform[] spawnPositions;
    List<Transform> potentialPositions;

    void Start()
    {
        potentialPositions = new List<Transform>(spawnPositions);

        NetworkManager.OnClientConnectedCallback += ClientConnectedCallback;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.OnClientConnectedCallback -= ClientConnectedCallback;
    }

    private void ClientConnectedCallback(ulong clientId)
    {
        if (!IsServer)
            return;

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
