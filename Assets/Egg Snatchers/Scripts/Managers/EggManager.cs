using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EggManager : NetworkBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private Transform[] spawnPositions;


    void Start()
    {
        NetworkManager.OnServerStarted += ServerStartedCallback;
    }

    public override void OnDestroy()
    {
        NetworkManager.OnServerStarted -= ServerStartedCallback;
    }

    private void ServerStartedCallback()
    {
        if (!IsServer)
            return;

        SpawnEgg();
    }

    private void SpawnEgg()
    {
        Vector3 spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
        GameObject eggInstance = Instantiate(eggPrefab, spawnPosition, Quaternion.identity, transform);

        eggInstance.GetComponent<NetworkObject>().Spawn();
    }
}
