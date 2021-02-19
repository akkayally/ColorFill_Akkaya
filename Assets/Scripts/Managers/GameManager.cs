using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public Events.EventGameState OnGameStateChange;

    public enum GameState
    {
        MENU,
        PLAYING,
        GAME_OVER
    }

    [SerializeField] GameState _currentGameState;

    private void OnEnable()
    {
        LevelManager.Instance.OnLevelCreated += HandleLevelStart;
    }
    private void OnDisable()
    {
        LevelManager.Instance.OnLevelCreated -= HandleLevelStart;
    }

    private void HandleLevelStart(Vector2 gridSize, List<Vector2> obstaclePositions)
    {
        UpdateState(GameState.PLAYING);
    }

    private void Start()
    {
        UpdateState(GameState.MENU);
    }

    private void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;

        switch (_currentGameState)
        {
            case GameState.MENU:
                break;
            case GameState.PLAYING:
                break;
            case GameState.GAME_OVER:
                break;
            default:
                break;
        }
        OnGameStateChange.Invoke(_currentGameState, previousGameState);
    }

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    public void GameOver()
    {
        UpdateState(GameState.GAME_OVER);
    }

    public void MainMenu()
    {
        UpdateState(GameState.MENU);
    }

    public void LevelCompleted()
    {
        UpdateState(GameState.MENU);
    }

}
