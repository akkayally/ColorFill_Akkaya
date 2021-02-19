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

    /// <summary>
    /// Creates a pool for enemy particle effects
    /// </summary>
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


    /// <summary>
    /// Gets an available enemy explosion fx from the pool
    /// If no available vfx found pops one
    /// </summary>
    /// <returns>Particle system for enemy explosion</returns>
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

    /// <summary>
    /// Gets a game object, finds the particle system in its children and if found plays it
    /// </summary>
    /// <param name="particleVFX">Game object with particle VFX in its child</param>
    private IEnumerator ParticleHandler(GameObject particleVFX)
    {
        ParticleSystem _particle = particleVFX.GetComponentInChildren<ParticleSystem>();
        if (_particle != null)
        {
            _particle.Play();
        }

        yield return new WaitForSeconds(1f);

        _particle.Stop();
    }

    /// <summary>
    /// Create a player particle fx in a given position
    /// </summary>
    /// <param name="spawnPos">Player's position</param>
    public void SpawnPlayerVfxAtPosition(Vector3 spawnPos)
    {
        GameObject playerVFX = Instantiate(playerExplosionPrefab, playerVFXContainer.transform);

        playerVFX.transform.position = spawnPos;

        StartCoroutine(ParticleHandler(playerVFX));
    }

    /// <summary>
    /// Gets an enemy explosion fx from the pool and plays it in the given position
    /// </summary>
    /// <param name="spawnPos">Position to spawn enemy particle effect</param>
    public void SpawnEnemyVfxAtPosition(Vector3 spawnPos)
    {
        GameObject enemyVFX = GetEnemyVFX();
        enemyVFX.transform.position = spawnPos;

        StartCoroutine(ParticleHandler(enemyVFX));        
    }


}
