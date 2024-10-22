using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public enum GameState { Waiting, Game, Win, Lose }

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    private GameState gameState;


    private void Awake()
    {
        gameState = GameState.Waiting;

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        NetworkManager.OnClientConnectedCallback += ClientConnectedCallback;
    }

    private void ClientConnectedCallback(ulong obj)
    {
        if (!IsServer)
            return;

        if (NetworkManager.Singleton.ConnectedClients.Count < 2)
            return;

        StartGameRpc();
    }

    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;

        IEnumerable<IGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener gameStateListener in gameStateListeners)
            gameStateListener.GameStateChangedCallback(gameState);
    }

    [Rpc(SendTo.Everyone)]
    private void StartGameRpc()
    {
        SetGameState(GameState.Game);
    }

    public bool IsGameState()
    {
        return gameState == GameState.Game;
    }
}

public interface IGameStateListener
{
    void GameStateChangedCallback(GameState gameState);
}