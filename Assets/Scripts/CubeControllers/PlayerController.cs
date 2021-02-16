using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody playerRb;

    private bool isMoving = false;
    [SerializeField] FloatVariable moveSpeed;
    private Vector3 movementVector = Vector3.zero;
    private Directions movementDirection = Directions.NULL;

    private int xCoord, zCoord;

    private List<Vector2> movementCoordinates;
    private List<Directions> movementDirections;

    private bool isPlayerOnEmptyTile = false;

    private void OnEnable()
    {
        InputManager.Instance.OnSwipe += HandleSwipe;        
        playerRb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        InputManager.Instance.OnSwipe -= HandleSwipe;
    }

    private void HandleSwipe(Directions _direction)
    {
        AdjustPosition();
        SetXZCoordinates();
        SetMovementDirection(_direction);
    }

    /// <summary>
    /// Adjust player's position to fit on the grid correctly
    /// </summary>
    private void AdjustPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
    }

    private void SetXZCoordinates()
    {
        xCoord = Mathf.RoundToInt(transform.position.x);
        zCoord = Mathf.RoundToInt(transform.position.z);

        if (movementCoordinates == null)
        {
            movementCoordinates = new List<Vector2>();
        }

        if (GridManager.Instance.IsTileEmpty(xCoord, zCoord))
        {
            movementCoordinates.Add(new Vector2(xCoord, zCoord));
            isPlayerOnEmptyTile = true;
        }
        else
        {
            isPlayerOnEmptyTile = false;
        }
    }

    /// <summary>
    /// Sets movement direction vector value based on user input
    /// </summary>
    /// <param name="direction">Swipe direction</param>
    private void SetMovementDirection(Directions direction)
    {
        switch (direction)
        {
            case Directions.UP:
                movementVector = Vector3.forward;
                break;
            case Directions.LEFT:
                movementVector = Vector3.left;
                break;
            case Directions.DOWN:
                movementVector = Vector3.back;
                break;
            case Directions.RIGHT:
                movementVector = Vector3.right;
                break;
        }
        movementDirection = direction;
        
        isMoving = true;

        if(movementDirections == null)
        {
            movementDirections = new List<Directions>();
        }
        if (isPlayerOnEmptyTile)
        {
            movementDirections.Add(movementDirection);
        }        
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;
        Move();
        CheckTrail();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            StopMovement();
        }
        else if (other.CompareTag("Filled") && isPlayerOnEmptyTile) //Player moves from empty tile to filled one
        {
            StopMovement();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        int _xCoord = Mathf.RoundToInt(transform.position.x);
        int _zCoord = Mathf.RoundToInt(transform.position.z);

        if (other.CompareTag("Filled") && GridManager.Instance.IsTileEmpty(_xCoord, _zCoord))
        {
            movementCoordinates.Add(new Vector2(_xCoord, _zCoord));
            movementDirections.Add(movementDirection);
        }
    }


    /// <summary>
    /// Moves the cube on a direction
    /// </summary>
    private void Move()
    {
        playerRb.velocity = movementVector * moveSpeed.Value;
    }

    /// <summary>
    /// Based on the movement direction creates a trail cube behind if player moves on a new tile
    /// </summary>
    private void CheckTrail()
    {
        switch (movementDirection)
        {
            case Directions.UP:
                if (transform.position.z > zCoord)
                {
                    GridManager.Instance.CreateTrailAtPosition(xCoord, zCoord);                    
                    zCoord++;
                }
                break;
            case Directions.DOWN:
                if (transform.position.z < zCoord)
                {
                    GridManager.Instance.CreateTrailAtPosition(xCoord, zCoord);
                    zCoord--;                    
                }
                break;
            case Directions.LEFT:
                if (transform.position.x < xCoord)
                {
                    GridManager.Instance.CreateTrailAtPosition(xCoord, zCoord);
                    xCoord--;                    
                }
                break;
            case Directions.RIGHT:
                if (transform.position.x > xCoord)
                {
                    GridManager.Instance.CreateTrailAtPosition(xCoord, zCoord);
                    xCoord++;                    
                }
                break;
            case Directions.NULL:
                Debug.LogWarning("[PLAYERCONTROLLER] : Trying to create trail when direction is not set!");
                break;
            default:
                break;
        }
    }

    public void StopMovement()
    {
        AdjustPosition();
        isMoving = false;
        playerRb.velocity = Vector3.zero;
        movementDirection = Directions.NULL;

        SetXZCoordinates();

        if(movementCoordinates.Count > 0 && movementDirections.Count > 0)
        {
            FillManager.Instance.FillTiles(movementCoordinates, movementDirections);
        }

        movementCoordinates.Clear();
        movementDirections.Clear();
    }
}
