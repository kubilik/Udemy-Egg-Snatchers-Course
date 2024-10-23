using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private GameObject[] panels;

    private void Awake()
    {
        panels = new GameObject[] { gamePanel, winPanel, losePanel };
    }

    private void ShowPanel(GameObject panel)
    {
        for (int i = 0; i < panels.Length; i++)
            panels[i].SetActive(panels[i] == panel);
    }

    public void GameStateChangedCallback(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Game:
                ShowPanel(gamePanel);
                break;

            case GameState.Win:
                ShowPanel(winPanel);
                break;

            case GameState.Lose:
                ShowPanel(losePanel);
                break;

            default:
                ShowPanel(gamePanel);
                break;
        }
    }
}
