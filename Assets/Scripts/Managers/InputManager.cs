using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    private Directions swipeDirection;

    [SerializeField] float SWIPE_THRESHOLD = 25f;

    private Touch theTouch;
    private Vector2 touchStartPosition, touchEndPosition;

    public Action<Directions> OnSwipe;

    /// <summary>
    /// Reads user input and sets movement direction
    /// </summary>
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            swipeDirection = Directions.UP;
            if (OnSwipe != null)
            {
                OnSwipe(swipeDirection);
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            swipeDirection = Directions.LEFT;
            if (OnSwipe != null)
            {
                OnSwipe(swipeDirection);
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            swipeDirection = Directions.DOWN;
            if (OnSwipe != null)
            {
                OnSwipe(swipeDirection);
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            swipeDirection = Directions.RIGHT;
            if (OnSwipe != null)
            {
                OnSwipe(swipeDirection);
            }
        }
#else
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            if (theTouch.phase == TouchPhase.Began)
            {
                touchStartPosition = theTouch.position;
                touchEndPosition = theTouch.position;
            }

            if (theTouch.phase == TouchPhase.Ended)
            {
                touchEndPosition = theTouch.position;
                SetInputType();
            }            
        }
#endif
    }

    /// <summary>
    /// Calculating the swipe direction
    /// </summary>
    private void CheckSwipe()
    {
        //Check if Vertical swipe
        if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalMove())
        {
            if (touchStartPosition.y - touchEndPosition.y > 0)
            {
                swipeDirection = Directions.DOWN;
            }
            else if (touchStartPosition.y - touchEndPosition.y < 0)
            {
                swipeDirection = Directions.UP;
            }
            touchEndPosition = touchStartPosition;
        }
        //Check if Horizontal swipe
        else if (HorizontalMove() > SWIPE_THRESHOLD && HorizontalMove() > VerticalMove())
        {
            if (touchStartPosition.x - touchEndPosition.x > 0)
            {
                swipeDirection = Directions.LEFT;
            }
            else if (touchStartPosition.x - touchEndPosition.x < 0)
            {
                swipeDirection = Directions.RIGHT;
            }
            touchEndPosition = touchStartPosition;
        }

        if (OnSwipe != null)
        {
            OnSwipe(swipeDirection);
        }
    }

    /// <summary>
    /// Checks if user movement is vertical
    /// </summary>
    /// <returns></returns>
    float VerticalMove()
    {
        return Mathf.Abs(touchEndPosition.y - touchStartPosition.y);
    }

    /// <summary>
    /// Checks if user movement is horizontal
    /// </summary>
    /// <returns></returns>
    float HorizontalMove()
    {
        return Mathf.Abs(touchEndPosition.x - touchStartPosition.x);
    }

}
