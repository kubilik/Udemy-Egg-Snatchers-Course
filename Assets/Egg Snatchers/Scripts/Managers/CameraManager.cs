using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using Unity.Mathematics;
using System;

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
        if (gameState == GameState.Win || gameState == GameState.Lose)
            StartFinalZoom();

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

    private void StartFinalZoom()
    {
        // Start a tween to change the weight of the other player (1 - 0)
        int playerIndex = targetGroup.m_Targets[0].weight == 10 ? 1 : 0;

        LeanTween.value(0, 1, 2).setOnUpdate((value) => UpdateCameraZoom(value, playerIndex));
    }

    private void UpdateCameraZoom(float weight, int playerIndex)
    {
        targetGroup.m_Targets[playerIndex].weight = 1 - weight;

        Camera mainCamera = Camera.main;
        CinemachineVirtualCamera vCam = mainCamera.GetComponent<CinemachineBrain>().
            ActiveVirtualCamera.
            VirtualCameraGameObject.
            GetComponent<CinemachineVirtualCamera>();

        vCam.m_Lens.OrthographicSize = Mathf.Lerp(7, 4, weight);
    }
}
