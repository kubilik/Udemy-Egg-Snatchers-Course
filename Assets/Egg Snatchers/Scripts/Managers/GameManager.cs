using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public enum GameState { Waiting, Game, Win, Lose }

[RequireComponent(typeof(NetworkObject))]
public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    private GameState gameState;

    [Header(" Elements ")]
    [SerializeField] private NetworkObject playerPrefab;

    [Header(" Settings ")]
    private int playerSpawnedCount;

    private void Awake()
    {
        gameState = GameState.Waiting;

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        PlayerController.onSpawned += PlayerSpawnedCallback;
    }

    private void PlayerSpawnedCallback()
    {
        if (!IsServer)
            return;

        Debug.Log("Player spawned!");
        playerSpawnedCount++;

        if (playerSpawnedCount == 2)
            Invoke("StartGameRpc", Time.deltaTime * 3);
    }

    private void Start()
    {
        PlayerFillManager.onPlayerEmpty += PlayerEmptyCallback;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
            return;

        Invoke("Initialize", Time.deltaTime * 3);
    }

    private void Initialize()
    {
        foreach (KeyValuePair<ulong, NetworkClient> kvp in NetworkManager.Singleton.ConnectedClients)
        {
            NetworkObject player = Instantiate(playerPrefab);
            player.SpawnAsPlayerObject(kvp.Key);
        }

        //StartGameRpc();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        PlayerFillManager.onPlayerEmpty -= PlayerEmptyCallback;
        PlayerController.onSpawned -= PlayerSpawnedCallback;

    }



    public void SetGameState(GameState gameState)
    {
        this.gameState = gameState;

        IEnumerable<IGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener gameStateListener in gameStateListeners)
            gameStateListener.GameStateChangedCallback(gameState);

        Debug.Log("New Game State : " + gameState);
    }

    [Rpc(SendTo.Everyone)]
    private void StartGameRpc()
    {
        SetGameState(GameState.Game);
    }
    private void PlayerEmptyCallback(ulong losingPlayerId)
    {
        if (gameState != GameState.Game)
            return;

        GameEndedRpc(losingPlayerId);
    }

    [Rpc(SendTo.Everyone)]
    private void GameEndedRpc(ulong losingPlayerId)
    {
        ulong localPlayerId = NetworkManager.SpawnManager.GetLocalPlayerObject().OwnerClientId;

        if (localPlayerId == losingPlayerId)
            SetGameState(GameState.Lose);
        else
            SetGameState(GameState.Win);
    }

    public void ContinueButtonCallback()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
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