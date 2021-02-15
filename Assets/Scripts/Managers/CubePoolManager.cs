using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePoolManager : MonoSingleton<CubePoolManager>
{
    [SerializeField] GameObject trailPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject enemyPrefab;


    [SerializeField] private GameObject _trailContainer;
    [SerializeField] private GameObject _wallContainer;
    [SerializeField] private GameObject _enemyContainer;

    private List<GameObject> _trailPool;
    private List<GameObject> _wallPool;
    private List<GameObject> _enemyPool;

    // Start is called before the first frame update
    void Start()
    {
        GenerateTrailCubePool();
    }

    private void GenerateTrailCubePool()
    {
        _trailPool = new List<GameObject>();

        for(int i= 0; i< 25; i++)
        {
            GameObject trailCube = Instantiate(trailPrefab,Vector3.zero, Quaternion.identity, _trailContainer.transform);
            trailCube.SetActive(false);
            _trailPool.Add(trailCube);
        }
    }

    public GameObject RequestTrailCube()
    {
        foreach(var trail in _trailPool)
        {
            if(trail.activeInHierarchy == false)
            {
                trail.SetActive(true);
                return trail;
            }
        }

        GameObject newTrail = Instantiate(trailPrefab, _trailContainer.transform);
        _trailPool.Add(newTrail);
        return newTrail;
    }
}
