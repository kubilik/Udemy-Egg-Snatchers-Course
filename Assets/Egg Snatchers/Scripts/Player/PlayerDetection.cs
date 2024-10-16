using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private CapsuleCollider capsuleCollider;

    void Start()
    {

    }


    void Update()
    {

    }

    /*
    public bool CanGoThere(Vector3 targetPosition)
    {
        Vector3 center = targetPosition + boxCollider.center;
        Collider[] colliders = Physics.OverlapBox(center, boxCollider.bounds.extents / 2, Quaternion.identity, groundMask);

        foreach (Collider col in colliders)
            Debug.Log(col.name);

        return colliders.Length > 0;

        //return true;
    }*/

    public bool CanGoThere(Vector3 targetPosition)
    {
        Vector3 center = targetPosition + capsuleCollider.center;

        float halfHeight = (capsuleCollider.height / 2) - capsuleCollider.radius;
        Vector3 offset = transform.up * halfHeight;
        Vector3 point0 = center + offset;
        Vector3 point1 = center - offset;

        return Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, groundMask).Length > 0; //<=0
    }

    public bool IsGrounded()
    {
        return Physics.OverlapBox(boxCollider.transform.position, boxCollider.size / 2, Quaternion.identity, groundMask).Length > 0;
    }

}
