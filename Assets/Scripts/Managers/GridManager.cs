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

    private void SetObstaclePositions(List<Vector2> obstacleCoordinates)
    {
        foreach(Vector2 xzCoordinates in obstacleCoordinates)
        {
            int _xCoord = (int)xzCoordinates.x;
            int _zCoord = (int)xzCoordinates.y;

            allGrids[_xCoord, _zCoord] = GridStatus.WALL;
        }
    }

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

    public bool IsTileEmpty(int xCoordinate, int zCoordinate)
    {
        return allGrids[xCoordinate, zCoordinate] == GridStatus.EMPTY;
    }

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