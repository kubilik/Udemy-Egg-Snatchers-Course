using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;

public enum LocalGameState { Menu, Waiting, Scanning, Joining }

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;

    private LocalGameState gameState;

    private bool isServer;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        IPButton.onClicked += IPButtonClickedCallback;

    }

    private void OnDestroy()
    {
        IPButton.onClicked -= IPButtonClickedCallback;

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedCallback;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedCallback;
        SetGameState(LocalGameState.Menu);
    }

    public void SetGameState(LocalGameState gameState)
    {
        this.gameState = gameState;

        IEnumerable<ILocalGameStateListener> gameStateListeners =
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<ILocalGameStateListener>();

        foreach (ILocalGameStateListener gameStateListener in gameStateListeners)
            gameStateListener.GameStateChangedCallback(gameState);
    }

    public void CreateButtonCallback()
    {
        SetGameState(LocalGameState.Waiting);

        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(NetworkUtilities.GetLocalIPv4(), 7777);

        NetworkManager.Singleton.StartHost();

        isServer = true;
    }

    public void BackFromWaitingCallback()
    {
        SetGameState(LocalGameState.Menu);
        NetworkManager.Singleton.Shutdown();
    }

    public void JoinButtonCallback()
    {
        SetGameState(LocalGameState.Scanning);
    }

    public void BackFromNetworkScan()
    {
        SetGameState(LocalGameState.Menu);
    }

    public void JoinAfterIPSelectedCallback()
    {
        SetGameState(LocalGameState.Joining);
        NetworkManager.Singleton.StartClient();
    }

    private void IPButtonClickedCallback(string ip)
    {
        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(ip, 7777);
    }

    private void ClientConnectedCallback(ulong clientId)
    {
        if (!isServer)
            return;

        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;

        if (playerCount == 2)
            StartGame();
    }

    private void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Multiplayer", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}

public interface ILocalGameStateListener
{
    void GameStateChangedCallback(LocalGameState gameState);
}
