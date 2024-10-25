using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraManager : NetworkBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private CinemachineTargetGroup targetGroup;

    private List<PlayerController> playerControllers = new List<PlayerController>();

    [Rpc(SendTo.Everyone)]
    private void StorePlayersRpc()
    {
        PlayerController[] playerControllersArray = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        playerControllers = new List<PlayerController>(playerControllersArray);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateCameraTargetGroupRpc()
    {
        foreach (PlayerController playerController in playerControllers)
        {
            float weight = 1;

            if (playerController.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                weight = 10;

            targetGroup.AddMember(playerController.transform, weight, 2);
        }
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        if (!IsServer)
            return;

        if (gameState == GameState.Game)
            Initialize();
    }

    private void Initialize()
    {
        StorePlayersRpc();
        UpdateCameraTargetGroupRpc();
    }
}
