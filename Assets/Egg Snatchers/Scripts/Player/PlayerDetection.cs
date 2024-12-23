using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

[RequireComponent(typeof(PlayerController), typeof(PlayerRenderer))]
public class PlayerDetection : NetworkBehaviour
{
    [Header(" Components ")]
    private PlayerController playerController;
    private PlayerRenderer playerRenderer;

    [Header(" Elements ")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask trampolineMask;
    [SerializeField] private LayerMask powerupMask;
    [SerializeField] private LayerMask eggMask;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private CapsuleCollider capsuleCollider;

    [Header(" Settings ")]
    private bool canStealEgg;
    public bool IsHoldingEgg { get; private set; }

    [Header(" Actions ")]
    public Action onEggTaken;
    public Action onEggLost;

    void Start()
    {
        canStealEgg = true;
        SetIsHoldingEggRpc(false);

        playerController = GetComponent<PlayerController>();
        playerRenderer = GetComponent<PlayerRenderer>();
    }


    void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.IsGameState())
            return;

        DetectTrampolines();
        DetectPowerups();
        if (IsServer)
            DetectEggs();
    }


    private void DetectTrampolines()
    {
        if (!IsGrounded())
            return;

        if (Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, trampolineMask).Length > 0)
            playerController.Jump();
    }

    private void DetectPowerups()
    {
        if (IsHoldingEgg)
            return;

        Collider[] detectedPowerups = DetectColliders(transform.position, powerupMask, out Collider powerup);

        if (powerup == null)
            return;

        GrabPowerup(powerup.GetComponent<Powerup>());
    }

    private void GrabPowerup(Powerup powerup)
    {
        Powerup.Type powerupType = powerup.PowerupType;

        if (IsServer)
            powerup.Destroy();

        switch (powerupType)
        {
            case Powerup.Type.Speed:
                playerController.SpeedUp();
                break;

            case Powerup.Type.Invisibility:
                playerRenderer.Hide();
                break;
        }
    }

    private void DetectEggs()
    {
        if (!canStealEgg || IsHoldingEgg)
            return;

        Collider[] detectedEggs = DetectColliders(transform.position, eggMask, out Collider egg);

        if (egg == null)
            return;

        if (egg.transform.parent == null)
            GrabEgg(egg);
        else if (egg.transform.parent.TryGetComponent(out PlayerDetection playerDetection))
            StealEggFrom(playerDetection, egg);

        Debug.Log("Egg Detected");
    }

    private void StealEggFrom(PlayerDetection otherPlayer, Collider egg)
    {
        GrabEgg(egg);
        otherPlayer.LoseEgg();
    }

    private void LoseEgg()
    {
        SetIsHoldingEggRpc(false);
        canStealEgg = false;

        StartCoroutine(LoseEggCoroutine());

        onEggLost?.Invoke();
    }

    IEnumerator LoseEggCoroutine()
    {
        yield return new WaitForSecondsRealtime(1);
        canStealEgg = true;
    }

    private void GrabEgg(Collider egg)
    {
        egg.transform.SetParent(transform);
        egg.transform.localPosition = Vector3.up * 3.5f;

        SetIsHoldingEggRpc(true);

        playerRenderer.Appear();

        onEggTaken?.Invoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetIsHoldingEggRpc(bool value)
    {
        IsHoldingEgg = value;
    }

    public bool CanGoThere(Vector3 targetPosition, out Collider firstCollider)
    {
        Collider[] detectedColliders = DetectColliders(targetPosition, groundMask, out firstCollider);
        return detectedColliders.Length <= 0;

        //return Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, groundMask).Length <= 0;
    }

    private Collider[] DetectColliders(Vector3 position, LayerMask mask, out Collider firstCollider)
    {
        Vector3 center = position + capsuleCollider.center;

        float halfHeight = (capsuleCollider.height / 2) - capsuleCollider.radius;
        Vector3 offset = transform.up * halfHeight;
        Vector3 point0 = center + offset;
        Vector3 point1 = center - offset;

        Collider[] colliders = Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, mask);

        if (colliders.Length > 0)
            firstCollider = colliders[0];
        else
            firstCollider = null;

        return colliders;
    }

    public bool IsGrounded()
    {
        return Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, groundMask).Length > 0;
    }

}
