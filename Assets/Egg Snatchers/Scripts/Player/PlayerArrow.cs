using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerRenderer))]
public class PlayerArrow : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform arrowParent;
    private PlayerRenderer playerRenderer;
    private Egg egg;

    [Header(" Settings ")]
    [SerializeField] private float distanceThreshold = 2;

    // Start is called before the first frame update
    void Start()
    {
        playerRenderer = GetComponent<PlayerRenderer>();
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

        if (playerRenderer.IsInvisible || IsEggCloseEnough())
        {
            HideArrow();
            return;
        }

        ShowArrow();
        arrowParent.up = (egg.transform.position - arrowParent.position).normalized;
    }
    private void StoreEgg()
    {
        egg = FindAnyObjectByType<Egg>();
    }
    private void ShowArrow() => arrowParent.gameObject.SetActive(true);
    private void HideArrow() => arrowParent.gameObject.SetActive(false);

    private bool IsEggCloseEnough()
    {
        float distance = Vector3.Distance(arrowParent.position, egg.transform.position);

        return distance < distanceThreshold;
    }
}
