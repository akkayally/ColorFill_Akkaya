using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] GameObject wallLeft;
    [SerializeField] GameObject wallRight;
    [SerializeField] GameObject wallTop;
    [SerializeField] GameObject wallBottom;

    [SerializeField] GameObject wallsContainer;
    [SerializeField] GameObject obstacleContainer;

    [SerializeField] LevelSettings level;

    public Action<Vector2, List<Vector2>> OnLevelCreated;
    private void Start()
    {
        GenerateLevel(level);
    }

    /// <summary>
    /// Creates and position each wall based on level column-row size
    /// </summary>
    /// <param name="columnSize">Number of columns in a level</param>
    /// <param name="rowSize">Number of rows in a level</param>
    private void CreateWalls(int columnSize, int rowSize)
    {
        GameObject _wallLeft = Instantiate(wallLeft, new Vector3(0f, 0f, (rowSize - 1) / 2), Quaternion.identity, wallsContainer.transform);
        GameObject _wallRight = Instantiate(wallRight, new Vector3(columnSize, 0f, (rowSize - 1) / 2), Quaternion.identity, wallsContainer.transform);
        GameObject _wallTop = Instantiate(wallTop, new Vector3((columnSize - 1) / 2, 0f, rowSize), Quaternion.identity, wallsContainer.transform);
        GameObject _wallBottom = Instantiate(wallBottom, new Vector3((columnSize - 1) / 2, 0f, -1), Quaternion.identity, wallsContainer.transform);
    }

    /// <summary>
    /// Creates an enemy at given position
    /// </summary>
    /// <param name="enemyPrefab">Enemy to spawn</param>
    /// <param name="spawnPos">XZ Coordinates of spawn position</param>
    private void CreateEnemy(GameObject enemyPrefab, Vector2 spawnPos)
    {
        Vector3 _spawnPos = new Vector3(spawnPos.x, 0f, spawnPos.y);
        GameObject enemy = Instantiate(enemyPrefab, _spawnPos, Quaternion.identity);
    }

    private void CreateObstacles(GameObject obstaclePrefab, List<Vector2> obstaclePositions)
    {
        foreach(Vector2 xzCoord in obstaclePositions)
        {
            Vector3 spawnPos = new Vector3(xzCoord.x, 0.5f, xzCoord.y);
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, obstacleContainer.transform);
        }
    }

    #region Public Methods
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
