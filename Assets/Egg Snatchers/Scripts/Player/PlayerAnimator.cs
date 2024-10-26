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

        AttackButton.onClicked += Attack;
    }

    private void OnDestroy()
    {
        playerController.onJumpStarted -= Jump;
        playerController.onFallStarted -= Fall;
        playerController.onLandStarted -= Land;

        AttackButton.onClicked -= Attack;
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

    private void Attack()
    {
        if (!IsOwner)
            return;

        if (playerDetection.IsHoldingEgg)
            return;

        animator.Play("Attack");
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
