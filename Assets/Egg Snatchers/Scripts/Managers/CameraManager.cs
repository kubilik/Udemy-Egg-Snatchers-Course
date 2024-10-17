using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;

public class CameraManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private CinemachineTargetGroup targetGroup;
    private bool configured;

    private List<PlayerController> playerControllers = new List<PlayerController>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (configured)
            return;

        if (playerControllers.Count < 2)
        {
            StorePlayers();
            return;
        }

        UpdateCameraTargetGroup();
    }

    private void StorePlayers()
    {
        PlayerController[] playerControllersArray = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

        if (playerControllersArray.Length < 2)
            return;

        playerControllers = new List<PlayerController>(playerControllersArray);
    }

    private void UpdateCameraTargetGroup()
    {
        configured = true;

        foreach (PlayerController playerController in playerControllers)
        {
            float weight = 1;

            if (playerController.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                weight = 10;

            targetGroup.AddMember(playerController.transform, weight, 2);
        }
    }
}
