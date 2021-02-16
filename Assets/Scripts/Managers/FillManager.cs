using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FillManager : MonoSingleton<FillManager>
{
    private bool isFilledBefore = false;

    [SerializeField] FloatVariable moveUpDelay;

    Directions rayDirection = Directions.NULL;
    Vector3 rayDirectionVector = Vector3.zero;
    float rayOffset = 0.5f;

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
    /// Determines the raydirection vector based on player's first movement direction
    /// </summary>
    /// <param name="_direction">first movement direction</param>
    /// <param name="turnPositions">Each corner points in player movement list</param>
    private void SetRayDirection(Directions _direction, List<Vector2> turnPositions)
    {
        rayDirectionVector = Vector3.zero;

        if (_direction == Directions.DOWN || _direction == Directions.UP) //the movement is vertical
        {
            if (turnPositions[0].x != turnPositions[turnPositions.Count - 1].x)
            {
                rayDirectionVector = (turnPositions[0].x < turnPositions[turnPositions.Count - 1].x) ? Vector3.right : Vector3.left;
                rayDirection = (turnPositions[0].x < turnPositions[turnPositions.Count - 1].x) ? Directions.RIGHT : Directions.LEFT;
            }
            else
            {
                float xAverage = turnPositions.Average(x => x.x) + Random.Range(-0.1f, 0.1f);
                rayDirectionVector = (11 - xAverage - 1 > xAverage) ? Vector3.left : Vector3.right;
                rayDirection = (11 - xAverage > xAverage) ? Directions.LEFT : Directions.RIGHT;
            }
        }
        else //the movement is horizontal
        {
            if (turnPositions[0].y != turnPositions[turnPositions.Count - 1].y)
            {
                rayDirectionVector = (turnPositions[0].y < turnPositions[turnPositions.Count - 1].y) ? Vector3.forward : Vector3.back;
                rayDirection = (turnPositions[0].y < turnPositions[turnPositions.Count - 1].y) ? Directions.UP : Directions.DOWN;
            }
            else
            {
                float yAverage = turnPositions.Average(x => x.y) + Random.Range(-0.1f, 0.1f);
                rayDirectionVector = (11 - yAverage - 1 > yAverage) ? Vector3.back : Vector3.forward;
                rayDirection = (11 - yAverage - 1 > yAverage) ? Directions.DOWN : Directions.UP;
            }
        }
    }

    /// <summary>
    /// Finds possible grid coordinates to fill along a movement direction
    /// </summary>
    /// <param name="_moveDirection">player movement direction</param>
    /// <param name="moveStartPos">position of the first grid on given path</param>
    /// <param name="moveEndPos">position of the last grid on given path</param>
    private void AddFillCoordinatesOnMovePath(Directions _moveDirection, Vector2 moveStartPos, Vector2 moveEndPos)
    {
        if (rayDirectionVector != Vector3.zero && _moveDirection != Directions.NULL)
        {
            Vector3 rayEndPos = Vector3.zero;
            Vector3 rayStartPos = new Vector3(moveStartPos.x, 0.1f, moveStartPos.y) + (rayDirectionVector * rayOffset);

            if (_moveDirection == Directions.UP || _moveDirection == Directions.DOWN) //Vertical movment
            {
                for (int i = 0; i <= Mathf.Abs(moveEndPos.y - moveStartPos.y); i++)
                {
                    rayEndPos = CastRay(rayStartPos);
                    for (int j = 0; j < Mathf.Abs(rayEndPos.x - moveStartPos.x) - 1; j++)
                    {
                        float xCoordinate, zCoordinate;

                        if (moveEndPos.y > moveStartPos.y) //Up
                        {
                            xCoordinate = moveStartPos.x + rayDirectionVector.x * (j + 1);
                            zCoordinate = moveStartPos.y + i + rayDirectionVector.z;

                        }
                        else // Down
                        {
                            xCoordinate = moveStartPos.x + rayDirectionVector.x * (j + 1);
                            zCoordinate = moveStartPos.y - i - rayDirectionVector.z;
                        }
                        fillXZCoordinates.Add(new Vector2(xCoordinate, zCoordinate));
                    }
                    rayStartPos += (moveEndPos.y > moveStartPos.y) ? Vector3.forward : Vector3.back;
                }
            }
            else
            {
                for (int i = 0; i <= Mathf.Abs(moveEndPos.x - moveStartPos.x); i++) //Horizontal movement
                {
                    rayEndPos = CastRay(rayStartPos);
                    for (int j = 0; j < Mathf.Abs(rayEndPos.z - moveStartPos.y) - 1; j++)
                    {
                        float xCoordinate, zCoordinate;

                        if (moveEndPos.x > moveStartPos.x) //Right
                        {
                            xCoordinate = moveStartPos.x + i + rayDirectionVector.x;
                            zCoordinate = moveStartPos.y + rayDirectionVector.z * (j + 1);
                        }
                        else //left
                        {
                            xCoordinate = moveStartPos.x - i - rayDirectionVector.x;
                            zCoordinate = moveStartPos.y + rayDirectionVector.z * (j + 1);
                        }
                        fillXZCoordinates.Add(new Vector2(xCoordinate, zCoordinate));
                    }
                    rayStartPos += (moveEndPos.x > moveStartPos.x) ? Vector3.right : Vector3.left;
                }
            }
        }
    }

    /// <summary>
    /// Casts a ray to find end point of a row/column
    /// </summary>
    /// <param name="startPos">Ray start position</param>
    /// <returns>The position of hit object(a wall or a trail cube)</returns>
    private Vector3 CastRay(Vector3 startPos)
    {
        Vector3 _hitPosition = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(startPos, rayDirectionVector, out hit, 11f, LayerMask.GetMask("Cube")))
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Trail") || hit.collider.CompareTag("Filled"))
            {
                _hitPosition = hit.transform.position;
            }
        }
        return _hitPosition;
    }

    /// <summary>
    /// Creates trail cubes at each possible fill position
    /// </summary>
    private IEnumerator CreateCubes()
    {
        foreach (Vector2 xzCoordinate in fillXZCoordinates)
        {
            int _xCoordinate = (int)xzCoordinate.x;
            int _zCoordinate = (int)xzCoordinate.y;
            GridManager.Instance.CreateTrailAtPosition(_xCoordinate, _zCoordinate);
        }
        yield return new WaitForSeconds(moveUpDelay.Value);
        GridManager.Instance.MoveTrailCubesUp();
        yield return null;
        if (GridManager.Instance.CheckEmptyTileLeft())
        {
            Debug.Log("Level Completed!");
            isFilledBefore = false;
        }
        isFilledBefore = true;
    }


    /// <summary>
    /// Based on player movement data fills the grid
    /// </summary>
    /// <param name="turnPositions">List of corner points of player movement</param>
    /// <param name="moveDirections">List of movement directions</param>
    public void FillTiles(List<Vector2> turnPositions, List<Directions> moveDirections)
      {
        ResetFillXZCoordinates();

        SetRayDirection(moveDirections[1], turnPositions);


        if (isFilledBefore && moveDirections.Count > 1)
        {
            SetRayDirection(moveDirections[1], turnPositions);
        }
        else
        {
            SetRayDirection(moveDirections[0], turnPositions);
        }

        for (int i = 0; i < moveDirections.Count; i++)
        {
            if (!isFilledBefore)
            {
                if (moveDirections[i] == moveDirections[0])
                {
                    AddFillCoordinatesOnMovePath(moveDirections[i], turnPositions[i], turnPositions[i + 1]);
                }
            }
            else
            {
                if (moveDirections[i] == moveDirections[1])
                {
                    AddFillCoordinatesOnMovePath(moveDirections[i], turnPositions[i], turnPositions[i + 1]);
                }
            }
        }

        if (fillXZCoordinates.Count > 0)
        {
            StartCoroutine("CreateCubes");
        }
    }
}
