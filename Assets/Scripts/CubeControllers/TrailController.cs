using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
    }
    private void OnDisable()
    {
        GameManager.Instance.OnGameStateChange.RemoveListener(HandleGameStateChanged);
    }

    private void Start()
    {
        AdjustPosition();
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState prevState)
    {
        if(prevState == GameManager.GameState.GAME_OVER && currentState == GameManager.GameState.MENU) //On Game Over
        {
            gameObject.SetActive(false);
        }
        if(prevState == GameManager.GameState.PLAYING && currentState == GameManager.GameState.MENU) //On level complete
        {
            StartCoroutine(DestoryCubeWithDelay());
        }
    }

    /// <summary>
    /// Push the cube to GridPool manager with a little delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestoryCubeWithDelay()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// When created snaps the cube to the nearest grid
    /// </summary>
    private void AdjustPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
    }
}
