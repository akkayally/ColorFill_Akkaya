using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject menuPanel, gameOverPanel;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChange.RemoveListener(HandleGameStateChanged);
    }

    private void Start()
    {
        SetPanelsInitialVisibilities();
    }

    private void SetPanelsInitialVisibilities()
    {
        menuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Listens GameManager for a possible game state change
    /// Arranges visibility of UI elements based on a state change
    /// </summary>
    /// <param name="currentState">Previous state of the game</param>
    /// <param name="prevState">Current state of the game</param>
    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState prevState)
    {
        if(prevState == GameManager.GameState.MENU && currentState == GameManager.GameState.PLAYING)
        {
            menuPanel.SetActive(false);
        }
        else if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.GAME_OVER)
        {
            gameOverPanel.SetActive(true);
        }
        else if(prevState == GameManager.GameState.GAME_OVER && currentState == GameManager.GameState.MENU)
        {
            gameOverPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        else if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.MENU)
        {
            StartCoroutine(ShowMenuWithDelay());
        }
    }

    /// <summary>
    /// Showing menu panel with a little delay
    /// </summary>
    private IEnumerator ShowMenuWithDelay()
    {
        yield return new WaitForSeconds(2.5f);
        menuPanel.SetActive(true);
    }
}
