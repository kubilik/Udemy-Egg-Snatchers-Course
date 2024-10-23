using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PowerupManager : NetworkBehaviour, IGameStateListener
{
    [Header("value")]
    [SerializeField] private Powerup[] powerups;
    [SerializeField] private Transform[] spawnPositions;

    public void GameStateChangedCallback(GameState gameState)
    {
        if (!IsServer)
            return;

        if (gameState == GameState.Game)
            StartSpawingPowerups();
        else
            StopSpawningPowerups();
    }
    private void StartSpawingPowerups()
    {
        StartCoroutine(SpawnPowerupsCoroutine());
    }

    IEnumerator SpawnPowerupsCoroutine()
    {
        yield return new WaitForSecondsRealtime(5);
        SpawnPowerup();

        while (true)
        {
            yield return new WaitForSecondsRealtime(5);
            SpawnPowerup();
        }
    }

    private void SpawnPowerup()
    {
        if (transform.childCount > 0)
            return;

        Powerup randomPowerup = powerups[Random.Range(0, powerups.Length)];
        Transform randomPosition = spawnPositions[Random.Range(0, spawnPositions.Length)];

        Powerup powerupInstance = Instantiate(randomPowerup, randomPosition.position, Quaternion.identity, transform);
        powerupInstance.GetComponent<NetworkObject>().Spawn();
        powerupInstance.transform.SetParent(transform);
    }

    private void StopSpawningPowerups()
    {
        StopAllCoroutines();
    }
}
