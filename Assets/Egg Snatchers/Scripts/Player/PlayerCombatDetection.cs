using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatDetection : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private SphereCollider[] hitBoxes;

    [Header(" Settings ")]
    private bool canDetect;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!canDetect)
            return;

        DetectHits();
    }

    private void DetectHits()
    {
        foreach (SphereCollider hitBox in hitBoxes)
            DetectSphereHits(hitBox);
    }

    private void DetectSphereHits(SphereCollider hitBox)
    {
        float realRadius = hitBox.radius * Mathf.Abs(hitBox.transform.lossyScale.x);
        Collider[] colliders = Physics.OverlapSphere(hitBox.transform.position, realRadius);

        for (int i = 0; i < colliders.Length; i++)
            ManageCollider(colliders[i]);
    }

    private void ManageCollider(Collider collider)
    {
        if (collider.TryGetComponent(out PlayerController playerController))
        {
            if (playerController == GetComponent<PlayerController>())
                return;
            /*
            if(playerController.IsStun)
                return;
            */

            playerController.GetHit(playerController.OwnerClientId);

        }
    }


    private void StartDetection()
    {
        canDetect = true;
    }

    private void StopDetection()
    {
        canDetect = false;
    }
}
