using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FillManager : MonoSingleton<FillManager>
{
    [SerializeField] FloatVariable moveUpDelay;
    List<Vector2> fillXZCoordinates;

    /// <summary>
    /// Creates or clears the fill coordinates
    /// </summary>
    private void ResetFillXZCoordinates()
    {
        if (fillXZCoordinates == null)
        {
            fillXZCoordinates = new List<Vector2>();
        }
        else
        fillXZCoordinates.Clear();
    }

    /// <summary>
    /// Finds the XZ coordinates of empty tiles in a closed area
    /// </summary>
    /// <param name="tilePosXZ">An empty tile's XZ coordinates</param>
    /// <returns>The list of all the empty tiles' XZ coordinates</returns>
    private List<Vector2> FloodFill(Vector2 tilePosXZ)
    {
        List<Vector2> fillCoordinates = new List<Vector2>();
        Stack<Vector2> tiles = new Stack<Vector2>();

        tiles.Push(tilePosXZ);

        while (tiles.Count > 0)
        {
            Vector2 _temp = tiles.Pop();
            if (_temp.x >= 0 && _temp.x < 11 && _temp.y >= 0 && _temp.y < 11)
            {
                if (GridManager.Instance.allGrids[(int)_temp.x, (int)_temp.y] == GridStatus.EMPTY)
                {
                    GridManager.Instance.allGrids[(int)_temp.x, (int)_temp.y] = GridStatus.TRAIL;
                    fillCoordinates.Add(new Vector2(_temp.x, _temp.y));

                    tiles.Push(new Vector2(_temp.x - 1, _temp.y));
                    tiles.Push(new Vector2(_temp.x + 1, _temp.y));
                    tiles.Push(new Vector2(_temp.x, _temp.y - 1));
                    tiles.Push(new Vector2(_temp.x, _temp.y + 1));
                }
            }
        }
        return fillCoordinates;
    }

    /// <summary>
    /// Creates trail cubes at each possible fill position
    /// </summary>
    private IEnumerator CreateCubes()
    {
        if(fillXZCoordinates.Count > 0)
        {
            foreach (Vector2 xzCoordinate in fillXZCoordinates)
            {
                int _xCoordinate = (int)xzCoordinate.x;
                int _zCoordinate = (int)xzCoordinate.y;
                GridManager.Instance.CreateTrailAtPosition(_xCoordinate, _zCoordinate);
            }
        }
        
        yield return new WaitForSeconds(moveUpDelay.Value);
        GridManager.Instance.MoveTrailCubesUp();

        yield return null;
    }

    /// <summary>
    /// Finds if trail cubes has two empty tiles as neighbour
    /// </summary>
    /// <param name="xzCoord">XZ coordiantes of a trail cube</param>
    /// <param name="moveDirection">Trail cubes' current movement direction</param>
    /// <returns>List of XZ coordinates of empty neighbours</returns>
    private List<Vector2> GetEmptyNeighbours(Vector2 xzCoord, Directions moveDirection)
    {
        List<Vector2> neighboursAtSides = new List<Vector2>();
        int xCoord = (int)xzCoord.x;
        int zCoord = (int)xzCoord.y;

        if (moveDirection == Directions.UP || moveDirection == Directions.DOWN)
        {
            if(xzCoord.x > 0 && xzCoord.x < 11 - 1)
            {
                if(GridManager.Instance.IsTileEmpty(xCoord -1, zCoord) && GridManager.Instance.IsTileEmpty(xCoord + 1, zCoord))
                {
                    neighboursAtSides.Add(new Vector2(xCoord - 1, zCoord));
                    neighboursAtSides.Add(new Vector2(xCoord + 1, zCoord));
                }
            }
        }
        else
        {
            if(xzCoord.y > 0 && xzCoord.y < 11 - 1)
            {
                if(GridManager.Instance.IsTileEmpty(xCoord, zCoord + 1) && GridManager.Instance.IsTileEmpty(xCoord, zCoord - 1))
                {
                    neighboursAtSides.Add(new Vector2(xCoord, zCoord + 1));
                    neighboursAtSides.Add(new Vector2(xCoord, zCoord - 1));
                }
            }
        }
        return neighboursAtSides;
    }

    /// <summary>
    /// Gets two tiles' position on opposite sides of user's trail
    /// Calculates the area covered at each side
    /// Add each tile in the smaller area in fill list
    /// </summary>
    /// <param name="emptyNeighbourCoordinates">XZ coordinates of two tiles on opposite sides of a trail cube</param>
    private void SetSmallArea(List<Vector2> emptyNeighbourCoordinates)
    {
        List<Vector2> tileGroup1 = FloodFill(emptyNeighbourCoordinates[0]);
        List<Vector2> tileGroup2 = FloodFill(emptyNeighbourCoordinates[1]);

        if(tileGroup1.Count > 0 && tileGroup2.Count > 0)
        {
            if(tileGroup1.Count == tileGroup2.Count)
            {
                AddToFillCoordinates(tileGroup1);
            }
            else if(tileGroup1.Count < tileGroup2.Count)
            {
                AddToFillCoordinates(tileGroup1);
            }
            else
            {
                AddToFillCoordinates(tileGroup2);
            }

            GridManager.Instance.EmptyTileGroup(tileGroup1); //Resetting tile statuses to empty before actually filling it
            GridManager.Instance.EmptyTileGroup(tileGroup2); //Resetting tile statuses to empty before actually filling it
        }
    }

    /// <summary>
    /// Adds each tile's xz coordinates to list
    /// </summary>
    /// <param name="tilesToAdd">List of XZ coordinates of empty tiles</param>
    private void AddToFillCoordinates(List<Vector2> tilesToAdd)
    {
        foreach(Vector2 tilePos in tilesToAdd)
        {
            fillXZCoordinates.Add(tilePos);
        }
    }

    /// <summary>
    /// Fills the smallest area based on user's trail
    /// </summary>
    /// <param name="trailPosAndDirections">Player's each movements coordinates and the movement direction pair</param>
    public void FillTiles(Dictionary<Vector2, Directions> trailPosAndDirections)
      {
        ResetFillXZCoordinates();

        Vector2 neighbour1 = Vector2.zero;
        Vector2 neighbour2 = Vector2.zero;

        foreach(KeyValuePair<Vector2, Directions> _trailPosDirection in trailPosAndDirections)
        {
            List<Vector2> emptyNeighbourCoordinates = GetEmptyNeighbours(_trailPosDirection.Key, _trailPosDirection.Value);

            if(emptyNeighbourCoordinates.Count == 2) // Both neighbours on sides are empty
            {
                SetSmallArea(emptyNeighbourCoordinates);
                    break;
            }
        }
        StartCoroutine("CreateCubes");
    }
}
