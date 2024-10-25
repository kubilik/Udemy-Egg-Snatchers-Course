using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

[RequireComponent(typeof(NetworkObject))]
public class PlayerFillManager : NetworkBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    private PlayerFill[] players;
    private bool canUpdatePlayers;

    [Header(" Settings ")]
    [SerializeField] private float fillStep;

    [Header(" Actions ")]
    public static Action<ulong> onPlayerEmpty;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        EggManager.onEggSpawned += EggSpawnedCallback;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        EggManager.onEggSpawned -= EggSpawnedCallback;
    }


    private void EggSpawnedCallback()
    {
        if (!IsServer)
            return;

        StartUpdatingPlayers();
    }
    void Update()
    {
        if (!canUpdatePlayers)
            return;

        UpdatePlayersRpc(fillStep);
    }

    private void StartUpdatingPlayers()
    {
        canUpdatePlayers = true;
    }

    private void StorePlayers()
    {
        players = FindObjectsByType<PlayerFill>(FindObjectsSortMode.None);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdatePlayersRpc(float fillStep)
    {
        if (players == null || players.Length < 2)
            StorePlayers();

        foreach (PlayerFill playerFill in players)
            if (playerFill.UpdateFill(fillStep))
                PlayerIsEmpty(playerFill);
    }

    private void PlayerIsEmpty(PlayerFill player)
    {
        canUpdatePlayers = false;

        if (!IsServer)
            return;

        onPlayerEmpty?.Invoke(player.GetComponent<NetworkObject>().OwnerClientId);
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState != GameState.Game)
        {
            canUpdatePlayers = false;
            players = null;
        }
    }

}
