using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [SerializeField] EnemySettings enemySettings;

    private Rigidbody rb;
    private bool isMoving = false;
    private MovementAxis movementAxis;
    private float moveSpeed;
    private int moveDistance;

    private bool dirFirst;

    private float minPosition, maxPosition;

    private void Awake()
    {
        SetValues();
    }

    private void SetValues()
    {
        if (enemySettings.MovementDirection != MovementAxis.NaN) //if the enemy is set to move
        {
            isMoving = true;
            movementAxis = enemySettings.MovementDirection;
            moveSpeed = enemySettings.MoveSpeed;
            moveDistance = enemySettings.MoveDistance;
            if(movementAxis == MovementAxis.HORIZONTAL)
            {
                minPosition = transform.position.x - moveDistance;
                maxPosition = transform.position.x + moveDistance;
            }
            else
            {
                minPosition = transform.position.z - moveDistance;
                maxPosition = transform.position.z + moveDistance;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        switch (movementAxis)
        {
            case MovementAxis.HORIZONTAL:
                MoveHorizontally();
                break;
            case MovementAxis.VERTICAL:
                MoveVertically();
                break;
            case MovementAxis.NaN:
                Debug.LogWarning("[ENEMY CONTROLLER: Invalid movement axis!]");
                break;
        }
    }

    private void MoveHorizontally()
    {
        if (dirFirst)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }

        if(transform.position.x >= maxPosition)
        {
            dirFirst = false;
        }
        if(transform.position.x <= minPosition)
        {
            dirFirst = true;
        }
    }
    private void MoveVertically()
    {
        if (dirFirst)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        if (transform.position.z >= maxPosition)
        {
            dirFirst = false;
        }
        if (transform.position.z <= minPosition)
        {
            dirFirst = true;
        }

    }
}
