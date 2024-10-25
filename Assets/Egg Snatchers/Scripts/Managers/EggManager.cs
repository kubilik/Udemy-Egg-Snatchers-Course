using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

using Random = UnityEngine.Random;
public class EggManager : NetworkBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private Transform[] spawnPositions;

    [Header(" Actions ")]
    public static Action onEggSpawned;

    private void SpawnEgg()
    {
        Vector3 spawnPosition = spawnPositions[Random.Range(0, spawnPositions.Length)].position;
        GameObject eggInstance = Instantiate(eggPrefab, spawnPosition, Quaternion.identity, transform);

        eggInstance.GetComponent<NetworkObject>().Spawn();

        onEggSpawned?.Invoke();
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (!IsServer)
            return;

        if (gameState != GameState.Game)
            return;

        SpawnEgg();
    }
}
