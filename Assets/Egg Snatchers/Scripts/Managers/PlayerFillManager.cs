using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerFillManager : NetworkBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    private PlayerFill[] players;
    private bool canUpdatePlayers;

    [Header(" Settings ")]
    [SerializeField] private float fillStep;

    void Update()
    {
        if (!canUpdatePlayers)
            return;

        UpdatePlayersRpc();
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
    private void UpdatePlayersRpc()
    {
        if (players == null || players.Length < 2)
            StorePlayers();

        foreach (PlayerFill playerFill in players)
            playerFill.UpdateFill(fillStep);
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState != GameState.Game)
            return;

        if (!IsServer)
            return;

        StartUpdatingPlayers();
    }

}
