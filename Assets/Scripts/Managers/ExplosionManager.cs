using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoSingleton<ExplosionManager>
{
    [SerializeField] GameObject enemyExplosionPrefab;
    [SerializeField] GameObject playerExplosionPrefab;

    [SerializeField] GameObject enemyVFXContainer;
    [SerializeField] GameObject playerVFXContainer;

    private List<GameObject> enemyExplosionPool;

    private void Start()
    {
        GenerateEnemyVFXPool();
    }

    private void GenerateEnemyVFXPool()
    {
        enemyExplosionPool = new List<GameObject>();

        for (int i= 0; i<10; i++)
        {
            GameObject go = Instantiate(enemyExplosionPrefab, enemyVFXContainer.transform);
            go.SetActive(false);
            enemyExplosionPool.Add(go);
        }
    }


    private GameObject GetEnemyVFX()
    {
        foreach (var enemyVFX in enemyExplosionPool)
        {
            if (enemyVFX.activeInHierarchy == false)
            {
                enemyVFX.SetActive(true);
                return enemyVFX;
            }
        }

        GameObject newEnemyVFX = Instantiate(enemyExplosionPrefab, enemyVFXContainer.transform);
        enemyExplosionPool.Add(newEnemyVFX);
        return newEnemyVFX;
    }

    public void SpawnEnemyVfxAtPosition(Vector3 spawnPos)
    {
        GameObject enemyVFX = GetEnemyVFX();
        enemyVFX.transform.position = spawnPos;

        StartCoroutine(EnemyParticleHandler(enemyVFX));        
    }

    private IEnumerator EnemyParticleHandler(GameObject particleVFX)
    {
        ParticleSystem _particle = particleVFX.GetComponentInChildren<ParticleSystem>();
        if (_particle != null)
        {
            _particle.Play();
        }

        yield return new WaitForSeconds(1f);

        _particle.Stop();
    }
}
