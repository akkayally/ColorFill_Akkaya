using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailController : MonoBehaviour
{
    [SerializeField] GameEvent OnPlayerHit;
    [SerializeField] GameEvent OnObstacleHit;


    void Start()
    {
        AdjustPosition();
    }
    private void AdjustPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerHit.Raise();
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Trail it enemy: Game over!");
            OnObstacleHit.Raise();
        }
    }
}
