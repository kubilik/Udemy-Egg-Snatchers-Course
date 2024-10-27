using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    enum PlayerState { Grounded, Air }
    private PlayerState playerState;

    [Header(" Components ")]
    [SerializeField] PlayerDetection playerDetection;

    [Header(" Elements ")]
    [SerializeField] private BoxCollider groundDetector;
    [SerializeField] private LayerMask groundMask;

    [Header(" Settings ")]
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float jumpSpeed;
    private float moveSpeed;
    public float XSpeed { get; private set; }
    private float ySpeed;
    public bool IsStun { get; private set; }

    [Header(" Actions ")]
    public Action onJumpStarted;
    public Action onFallStarted;
    public Action onLandStarted;
    public Action onStun;

    public static Action onSpawned;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        onSpawned?.Invoke();

        JumpButton.onClicked += MiniJump;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        JumpButton.onClicked -= MiniJump;
    }


    void Start()
    {
        playerState = PlayerState.Grounded;
        moveSpeed = maxMoveSpeed;
    }
    void Update()
    {
        if (!IsOwner)
            return;

        MoveVertical();

        if (GameManager.instance == null || !GameManager.instance.IsGameState())
            return;

        MoveHorizontal();
    }

    private void MoveHorizontal()
    {
        if (IsStun)
            return;

        Vector2 moveVector = InputManager.instance.GetMoveVector();

        UpdateXSpeedRpc(Mathf.Abs(moveVector.x));
        //XSpeed = Mathf.Abs(moveVector.x);

        ManageFacing(moveVector.x);

        moveVector.x *= moveSpeed;

        float targetX = transform.position.x + moveVector.x * Time.deltaTime;

        Vector2 targetPosition = transform.position.With(x: targetX);

        if (playerDetection.CanGoThere(targetPosition, out Collider firstCollider))
            transform.position = targetPosition;
    }

    private void ManageFacing(float xSpeed)
    {
        float facing = xSpeed != 0 ? Mathf.Sign(xSpeed) : transform.localScale.x;

        transform.localScale = transform.localScale.With(x: facing);
    }

    private void MoveVertical()
    {
        switch (playerState)
        {
            case PlayerState.Grounded:
                MoveVerticalGrounded();
                break;

            case PlayerState.Air:
                MoveVerticalAir();
                break;
        }
    }

    private void MoveVerticalGrounded()
    {
        if (!playerDetection.IsGrounded())
        {
            StartFalling();
            return;
        }
    }

    private void MoveVerticalAir()
    {
        float targetY = transform.position.y + ySpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position.With(y: targetY);

        // We are falling
        if (!playerDetection.CanGoThere(targetPosition, out Collider firstCollider))
        {
            float minY = firstCollider.ClosestPoint(transform.position).y;
            Physics.Raycast(groundDetector.transform.position, Vector3.down, out RaycastHit hit, 1, groundMask);

            if (hit.collider != null)
                targetPosition.y = minY;
            else
            {
                float maxY = firstCollider.ClosestPoint(transform.position).y;

                //Physics.Raycast(transform.position, Vector3.up, out hit, 3f, groundMask);

                //float maxY = hit.point.y;
                targetPosition.y = maxY - 2.4f;

                ySpeed = 0;
            }
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        transform.position = targetPosition;

        if (playerDetection.IsGrounded())
            Land();
    }

    private void StartFalling()
    {
        playerState = PlayerState.Air;

        onFallStarted?.Invoke();
    }

    private void Land()
    {
        playerState = PlayerState.Grounded;
        ySpeed = 0;

        onLandStarted?.Invoke();
    }

    public void Jump()
    {
        if (IsStun)
            return;

        playerState = PlayerState.Air;
        ySpeed = jumpSpeed;

        HapticsManager.Medium();
        AudioManager.instance.PlayJumpSound();

        onJumpStarted?.Invoke();
    }

    private void MiniJump()
    {
        if (IsStun)
            return;

        if (!IsOwner)
            return;

        if (playerState != PlayerState.Grounded)
            return;

        playerState = PlayerState.Air;
        ySpeed = jumpSpeed / 2;

        HapticsManager.Medium();
        AudioManager.instance.PlayJumpSound();

        onJumpStarted?.Invoke();
    }

    public void GetHit(ulong ownerClientId)
    {
        GetHitRpc(ownerClientId);
    }

    [Rpc(SendTo.Everyone)]
    private void GetHitRpc(ulong ownerClientId)
    {
        if (ownerClientId != OwnerClientId)
            return;

        IsStun = true;

        LeanTween.cancel(gameObject);
        LeanTween.delayedCall(gameObject, 2.5f, () => IsStun = false);

        HapticsManager.Heavy();

        onStun?.Invoke();
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateXSpeedRpc(float xSpeed)
    {
        XSpeed = xSpeed;
    }

    Coroutine resetCoroutine;
    public void SpeedUp()
    {
        if (resetCoroutine != null)
            StopCoroutine(resetCoroutine);

        moveSpeed = maxMoveSpeed * 1.5f;
        resetCoroutine = StartCoroutine(ResetSpeedCoroutine());
    }

    IEnumerator ResetSpeedCoroutine()
    {
        yield return new WaitForSecondsRealtime(10);
        moveSpeed = maxMoveSpeed;
    }
}
