using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class GridManager : MonoSingleton<GridManager>
{
    [SerializeField] GameEvent OnLevelComplete;

    List<GameObject> trailCubes = new List<GameObject>();

    public GridStatus[,] allGrids;

    private int columnSize, rowSize;
    private int totalGridCount = 0;

    private void OnEnable()
    {
        LevelManager.Instance.OnLevelCreated += SetGridSize;
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChange.RemoveListener(HandleGameStateChanged);
    }

    /// <summary>
    /// Sets the grid's column and row counts when a level is created
    /// </summary>
    /// <param name="_gridSize">Number of columns and rows in the loaded level</param>
    /// <param name="obstacleCoordinates">List of XZ Coordinates of obstacle in the loaded level</param>
    private void SetGridSize(Vector2 _gridSize, List<Vector2> obstacleCoordinates)
    {
        columnSize = (int)_gridSize.x;
        rowSize = (int)_gridSize.y;

        GenerateGrid(columnSize, rowSize);
        SetObstaclePositions(obstacleCoordinates);

        totalGridCount = columnSize * rowSize - obstacleCoordinates.Count;
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState prevState)
    {
        if (prevState == GameManager.GameState.GAME_OVER && currentState == GameManager.GameState.MENU)
        {
            allGrids = null;
            totalGridCount = 0;
        }
    }

    /// <summary>
    /// Creates an obstacle at each position in a given list of XZ coordinates
    /// </summary>
    /// <param name="obstacleCoordinates">XZ Coordinates of obstacles in a level</param>
    private void SetObstaclePositions(List<Vector2> obstacleCoordinates)
    {
        foreach(Vector2 xzCoordinates in obstacleCoordinates)
        {
            int _xCoord = (int)xzCoordinates.x;
            int _zCoord = (int)xzCoordinates.y;

            allGrids[_xCoord, _zCoord] = GridStatus.WALL;
        }
    }

    /// <summary>
    /// Goes through all tiles in the grid and sums the number of filled ones
    /// </summary>
    /// <returns>Number of filled tiles</returns>
    private int GetFilledGridCount()
    {
        int fillCount = 0;
        for (int i = 0; i < columnSize; i++)
        {
            for (int j = 0; j < rowSize; j++)
            {
                fillCount += (allGrids[i, j] == GridStatus.FILLED) ? 1 : 0;
            }
        }
        return fillCount;
    }

    /// <summary>
    /// Based on the column and row counts creates a grid
    /// </summary>
    /// <param name="columnSize">Number of columns</param>
    /// <param name="rowSize">Number of rows</param>
    public void GenerateGrid(int columnSize, int rowSize)
    {
        allGrids = new GridStatus[columnSize, rowSize];

        for(int i = 0; i< columnSize; i++)
        {
            for(int j= 0; j<rowSize; j++)
            {
                allGrids[i, j] = GridStatus.EMPTY;
            }
        }
    }

    /// <summary>
    /// Creates a trail cube at a given XZ coordinates if the tile is empty
    /// </summary>
    /// <param name="xCoord">Column index of a tile</param>
    /// <param name="zCoord">Row index of a tile</param>
    /// <param name="isTrail">If the created cube is a trail cube or a filled one</param>
    /// <returns>If a tile is created at the given position or not</returns>
    public bool CreateTrailAtPosition(int xCoord, int zCoord, bool isTrail)
    {
        if (allGrids[xCoord, zCoord] == GridStatus.EMPTY)
        {
            GameObject trailCube = CubePoolManager.Instance.RequestTrailCube();
            trailCube.tag = (isTrail) ? "Trail" : "Filled";
            trailCube.transform.position = new Vector3(xCoord, 0f, zCoord);
            allGrids[xCoord, zCoord] = GridStatus.TRAIL;
            trailCubes.Add(trailCube);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if a tile on a given XZ coordinate is empty
    /// </summary>
    /// <param name="xCoordinate">Column index of a tile</param>
    /// <param name="zCoordinate">Row index of a tile</param>
    /// <returns></returns>
    public bool IsTileEmpty(int xCoordinate, int zCoordinate)
    {
        return allGrids[xCoordinate, zCoordinate] == GridStatus.EMPTY;
    }

    /// <summary>
    /// Gets coordinates of each empty tile on the grid
    /// </summary>
    /// <returns></returns>
    public List<Vector2> GetEmptyTilesCoordinates()
    {
        List<Vector2> emptyTiles = new List<Vector2>();

        for(int i = 0; i< columnSize; i++)
        {
            for(int j = 0; j<rowSize; j++)
            {
                if (allGrids[i, j] == GridStatus.EMPTY)
                    emptyTiles.Add(new Vector2(i,j));
            }
        }
        return emptyTiles;
    }

    /// <summary>
    /// Sets the statuses of each tile in the group to empty 
    /// </summary>
    /// <param name="tileGroup">A group of tiles</param>
    public void EmptyTileGroup(List<Vector2> tileGroup)
    {
        foreach(Vector2 tilePos in tileGroup)
        {
            int xCoord = (int)tilePos.x;
            int zCoord = (int)tilePos.y;

            allGrids[xCoord, zCoord] = GridStatus.EMPTY;
        }
    }

    public bool CheckEmptyTileLeft()
    {
        return GetFilledGridCount() == totalGridCount;
    }


    /// <summary>
    /// Move all trail cubes up to fill an area
    /// </summary>
    public void MoveTrailCubesUp()
    {
        foreach(GameObject trailCube in trailCubes)
        {
            trailCube.transform.DOMoveY(0.5f, 0.25f);
            trailCube.tag = "Filled";
            Vector3 cubePosition = trailCube.transform.position;
            allGrids[(int)cubePosition.x, (int)cubePosition.z] = GridStatus.FILLED;
        }

        trailCubes.Clear();

        if (CheckEmptyTileLeft())
        {
            OnLevelComplete.Raise();
            
            allGrids = null;
            totalGridCount = 0;
        }
    }
}