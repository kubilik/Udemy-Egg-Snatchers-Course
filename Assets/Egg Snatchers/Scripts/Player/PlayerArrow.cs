using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform arrowParent;
    private Egg egg;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AimAtEgg();
    }

    private void AimAtEgg()
    {
        if (egg == null)
        {
            StoreEgg();
            return;
        }

        arrowParent.up = (egg.transform.position - arrowParent.position).normalized;
    }
    private void StoreEgg()
    {
        egg = FindAnyObjectByType<Egg>();
    }
}
