using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager instance;

    [Header(" Elements ")]
    [SerializeField] private AudioSource jumpSource;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayJumpSound()
    {
        PlayJumpSoundRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void PlayJumpSoundRpc()
    {
        jumpSource.Play();
    }
}
