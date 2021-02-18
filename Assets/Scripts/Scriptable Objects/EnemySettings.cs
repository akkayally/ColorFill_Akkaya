using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Settings", fileName = "New Enemy Setting")]
public class EnemySettings : ScriptableObject
{
    [SerializeField] MovementAxis movementDirection;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject prefab;
    [SerializeField] int moveDistance;

    public MovementAxis MovementDirection => movementDirection;
    public GameObject Prefab => prefab;
    public float MoveSpeed => moveSpeed;
    public int MoveDistance => moveDistance;
}
