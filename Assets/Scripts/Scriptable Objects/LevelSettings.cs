using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Levels/Level Settings", fileName = "New Level Setting")]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    [SerializeField] GameObject obstacle;

    public GameObject Obstacle => obstacle;

    [Tooltip("Position values are X and Z coordinates.")]
    [SerializeField] private List<Vector2> obstaclePositions;

    public List<Vector2> ObstaclePositions => obstaclePositions;

    [SerializeField] private EnemySettings enemy;

    public EnemySettings Enemy => enemy;

    [Tooltip("Please write enemy cube's Z position in the Y value")]
    [SerializeField] private Vector2 enemyPositionXZ;
    public Vector2 EnemyPositionXZ => enemyPositionXZ;
}
