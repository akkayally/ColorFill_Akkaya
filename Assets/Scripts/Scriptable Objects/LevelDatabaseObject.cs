using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Database", menuName = "Levels/Level Database")]
public class LevelDatabaseObject : ScriptableObject
{
    [SerializeField] List<LevelSettings> levels;

    public List<LevelSettings> Levels => levels;
}
