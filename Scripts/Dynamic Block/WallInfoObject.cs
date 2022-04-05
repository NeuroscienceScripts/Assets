using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;

public enum WallDirection
{
    Vertical = 0,
    Horizontal = 1
}

public class WallInfoObject : ScriptableObject
{
    [SerializeField] public List<WallDirection> directions = new();
    [SerializeField] public List<GridLocation> positions = new();

    public void Clear()
    {
        directions.Clear();
        positions.Clear();
    }

    public void Add(WallDirection dir, GridLocation pos)
    {
        directions.Add(dir);
        positions.Add(pos);
    }
}
