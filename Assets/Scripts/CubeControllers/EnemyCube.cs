using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCube : MonoBehaviour
{
    [SerializeField] GameEvent OnGameOver;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Trail"))
        {
            OnGameOver.Raise();
        }
        else if (other.CompareTag("Filled"))
        {
            DestroyEnemyCube();
        }
    }

    private void DestroyEnemyCube()
    {
        ExplosionManager.Instance.SpawnEnemyVfxAtPosition(transform.position);
        gameObject.SetActive(false);        
    }
}
