using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;

[System.Serializable]
public class BlockInfoMapping
{
    public string Mapping;
    public List<string> PossibleLocations;
}

[System.Serializable]
public struct CoordList
{
    public List<Coordinate> coords;
}

[CreateAssetMenu(menuName = "ScriptableObjects/Block Info")]
public class BlockInfoObject : ScriptableObject
{
    public Dictionary<GridLocation, Dictionary<GridLocation, List<Coordinate>>> wallLocations = new();
    public List<Coordinate> paintingLocations = new();

    [SerializeField] private List<GridLocation> parents = new();
    [SerializeField] private List<GridLocation> keys = new();
    [SerializeField] private List<CoordList> vals = new();

    [SerializeField]
    private List<BlockInfoMapping> mappings = new();

    public void Clear()
    {
        wallLocations.Clear();
        paintingLocations.Clear();
    }

    public void Add(GridLocation start, Dictionary<GridLocation, List<Coordinate>> locations)
    {
        wallLocations.Add(start, locations);
    }

    public void PopulateList()
    {
        mappings = new();
        parents = new();
        keys = new();
        vals = new();
        foreach (var kvp in wallLocations)
        {
            foreach (var kvp1 in kvp.Value)
            {
                BlockInfoMapping mapping = new() { Mapping = kvp.Key.GetString() + " to " + kvp1.Key.GetString(), PossibleLocations = new() };
                foreach (var coord in kvp1.Value)
                {
                    mapping.PossibleLocations.Add(coord.GridLocString());
                }
                mappings.Add(mapping);
                parents.Add(kvp.Key);
                keys.Add(kvp1.Key);
                vals.Add(new CoordList { coords = kvp1.Value });
            }
        }
    }

    public void MakeDict()
    {
        wallLocations = new();
        Dictionary<GridLocation, int> numParents = new();
        for (int i = 0; i < parents.Count; i++)
        {
            if (numParents.ContainsKey(parents[i]))
            {
                numParents[parents[i]]++;
            }
            else
            {
                numParents.Add(parents[i], 1);
            }
        }
        int count = 0;
        foreach (var kvp in numParents)
        {
            Dictionary<GridLocation, List<Coordinate>> temp = new();
            for (int i = count; i < count + kvp.Value; i++)
            {
                temp.Add(keys[i], vals[i].coords);
            }
            wallLocations.Add(kvp.Key, temp);
            count += kvp.Value;
        }
    }

}
