using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] GameEvent OnWallHit;
    [SerializeField] GameEvent OnGameOver;
    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player hit enemy: Game over!");
            OnGameOver.Raise();
        }
    }
}
