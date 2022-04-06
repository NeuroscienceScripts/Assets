using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using DefaultNamespace;


public class DynamicBlock : MonoBehaviour
{

    [SerializeField] private WallInfoObject wallInfo;
    public GameObject temp;

    [SerializeField] private GameObject[] walls; // 0 is north, 1 is south, 2 is east, 3 is west
    private List<GameObject> possWalls;

    private void OnEnable()
    {
        temp = new GameObject();
        possWalls = new();
        CreateWallTriggers();
    }

    private void OnDisable()
    {
        possWalls = null;
        temp = null;
    }

    private void Update()
    {
        if(ControllerCollider.Instance.wallActivated)
        {
            for(int i = 0; i < possWalls.Count; i++)
            {
                if(possWalls[i].name == ControllerCollider.Instance.currWall)
                {
                    if(possWalls[i].name.Contains("N"))
                    {
                        possWalls[i+1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        enabled = false;
                    } else if (possWalls[i].name.Contains("S"))
                    {
                        possWalls[i - 1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        enabled = false;
                    } else if(possWalls[i].name.Contains("E"))
                    {
                        possWalls[i+1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        enabled = false;
                    } else if(possWalls[i].name.Contains("W"))
                    {
                        possWalls[i-1].transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        enabled = false;
                    }

                }
            }
        }
    }

    private void CreateWallTriggers()
    {
        //TODO: FIX THIS LINE IT IS A TEST FOR NOW
        GridLocation startTrial = new GridLocation("A", 1);
        //GridLocation startTrial = ExperimentController.Instance.GetTrialInfo().start;
        for (int i = 0; i < wallInfo.GetPositions().Count; i++)
        {
            WallDirection dir = wallInfo.GetDirections()[i];
            switch (dir)
            {
                case WallDirection.Vertical:
                    if(!startTrial.GetString().Contains("A") & !startTrial.GetString().Contains("G") & !wallInfo.GetPositions()[i].GetString().Contains("A1")) //watch out for top and bottom start
                    {
                        temp = Instantiate(walls[2], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[2].transform.rotation) as GameObject;
                        temp.name = wallInfo.GetPositions()[i].GetString() + "E";
                        temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                        possWalls.Add(temp);
                        temp = Instantiate(walls[3], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[3].transform.rotation) as GameObject;
                        temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                        temp.name = wallInfo.GetPositions()[i].GetString() + "W";
                        possWalls.Add(temp);
                    }
                    break;
                case WallDirection.Horizontal:
                    temp = Instantiate(walls[0], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[0].transform.rotation) as GameObject;
                    temp.name = wallInfo.GetPositions()[i].GetString() + "N";
                    temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    possWalls.Add(temp);
                    temp = Instantiate(walls[1], new Vector3(wallInfo.GetPositions()[i].GetX(), 1, wallInfo.GetPositions()[i].GetY()), walls[1].transform.rotation) as GameObject;
                    temp.name = wallInfo.GetPositions()[i].GetString() + "S";
                    temp.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    possWalls.Add(temp);
                    break;
            }
        }
    }
}
