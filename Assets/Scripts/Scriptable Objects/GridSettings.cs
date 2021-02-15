using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid Settings", fileName = "New Grid Setting")]
public class GridSettings : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    [Tooltip("Number of Columns on the Game Screen.")]
    [SerializeField] private int _columnSize;
    [Tooltip("Number of Rows on the Game Screen.")]
    [SerializeField] private int _rowSize;

    public int ColumnSize => _columnSize;
    public int RowSize => _rowSize;
}