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
        gameState = LocalGameState.Menu;

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
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
}

public interface ILocalGameStateListener
{
    void GameStateChangedCallback(LocalGameState gameState);
}
