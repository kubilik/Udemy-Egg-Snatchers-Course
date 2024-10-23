using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRenderer : NetworkBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Renderer[] renderers;

    [Header(" Settings ")]
    [SerializeField] private AnimationCurve fadeCurve;
    private const string transparencyRef = "Transparency";
    public bool IsInvisible { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Hide() => HideRpc();

    [Rpc(SendTo.Everyone)]
    private void HideRpc()
    {
        if (IsInvisible)
            return;

        IsInvisible = true;

        float alpha = IsOwner ? .2f : 0;

        LeanTween.value(1, alpha, 2)
            .setEase(fadeCurve)
            .setOnUpdate((value) => UpdateTransparency(value));

        LeanTween.delayedCall(10, Appear);

        //UpdateTransparency(alpha);
    }

    private void Appear() => AppearRpc();

    [Rpc(SendTo.Everyone)]
    private void AppearRpc()
    {
        float alpha = IsOwner ? .2f : 0;

        LeanTween.value(alpha, 1, 2)
            .setEase(fadeCurve)
            .setOnUpdate((value) => UpdateTransparency(value))
            .setOnComplete(() => IsInvisible = false);
    }


    private void UpdateTransparency(float alpha)
    {
        foreach (Renderer renderer in renderers)
            renderer.material.SetFloat(transparencyRef, alpha);
    }
}