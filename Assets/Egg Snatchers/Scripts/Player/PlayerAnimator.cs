using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : NetworkBehaviour
{
    [Header(" Components ")]
    private PlayerController playerController;

    [Header(" Elements ")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();

        playerController.onJumpStarted += Jump;
        playerController.onFallStarted += Fall;
        playerController.onLandStarted += Land;
    }

    private void OnDestroy()
    {
        playerController.onJumpStarted -= Jump;
        playerController.onFallStarted -= Fall;
        playerController.onLandStarted -= Land;

    }

    void Start()
    {

    }


    void Update()
    {
        UpdateBlendTreeRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateBlendTreeRpc()
    {
        animator.SetFloat("xSpeed", playerController.XSpeed);
    }

    private void Jump()
    {
        animator.Play("Jump");
    }

    private void Fall()
    {
        animator.Play("Fall");
    }

    private void Land()
    {
        animator.Play("Land");
    }
}
