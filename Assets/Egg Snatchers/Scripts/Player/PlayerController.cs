using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    }

    private void MoveHorizontal()
    {
        Vector2 moveVector = joystick.GetMoveVector();
        moveVector.x *= moveSpeed;

        float targetX = transform.position.x + moveVector.x * Time.deltaTime;

        Vector2 targetPosition = transform.position.With(x: targetX);

        transform.position = targetPosition;
    }
}
