using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Powerup : NetworkBehaviour
{
    public enum Type { Speed, Invisibility }

    [field: SerializeField] public Type PowerupType { get; private set; }

    public void Destroy() => GetComponent<NetworkObject>().Despawn();
}
