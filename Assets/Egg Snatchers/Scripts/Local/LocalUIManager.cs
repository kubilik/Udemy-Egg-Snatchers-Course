using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LocalUIManager : MonoBehaviour, ILocalGameStateListener
{
    [Header(" Panels ")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject networkScanPanel;
    [SerializeField] private GameObject joiningPanel;

    [Header(" Elements ")]
    [SerializeField] private TextMeshProUGUI ipText;
    [SerializeField] private GameObject joinButton;

    private GameObject[] panels;

    private void Awake()
    {
        IPButton.onClicked += IPButtonClickedCallback;

        panels = new GameObject[]
        {
            menuPanel,
            waitingPanel,
            networkScanPanel,
            joiningPanel
        };
    }
    private void OnDestroy()
    {
        IPButton.onClicked -= IPButtonClickedCallback;
    }

    private void IPButtonClickedCallback(string ip) => joinButton.SetActive(true);




    private void Start()
    {
        ipText.text = "Local IP : " + NetworkUtilities.GetLocalIPv4();
    }

    private void ShowPanel(GameObject panel)
    {
        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(panels[i] == panel);
    }

    public void GameStateChangedCallback(LocalGameState gameState)
    {
        switch (gameState)
        {
            case LocalGameState.Menu:
                ShowPanel(menuPanel);
                break;

            case LocalGameState.Waiting:
                ShowPanel(waitingPanel);
                break;

            case LocalGameState.Scanning:
                joinButton.SetActive(false);
                ShowPanel(networkScanPanel);
                break;

            case LocalGameState.Joining:
                ShowPanel(joiningPanel);
                break;

            default:
                ShowPanel(menuPanel);
                break;
        }
    }
}
