using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] GameObject wallLeftPrefab;
    [SerializeField] GameObject wallRightPrefab;
    [SerializeField] GameObject wallTopPrefab;
    [SerializeField] GameObject wallBottomPrefab;

    private GameObject wallLeft, wallRight, wallTop, wallBottom;
    List<GameObject> obstacles;
    GameObject enemy;

    [SerializeField] GameObject wallsContainer;
    [SerializeField] GameObject obstacleContainer;

    public Action<Vector2, List<Vector2>> OnLevelCreated;

    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChange.RemoveListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState prevState)
    {
        if(prevState == GameManager.GameState.GAME_OVER && currentState == GameManager.GameState.MENU)
        {
            ResetLevel();
        }
        else if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.MENU)
        {
            StartCoroutine(ResetLevelWithDelay());            
        }
    }

    /// <summary>
    /// Reset current level with a little delay
    /// </summary>
    private IEnumerator ResetLevelWithDelay()
    {
        yield return new WaitForSeconds(2.5f);
        ResetLevel();
    }

    /// <summary>
    /// Destroy walls, enemies and obstacles in a level if one exists
    /// </summary>
    private void ResetLevel()
    {        
        DestroyWalls();
        DestroyEnemy();
        DestroyObstacles();
    }

    /// <summary>
    /// Destroys each wall
    /// </summary>
    private void DestroyWalls()
    {
        Destroy(wallLeft);
        Destroy(wallRight);
        Destroy(wallTop);
        Destroy(wallBottom);
    }


    /// <summary>
    /// Destroy if an enemy exists on a level
    /// </summary>
    private void DestroyEnemy()
    {
        if(enemy != null)
            Destroy(enemy);
    }

    /// <summary>
    /// Destroys obstacles if any exists
    /// </summary>
    private void DestroyObstacles()
    {
       if(obstacles != null)
        {            
            foreach (GameObject _obstacle in obstacles)
            {
                Destroy(_obstacle);
            }
            obstacles.Clear();
        }
    }


    /// <summary>
    /// Creates and position each wall based on level column-row size
    /// </summary>
    /// <param name="columnSize">Number of columns in a level</param>
    /// <param name="rowSize">Number of rows in a level</param>
    private void CreateWalls(int columnSize, int rowSize)
    {
        wallLeft = Instantiate(wallLeftPrefab, new Vector3(0f, 0f, (rowSize - 1) / 2), Quaternion.identity, wallsContainer.transform);
        wallRight = Instantiate(wallRightPrefab, new Vector3(columnSize, 0f, (rowSize - 1) / 2), Quaternion.identity, wallsContainer.transform);
        wallTop = Instantiate(wallTopPrefab, new Vector3((columnSize - 1) / 2, 0f, rowSize), Quaternion.identity, wallsContainer.transform);
        wallBottom = Instantiate(wallBottomPrefab, new Vector3((columnSize - 1) / 2, 0f, -1), Quaternion.identity, wallsContainer.transform);
    }

    /// <summary>
    /// Creates an enemy at given position
    /// </summary>
    /// <param name="enemyPrefab">Enemy to spawn</param>
    /// <param name="spawnPos">XZ Coordinates of spawn position</param>
    private void CreateEnemy(GameObject enemyPrefab, Vector2 spawnPos)
    {
        Vector3 _spawnPos = new Vector3(spawnPos.x, 0f, spawnPos.y);
        enemy = Instantiate(enemyPrefab, _spawnPos, Quaternion.identity);
    }


    /// <summary>
    /// Creates obstacles at given XZ coordinates
    /// </summary>
    /// <param name="obstaclePrefab">Prefab to create an obstacle from</param>
    /// <param name="obstaclePositions">List of XZ coordniates of obstacles in a level</param>
    private void CreateObstacles(GameObject obstaclePrefab, List<Vector2> obstaclePositions)
    {
        obstacles = new List<GameObject>();
        foreach(Vector2 xzCoord in obstaclePositions)
        {
            Vector3 spawnPos = new Vector3(xzCoord.x, 0.5f, xzCoord.y);
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, obstacleContainer.transform);
            obstacles.Add(obstacle);
        }
    }

    #region Public Methods
    /// <summary>
    /// Generates a level based on given level settings
    /// </summary>
    /// <param name="levelSettings">Scriptable object holds the details of a game level</param>
    public void GenerateLevel(LevelSettings levelSettings)
    {
        int columnCount = levelSettings.GridSettings.ColumnSize;
        int rowCount = levelSettings.GridSettings.RowSize;

        CreateWalls(columnCount, rowCount);
        
        if(levelSettings.Enemy != null)
        {
            CreateEnemy(levelSettings.Enemy.Prefab, levelSettings.EnemyPositionXZ);
        }

        if (levelSettings.Obstacle != null)
        {
            CreateObstacles(levelSettings.Obstacle, levelSettings.ObstaclePositions);
        }
        
        if(OnLevelCreated != null)
        {
            OnLevelCreated(new Vector2(columnCount, rowCount), levelSettings.ObstaclePositions);
        }
    }
    #endregion
}
