using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Settings", fileName = "New Level Setting")]
public class LevelSettings : ScriptableObject
{
    [SerializeField] private GridSettings gridSettings;
    public GridSettings GridSettings => gridSettings;

    [SerializeField] private List<Vector2> obstaclePositions;

    public List<Vector2> ObstaclePositions => obstaclePositions;
}
