using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using DefaultNamespace;


public class DynamicBlock : MonoBehaviour
{
    //private string prevRow;
    //private string[] obstaclesList = { "B1", "B3", "B5", "B6", "D2", "D3", "D5", "D6", "F2", "F4", "F5", "F7" };
    [SerializeField] private WallInfoObject info;

    private void OnEnable()
    {
        foreach (var item in info.GetPositions())
        {
            Debug.Log(item);
        }
    }

    //[SerializeField] private GameObject[] walls; // 0 is north, 1 is south, 2 is east, 3 is west
    //[SerializeField] private BlockInfoObject blockInfo;
    //private Trial trial;
    //private Dictionary<GridLocation, Dictionary<GridLocation, List<Coordinate>>> wallLocations = new();
    //private List<GridLocation> blockLocations;
    //private List<GridLocation> nodeTriggers;
    //private WallDirection wallDirection;
    //private GameObject wall;

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

    //private void CreateNodeTriggers()
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

    // Update is called once per frame
    //void Update()
    //{
    //    if (ExperimentController.Instance.GetTrialInfo().stressTrial)
    //    {
    //        //Currently this will just compare where the start and end are relative to each other and determine what the row right
    //        //before the end is. TODO: Determine an effective method for when start and end are on the same row, as for now it defaults
    //        //to the row after it (this can be an issue if start is mailbox and end is chair for example as it goes out of bounds to Row H)
    //        if (ExperimentController.Instance.GetTrialInfo().start.GetY().CompareTo(ExperimentController.Instance.GetTrialInfo().end.GetY()) < 0)
    //        {
    //            prevRow = (char)(ExperimentController.Instance.GetTrialInfo().end.y[0] - 1) + "";
    //        }
    //        else
    //        {
    //            prevRow = (char)(ExperimentController.Instance.GetTrialInfo().end.y[0] + 1) + "";
    //        }

    //        //if end is to your left it will try to block your left
    //        //if end is to your right it will try to block to your right
    //        //if end is ahead, it will try blocking top and bottom
    //        //if left/right were unsuccessful, it will default to top/bottom
    //        if (ControllerCollider.Instance.currentNode.y.Equals(prevRow))
    //        {
    //            //left case which reverts into top/bot
    //            if(ExperimentController.Instance.GetTrialInfo().end.x < ControllerCollider.Instance.currentNode.x)
    //            {
    //                if (Array.IndexOf(obstaclesList, prevRow + (ControllerCollider.Instance.currentNode.GetX() - 1 + "")) == -1)
    //                    ExperimentController.Instance.SetTrialBlockedLocation(ControllerCollider.Instance.currentNode.GetString().Substring(0,1), (int)ControllerCollider.Instance.currentNode.GetX() - 1);
    //                else
    //                    ExperimentController.Instance.SetTrialBlockedLocation(ExperimentController.Instance.GetTrialInfo().end.GetString().Substring(0,1), ExperimentController.Instance.GetTrialInfo().end.GetX());
    //            } else if (ExperimentController.Instance.GetTrialInfo().end.GetX() > ControllerCollider.Instance.currentNode.GetX()) //right case
    //            {
    //                if (Array.IndexOf(obstaclesList, prevRow + (ControllerCollider.Instance.currentNode.x + 1 + "")) == -1)
    //                    ExperimentController.Instance.SetTrialBlockedLocation(ControllerCollider.Instance.currentNode.GetString().Substring(0, 1), ControllerCollider.Instance.currentNode.GetX() + 1);
    //                else
    //                    ExperimentController.Instance.SetTrialBlockedLocation(ExperimentController.Instance.GetTrialInfo().end.GetString().Substring(0, 1), ExperimentController.Instance.GetTrialInfo().end.GetX());
    //            } else //case that you are right in front of the end
    //            {
    //                ExperimentController.Instance.SetTrialBlockedLocation(ExperimentController.Instance.GetTrialInfo().end.GetString().Substring(0, 1), ExperimentController.Instance.GetTrialInfo().end.GetX());
    //            }
    //        }
    //    }
    //    else
    //    {
    //        this.gameObject.transform.position = new Vector3(12, 12, 12);
    //    }
    //}
}
