using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerRenderer : NetworkBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Renderer[] renderers;

    [Header(" Settings ")]
    private const string transparencyRef = "Transparency";

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
        float alpha = IsOwner ? .2f : 0;
        UpdateTransparency(alpha);
    }

    private void UpdateTransparency(float alpha)
    {
        foreach (Renderer renderer in renderers)
            renderer.material.SetFloat(transparencyRef, alpha);
    }
}
