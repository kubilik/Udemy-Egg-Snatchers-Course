using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private BoxCollider boxCollider;

    void Start()
    {

    }


    void Update()
    {

    }

    public bool CanGoThere(Vector3 targetPosition)
    {
        Vector3 center = targetPosition + boxCollider.center;
        Collider[] colliders = Physics.OverlapBox(center, boxCollider.bounds.extents / 2, Quaternion.identity, groundMask);

        foreach (Collider col in colliders)
            Debug.Log(col.name);

        return colliders.Length > 0;

        //return true;
    }
}
