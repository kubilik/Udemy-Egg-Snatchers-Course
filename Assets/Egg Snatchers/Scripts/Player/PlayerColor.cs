using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerColor : NetworkBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private SkinnedMeshRenderer[] renderers;

    [Header(" Settings ")]
    [SerializeField] private Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
            return;

        int colorIndex = Random.Range(0, colors.Length);
        ColorizeRpc(colorIndex);
    }

    [Rpc(SendTo.Everyone)]
    private void ColorizeRpc(int colorIndex)
    {

        foreach (SkinnedMeshRenderer renderer in renderers)
            renderer.material.color = colors[colorIndex];
    }
}
