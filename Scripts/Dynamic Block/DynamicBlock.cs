using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using DefaultNamespace;


public class DynamicBlock : MonoBehaviour
{

    [SerializeField] private WallInfoObject wallInfo;
    private GameObject temp;

    [SerializeField] private GameObject[] walls; // 0 is north, 1 is south, 2 is east, 3 is west
    private List<GameObject> possWalls;

    private void OnEnable()
    {
        if (!ExperimentController.Instance.GetTrialInfo().stressTrial)
        {
            enabled = false;
            return;
        }
        ActivateWalls();
    }

    private void Awake()
    {
        possWalls = new();
        InstantiateWalls();
        DisableWalls();
        enabled = false;
        
    }

    private void OnDisable()
    {
        temp = null;
        if(ControllerCollider.Instance != null)
        {
            ControllerCollider.Instance.wallActivated = false;
            ControllerCollider.Instance.currWall = "";
        }
        
    }

    private void Update()
    {
        if (ControllerCollider.Instance.wallActivated)
        {
            for (int i = 0; i < possWalls.Count; i++)
            {
                if (possWalls[i].name == ControllerCollider.Instance.currWall)
                {
                    Debug.Log(ControllerCollider.Instance.currWall);
                    if (possWalls[i].name.Contains("N"))
                    {
                        possWalls[i + 1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        possWalls[i + 1].transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
                        enabled = false;
                    }
                    else if (possWalls[i].name.Contains("S"))
                    {
                        possWalls[i - 1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        possWalls[i - 1].transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
                        enabled = false;
                    }
                    else if (possWalls[i].name.Contains("E"))
                    {
                        possWalls[i + 1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        possWalls[i + 1].transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
                        enabled = false;
                    }
                    else if (possWalls[i].name.Contains("W"))
                    {
                        possWalls[i - 1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        possWalls[i - 1].transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
                        enabled = false;
                    }
                    break;
                }
            }
        }
    }

    private void InstantiateWalls()
    {
        GameObject parent = new();
        parent.name = "Walls";
        foreach (WallPosition wp in wallInfo.wallPositions)
        {
            if(wp.direction == WallDirection.Horizontal)
            {
                temp = Instantiate(walls[2], new Vector3(wp.position.ToGridLocation().GetX(), 1, wp.position.ToGridLocation().GetY()), walls[2].transform.rotation);
                temp.name = wp.position.ToGridLocation().GetString() + "E";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
                temp = Instantiate(walls[3], new Vector3(wp.position.ToGridLocation().GetX(), 1, wp.position.ToGridLocation().GetY()), walls[3].transform.rotation);
                temp.name = wp.position.ToGridLocation().GetString() + "W";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
            }
            else
            {
                temp = Instantiate(walls[0], new Vector3(wp.position.ToGridLocation().GetX(), 1, wp.position.ToGridLocation().GetY()), walls[0].transform.rotation);
                temp.name = wp.position.ToGridLocation().GetString() + "N";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
                temp = Instantiate(walls[1], new Vector3(wp.position.ToGridLocation().GetX(), 1, wp.position.ToGridLocation().GetY()), walls[1].transform.rotation);
                temp.name = wp.position.ToGridLocation().GetString() + "S";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
            }
        }
    }

    public void DisableWalls()
    {
        foreach (GameObject go in possWalls)
        {
            go.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            go.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = true;
            go.SetActive(false);
        }
    }

    private void ActivateWalls()
    {
        GridLocation startLocation = ExperimentController.Instance.GetTrialInfo().start;

        foreach (GameObject go in possWalls)
        {
            if (!(startLocation.GetY() == -(Constants.GRID_LENGTH / 2)) && !(startLocation.GetY() == Constants.GRID_LENGTH / 2))
            {
                if(go.transform.position != new Vector3(startLocation.GetX(), 1f, startLocation.GetY())) go.SetActive(true);
            }
            else
            {
                if(go.transform.position.z != -(Constants.GRID_LENGTH / 2) && go.transform.position.z != Constants.GRID_LENGTH / 2)
                {
                    if (go.transform.position != new Vector3(startLocation.GetX(), 1f, startLocation.GetY())) go.SetActive(true);
                }
            }
        }

        //for (int i = 0; i < wallInfo.GetPositions().Count; i++)
        //{
        //    WallDirection dir = wallInfo.GetDirections()[i];
        //    switch (dir)
        //    {
        //        case WallDirection.Vertical:
        //            if (!startTrial.GetString().Contains("A") & !startTrial.GetString().Contains("G") & !wallInfo.GetPositions()[i].GetString().Contains("A1")) //watch out for top and bottom start
        //            {
        //                temp = Instantiate(walls[2], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[2].transform.rotation) as GameObject;
        //                temp.name = wallInfo.GetPositions()[i].GetString() + "E";
        //                temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        //                possWalls.Add(temp);
        //                temp = Instantiate(walls[3], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[3].transform.rotation) as GameObject;
        //                temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        //                temp.name = wallInfo.GetPositions()[i].GetString() + "W";
        //                possWalls.Add(temp);
        //            }
        //            break;
        //        case WallDirection.Horizontal:
        //            temp = Instantiate(walls[0], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[0].transform.rotation) as GameObject;
        //            temp.name = wallInfo.GetPositions()[i].GetString() + "N";
        //            temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        //            possWalls.Add(temp);
        //            temp = Instantiate(walls[1], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[1].transform.rotation) as GameObject;
        //            temp.name = wallInfo.GetPositions()[i].GetString() + "S";
        //            temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        //            possWalls.Add(temp);
        //            break;
        //    }
        //}
    }
}