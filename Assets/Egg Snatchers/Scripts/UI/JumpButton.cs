using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class JumpButton : MonoBehaviour
{
    [Header(" Elements ")]
    private Button button;

    [Header(" Actions ")]
    public static Action onClicked;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => onClicked?.Invoke());
    }
}
