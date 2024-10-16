using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimator : MonoBehaviour
{
    [Header(" Components ")]
    private PlayerController playerController;

    [Header(" Elements ")]
    [SerializeField] private Animator animator;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    void Update()
    {
        UpdateBlendTree();
    }

    private void UpdateBlendTree()
    {
        animator.SetFloat("xSpeed", playerController.XSpeed);
    }
}
