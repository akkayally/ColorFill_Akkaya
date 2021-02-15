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

    private void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;

        switch (_currentGameState)
        {
            case GameState.MENU:
                break;
            case GameState.PLAYING:
                Time.timeScale = 1;
                break;
            case GameState.GAME_OVER:
                Time.timeScale = 0;
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
}
