using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] GameEvent OnGameOver;

    Rigidbody playerRb;

    private bool isMoving = false;
    [SerializeField] FloatVariable moveSpeed;
    private Vector3 movementVector = Vector3.zero;
    private Directions movementDirection = Directions.NULL;

    private int xCoord, zCoord;

    private int gridColumnCount, gridRowCount;

    Dictionary<Vector2, Directions> trailPositionDirection;
    private bool isPlayerOnEmptyTile = false;

    private void OnEnable()
    {
        playerRb = GetComponent<Rigidbody>();
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
        InputManager.Instance.OnSwipe += HandleSwipe;
        LevelManager.Instance.OnLevelCreated += AdjustInitialPosition;        
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChange.RemoveListener(HandleGameStateChanged);
        LevelManager.Instance.OnLevelCreated -= AdjustInitialPosition;
        InputManager.Instance.OnSwipe -= HandleSwipe;        
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState prevState)
    {
        if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.GAME_OVER)
        {
            StopMovement();
        }
        else if (prevState == GameManager.GameState.GAME_OVER && currentState == GameManager.GameState.MENU)
        {
            ResetParemeters();
        }
        else if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.MENU)
        {
            StartCoroutine(ResetParametersWithDelay());
        }

    }

    private IEnumerator ResetParametersWithDelay()
    {
        yield return new WaitForSeconds(2.5f);
        ResetParemeters();
    }
        
    /// <summary>
    /// Reset values of all parameters
    /// </summary>
    private void ResetParemeters()
    {
        movementVector = Vector3.zero;
        movementDirection = Directions.NULL;
        xCoord = 0;
        zCoord = 0;
        trailPositionDirection.Clear();
        isPlayerOnEmptyTile = false;
        transform.position = new Vector3(5f, 0.5f, 0f);
    }

    /// <summary>
    /// Positions the player cube when a level is created
    /// </summary>
    /// <param name="gridSize">Loaded level's grid dimension</param>
    private void AdjustInitialPosition(Vector2 gridSize, List<Vector2> obstacleCoordiantes)
    {
        StopMovement();
        float _xPosition = ((int)gridSize.x - 1) / 2;

        transform.position = new Vector3(_xPosition, 0.5f, 0f);

        gridColumnCount = (int)gridSize.x;
        gridRowCount = (int)gridSize.y;
    }


    /// <summary>
    /// On user input first snaps the player on the nearest grid
    /// Sets relevant parameters  for track mechanics
    /// Sets the movement direction according to user input and starts movement
    /// </summary>
    /// <param name="_direction"></param>
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


    /// <summary>
    /// Sets xCoord and zCoord parameters based on player's current position
    /// </summary>
    private void SetXZCoordinates()
    {
        xCoord = Mathf.RoundToInt(transform.position.x);
        zCoord = Mathf.RoundToInt(transform.position.z);
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

        if(trailPositionDirection == null)
        {
            trailPositionDirection = new Dictionary<Vector2, Directions>();
        }
        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;
        Move();
        CheckTrail();
    }

    #region CollisionDetection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            StopMovement();
            FillArea();
        }
        else if (other.CompareTag("Filled") && isPlayerOnEmptyTile) //Player moves from an empty tile to filled one
        {
            FillArea();
        }
    }
    #endregion

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
                    TryCreatingTrailCube();
                    zCoord++;
                }
                break;
            case Directions.DOWN:
                if (transform.position.z < zCoord)
                {
                    TryCreatingTrailCube();
                    zCoord--;                    
                }
                break;
            case Directions.LEFT:
                if (transform.position.x < xCoord)
                {
                    TryCreatingTrailCube();
                    xCoord--;                    
                }
                break;
            case Directions.RIGHT:
                if (transform.position.x > xCoord)
                {
                    TryCreatingTrailCube();
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

    /// <summary>
    /// Tries to create a trail cube if player is on an empty tile
    /// </summary>
    private void TryCreatingTrailCube()
    {
        if (GridManager.Instance.CreateTrailAtPosition(xCoord, zCoord, true))
        {
            isPlayerOnEmptyTile = true;
            trailPositionDirection.Add(new Vector2(xCoord, zCoord), movementDirection);
        }
    }

    /// <summary>
    /// Fills the area when player hits a filled tile or a wall/obstacle
    /// </summary>
    private void FillArea()
    {
        SetXZCoordinates();

        if (trailPositionDirection.Count > 0)
        {
            FillManager.Instance.FillTiles(trailPositionDirection);
        }
               
        trailPositionDirection.Clear();        
    }

    /// <summary>
    /// Stops cube movement
    /// </summary>
    private void StopMovement()
    {
        AdjustPosition();
        isMoving = false;
        playerRb.velocity = Vector3.zero;

        isPlayerOnEmptyTile = false;
        movementDirection = Directions.NULL;
    }


    /// <summary>
    /// Handle animation with delays when a level completed.
    /// </summary>
    private IEnumerator LevelEndAnimation()
    {
        transform.DOMoveX((gridColumnCount - 1) / 2, 1f);
        yield return new WaitForSeconds(1.5f);

        transform.DOMoveZ(gridRowCount + 2f, 1f);
    }

    /// <summary>
    /// When a level completed stops player movement then play level ending animation
    /// </summary>
    public void HandleLevelComplete()
    {
        StopMovement();
        StartCoroutine("LevelEndAnimation");
    }
}
