using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Settings", fileName = "New Enemy Setting")]
public class EnemySettings : ScriptableObject
{
    public enum MovementAxis
    {
       NaN,
       HORIZONTAL,
       VERTICAL
    }

    [SerializeField] MovementAxis movementDirection;
    [SerializeField] FloatReference moveSpeed;
    [SerializeField] GameObject prefab;
    public MovementAxis MovementDirection => movementDirection;
    public GameObject Prefab => prefab;
    public FloatReference MoveSpeed;
}
