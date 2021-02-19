using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
        LevelManager.Instance.OnLevelCreated += SetPosition;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChange.RemoveListener(HandleGameStateChanged);
        LevelManager.Instance.OnLevelCreated -= SetPosition;
    }

    /// <summary>
    /// Listens GameManager for gamestate changes and sets camera position
    /// </summary>
    /// <param name="currentState">Current state of the game</param>
    /// <param name="prevState">Previous state of the game</param>
    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState prevState)
    {
        if (prevState == GameManager.GameState.GAME_OVER && currentState == GameManager.GameState.MENU)
        {
            ResetPosition();
        }
        if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.MENU)
        {
            StartCoroutine(ResetPositionWithDelay());
        }
    }

    /// <summary>
    /// Resets the camera's position after a little delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetPositionWithDelay()
    {
        yield return new WaitForSeconds(2.5f);
        ResetPosition();
    }


    /// <summary>
    /// Resets the camera's position for main menu
    /// </summary>
    private void ResetPosition()
    {
        Camera.main.transform.position = new Vector3(5f, 22f, 5f);
    }


    /// <summary>
    /// Sets the camera position according to the level's grid dimension
    /// </summary>
    /// <param name="gridDimensions">Loaded level's number of columns and rows</param>
    private void SetPosition(Vector2 gridDimensions, List<Vector2> obstaclePositions)
    {
        float xCoordinate = (gridDimensions.x - 1) / 2;
        float zCoordinate = (gridDimensions.y > 18) ? 5f : 2f;
        Camera.main.transform.position = new Vector3(xCoordinate, 22f, zCoordinate);
    }
}
