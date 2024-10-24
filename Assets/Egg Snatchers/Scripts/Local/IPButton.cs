using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Button))]
public class IPButton : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private TextMeshProUGUI ipText;

    public string IP { get; private set; }

    public static Action<string> onClicked;

    public void Configure(string ip)
    {
        ipText.text = ip;
        IP = ip;

        GetComponent<Button>().onClick.AddListener(() => onClicked?.Invoke(IP));
    }
}
