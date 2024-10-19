using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerDetection : MonoBehaviour
{
    [Header(" Components ")]
    private PlayerController playerController;

    [Header(" Elements ")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask trampolineMask;
    [SerializeField] private LayerMask eggMask;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private CapsuleCollider capsuleCollider;

    public bool IsHoldingEgg { get; private set; }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {
        DetectTrampolines();

        if (!IsHoldingEgg)
            DetectEggs();
    }


    private void DetectTrampolines()
    {
        if (!IsGrounded())
            return;

        if (Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, trampolineMask).Length > 0)
            playerController.Jump();
    }

    private void DetectEggs()
    {
        Collider[] detectedEggs = DetectColliders(transform.position, eggMask, out Collider egg);

        if (egg == null)
            return;

        GrabEgg(egg);

        Debug.Log("Egg Detected");
    }

    private void GrabEgg(Collider egg)
    {
        egg.transform.SetParent(transform);
        egg.transform.localPosition = Vector3.up * 3.5f;

        IsHoldingEgg = true;
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
