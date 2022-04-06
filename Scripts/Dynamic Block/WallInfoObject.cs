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
    [SerializeField] private List<WallDirection> directions = new();
    [SerializeField] private List<GridLocation> _positions = new();
    public List<GridLocation> positions = new();

    public void Clear()
    {
        directions.Clear();
        positions.Clear();
    }

    public void Add(WallDirection dir, GridLocation pos)
    {
        directions.Add(dir);
        _positions.Add(pos);
    }

    public List<WallDirection> GetDirections()
    {
        List<WallDirection> list = new();
        foreach (var dir in directions)
        {
            list.Add(dir);
        }
        return list;
    }
    public List<GridLocation> GetPositions()
    {
        positions = new();
        foreach (var pos in _positions)
        {
            positions.Add(pos);
        }
        return positions;
    }
}
