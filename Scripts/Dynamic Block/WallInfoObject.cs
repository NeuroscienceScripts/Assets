using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;

public enum WallDirection
{
    Vertical = 0,
    Horizontal = 1
}
[System.Serializable]
public struct WallPosition
{
    public WallDirection direction;
    public Coordinate position;
}

public class WallInfoObject : ScriptableObject
{

    public List<WallPosition> wallPositions = new();

    public void Clear()
    {
        wallPositions.Clear();
    }

    public void Add(WallDirection dir, Coordinate pos)
    {
        wallPositions.Add(new WallPosition { direction = dir, position = pos });
    }

    public List<WallDirection> GetDirections()
    {
        List<WallDirection> list = new();
        foreach (var item in wallPositions)
        {
            list.Add(item.direction);
        }
        return list;
    }
    public List<GridLocation> GetPositions()
    {
        List<GridLocation> list = new();
        foreach (var item in wallPositions)
        {
            list.Add(item.position.ToGridLocation());
        }
        return list;
    }
}
