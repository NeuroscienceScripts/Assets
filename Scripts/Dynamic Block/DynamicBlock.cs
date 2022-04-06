using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using DefaultNamespace;


public class DynamicBlock : MonoBehaviour
{
    public static DynamicBlock Instance { get; private set; }

    [SerializeField] private WallInfoObject wallInfo;
    public string collName;
    //private string prevRow;
    //private string[] obstaclesList = { "B1", "B3", "B5", "B6", "D2", "D3", "D5", "D6", "F2", "F4", "F5", "F7" };

    [SerializeField] private GameObject[] walls; // 0 is north, 1 is south, 2 is east, 3 is west
    private List<GameObject> possWalls;
    //[SerializeField] private BlockInfoObject blockInfo;
    //private Trial trial;
    //private Dictionary<GridLocation, Dictionary<GridLocation, List<Coordinate>>> wallLocations = new();
    //private List<GridLocation> blockLocations;
    //private List<GridLocation> nodeTriggers;
    //private WallDirection wallDirection;
    //private GameObject wall;

    private void OnEnable()
    {
        CreateWallTriggers();
    }
    //private void OnEnable()
    //{
    //    blockLocations = new();
    //    nodeTriggers = new();
    //    blockInfo.MakeDict();
    //    wallLocations = blockInfo.wallLocations;
    //    trial = ExperimentController.Instance.GetTrialInfo();
    //    if (trial.stressTrial)
    //    {
    //        foreach (Coordinate coordinate in wallLocations[trial.start][trial.end])
    //        {
    //            blockLocations.Add(coordinate.ToGridLocation());
    //        }
    //    }
    //    CheckWallDirection();
    //    CreateNodeTriggers();
    //}

    //private void OnDisable()
    //{
    //    blockLocations = null;
    //    nodeTriggers = null;
    //}

    //private void Update()
    //{
    //    int index = IndexOf(nodeTriggers, ControllerCollider.Instance.currentNode);
    //    Debug.Log(index);
    //    if (index >= 0)
    //    {
    //        wall = Instantiate(walls[(int)wallDirection], new Vector3(blockLocations[index].GetX(), 1, blockLocations[index].GetY()), walls[(int)wallDirection].transform.rotation);
    //        enabled = false;
    //    }
    //}

    /*private int IndexOf(List<GameObject> goList, GridLocation target)
    {
        if()
    }
    */
    //private int IndexOf(List<GridLocation> list, GridLocation target)
    //{
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        if(list[i] == target)
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}

    //private void CheckWallDirection()
    //{
    //    if(blockLocations[0].GetX() == blockLocations[1].GetX())
    //    {
    //        // Horizontal Wall
    //        if(trial.start.GetX() < trial.end.GetX())
    //        {
    //            //Instantiate facing west
    //            wallDirection = WallDirection.West;
    //        }
    //        else
    //        {
    //            // east
    //            wallDirection = WallDirection.East;
    //        }
    //    }
    //    else if(blockLocations[0].GetY() == blockLocations[1].GetY())
    //    {
    //        // Vertical
    //        if(trial.start.GetY() > trial.end.GetY())
    //        {
    //            // south
    //            wallDirection = WallDirection.North;
    //        }
    //        else
    //        {
    //            // north
    //            wallDirection = WallDirection.South;
    //        }
    //    }
    //}

    private void CreateWallTriggers()
    {
        GameObject temp;
        for (int i = 0; i < wallInfo.GetPositions().Count; i++)
        {
            WallDirection dir = wallInfo.GetDirections()[i];
            switch (dir)
            {
                case WallDirection.Vertical:
                    temp = Instantiate(walls[2], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[2].transform.rotation) as GameObject;
                    temp.name = wallInfo.GetPositions()[i].GetString() + "E";
                    //temp.GetComponent<MeshRenderer>().enabled = false;
                    possWalls.Add(temp);
                    temp = Instantiate(walls[3], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[3].transform.rotation) as GameObject;
                    //temp.GetComponent<MeshRenderer>().enabled = false;
                    temp.name = wallInfo.GetPositions()[i].GetString() + "W";
                    possWalls.Add(temp);
                    break;
                case WallDirection.Horizontal:
                    temp = Instantiate(walls[0], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[0].transform.rotation) as GameObject;
                    temp.name = wallInfo.GetPositions()[i].GetString() + "N";
                    //temp.GetComponent<MeshRenderer>().enabled = false;
                    possWalls.Add(temp);
                    temp = Instantiate(walls[1], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[1].transform.rotation) as GameObject;
                    temp.name = wallInfo.GetPositions()[i].GetString() + "S";
                    //temp.GetComponent<MeshRenderer>().enabled = false;
                    possWalls.Add(temp);
                    break;
            }
        }
    }

    //private void CreatewallInfo.positions[i]Triggers()
    //{
    //    foreach (GridLocation node in blockLocations)
    //    {
    //        switch (wallDirection)
    //        {
    //            case WallDirection.North:
    //                nodeTriggers.Add(new GridLocation(node.GetX(), node.GetY()+1));
    //                break;
    //            case WallDirection.South:
    //                nodeTriggers.Add(new GridLocation(node.GetX(), node.GetY() - 1));
    //                break;
    //            case WallDirection.East:
    //                nodeTriggers.Add(new GridLocation(node.GetX() + 1, node.GetY()));
    //                break;
    //            case WallDirection.West:
    //                nodeTriggers.Add(new GridLocation(node.GetX() - 1, node.GetY()));
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
