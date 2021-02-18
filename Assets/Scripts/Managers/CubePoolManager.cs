using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePoolManager : MonoSingleton<CubePoolManager>
{
    [SerializeField] GameObject trailPrefab;

    [SerializeField] private GameObject _cubesContainer;

    private List<GameObject> _trailPool;

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
            GameObject trailCube = Instantiate(trailPrefab,Vector3.zero, Quaternion.identity, _cubesContainer.transform);
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

        GameObject newTrail = Instantiate(trailPrefab, _cubesContainer.transform);
        _trailPool.Add(newTrail);
        return newTrail;
    }
}
