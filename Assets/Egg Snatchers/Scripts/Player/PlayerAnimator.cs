using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using System;

[RequireComponent(typeof(PlayerController), typeof(PlayerDetection))]
public class PlayerAnimator : NetworkBehaviour, IGameStateListener
{
    [Header(" Components ")]
    private PlayerController playerController;
    private PlayerDetection playerDetection;

    [Header(" Elements ")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerDetection = GetComponent<PlayerDetection>();

        playerController.onJumpStarted += Jump;
        playerController.onFallStarted += Fall;
        playerController.onLandStarted += Land;
        playerController.onStun += Stun;

        AttackButton.onClicked += Attack;

        playerDetection.onEggTaken += EggTakenCallback;
        playerDetection.onEggLost += EggLostCallback;
    }

    private void OnDestroy()
    {
        playerController.onJumpStarted -= Jump;
        playerController.onFallStarted -= Fall;
        playerController.onLandStarted -= Land;
        playerController.onStun -= Stun;

        AttackButton.onClicked -= Attack;

        playerDetection.onEggTaken -= EggTakenCallback;
        playerDetection.onEggLost -= EggLostCallback;
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
        if (playerController.IsStun)
            return;

        animator.Play("Jump");
    }

    private void Fall()
    {
        if (playerController.IsStun)
            return;

        animator.Play("Fall");
    }

    private void Land()
    {
        if (playerController.IsStun)
            return;

        animator.Play("Land");
    }

    private void Attack()
    {
        if (playerController.IsStun)
            return;

        if (!IsOwner)
            return;

        if (playerDetection.IsHoldingEgg)
            return;

        animator.Play("Attack");
    }

    private void Stun()
    {
        animator.Play("Stun");
    }

    private void EggTakenCallback()
    {
        animator.SetLayerWeight(1, 1);
    }

    private void EggLostCallback()
    {
        animator.SetLayerWeight(1, 0);
    }

    private void Lose()
    {
        animator.transform.rotation = Quaternion.Euler(0, 180, 0);
        animator.Play("Lose");
    }
    private void Win()
    {
        animator.transform.rotation = Quaternion.Euler(0, 180, 0);
        animator.Play("Win");
    }
    public void GameStateChangedCallback(GameState gameState)
    {
        if (gameState == GameState.Lose)
            Lose();
        else if (gameState == GameState.Win)
            Win();
    }
}
