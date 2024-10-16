using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState { Grounded, Air }
    private PlayerState playerState;

    [Header(" Components ")]
    [SerializeField] PlayerDetection playerDetection;

    [Header(" Elements ")]
    [SerializeField] private MobileJoystick joystick;
    [SerializeField] private BoxCollider groundDetector;
    [SerializeField] private LayerMask groundMask;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    private float ySpeed;

    void Start()
    {
        playerState = PlayerState.Grounded;
    }
    void Update()
    {
        MoveHorizontal();

        MoveVertical();
    }

    private void MoveHorizontal()
    {
        Vector2 moveVector = joystick.GetMoveVector();
        moveVector.x *= moveSpeed;

        float targetX = transform.position.x + moveVector.x * Time.deltaTime;

        Vector2 targetPosition = transform.position.With(x: targetX);

        if (playerDetection.CanGoThere(targetPosition))
            transform.position = targetPosition;
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
        if (!playerDetection.CanGoThere(targetPosition))
        {
            Physics.Raycast(groundDetector.transform.position, Vector3.down, out RaycastHit hit, 1, groundMask);

            if (hit.collider != null)
                targetPosition.y = hit.point.y;

            transform.position = targetPosition;

            Land();
            return;
        }

        transform.position = targetPosition;
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (playerDetection.IsGrounded())
            Land();
    }

    private void StartFalling()
    {
        playerState = PlayerState.Air;
    }

    private void Land()
    {
        playerState = PlayerState.Grounded;
        ySpeed = 0;
    }

    public void Jump()
    {
        playerState = PlayerState.Air;
        ySpeed = jumpSpeed;
    }
}
