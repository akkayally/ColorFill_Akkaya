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

    private void Start()
    {
        GenerateGrid(11, 11);
    }
    private int GetFilledGridCount()
    {
        int fillCount = 0;
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
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

    public bool CreateTrailAtPosition(int xCoord, int zCoord)
    {
        if (allGrids[xCoord, zCoord] == GridStatus.EMPTY)
        {
            GameObject trailCube = CubePoolManager.Instance.RequestTrailCube();
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
        return GetFilledGridCount() == (11 * 11);
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
        }
    }
}