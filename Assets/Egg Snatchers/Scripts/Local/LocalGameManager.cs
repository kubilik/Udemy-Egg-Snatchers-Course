using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public enum LocalGameState { Menu, Waiting, Scanning, Joining }

public class LocalGameManager : MonoBehaviour
{
    public static LocalGameManager instance;

    private LocalGameState gameState;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
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
}

public interface ILocalGameStateListener
{
    void GameStateChangedCallback(LocalGameState gameState);
}
