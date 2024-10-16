using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header(" Components ")]
    [SerializeField] PlayerDetection playerDetection;

    [Header(" Elements ")]
    [SerializeField] private MobileJoystick joystick;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;

    void Start()
    {

    }
    void Update()
    {
        MoveHorizontal();

        if (playerDetection.IsGrounded())
            Debug.Log("We're Grounded !! ");
    }

    private void MoveHorizontal()
    {
        Vector2 moveVector = joystick.GetMoveVector();
        moveVector.x *= moveSpeed;

        float targetX = transform.position.x + moveVector.x * Time.deltaTime;

        Vector2 targetPosition = transform.position.With(x: targetX);

        if (!playerDetection.CanGoThere(targetPosition))
            transform.position = targetPosition;
    }
}
