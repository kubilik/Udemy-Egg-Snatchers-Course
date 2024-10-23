using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerDetection))]
public class PlayerFill : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Renderer[] renderers;
    private PlayerDetection playerDetection;

    [Header(" Settings ")]
    private float fillAmount;
    private const string fillAmountRef = "Fill_Amount";

    private void Awake()
    {
        playerDetection = GetComponent<PlayerDetection>();
    }

    void Start()
    {
        fillAmount = 1;
        UpdateRenderers();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateFill(float fillStep)
    {
        if (playerDetection.IsHoldingEgg)
            fillAmount += fillStep;
        else
            fillAmount -= fillStep;

        fillAmount = Mathf.Clamp01(fillAmount);
        UpdateRenderers();
    }

    private void UpdateRenderers()
    {
        foreach (Renderer renderer in renderers)
            renderer.material.SetFloat(fillAmountRef, fillAmount);
    }
}
