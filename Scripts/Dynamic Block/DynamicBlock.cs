using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using DefaultNamespace;
using TMPro;

public class DynamicBlock : MonoBehaviour
{
    [SerializeField] private WallInfoObject wallInfo;
    private GameObject temp;

    [SerializeField] private GameObject[] walls; // 0 is north, 1 is south, 2 is east, 3 is west
    private List<GameObject> possWalls;
    //[SerializeField] private List<MeshRenderer> renderers;
    //[SerializeField] private List<BoxCollider> colliders;

    [SerializeField] private AudioSource wallSound;
    private bool playedSound = false;

    private GridLocation prevNode;

    private Dictionary<GridLocation, GameObject[]> horizWalls;
    private Dictionary<GridLocation, GameObject[]> vertWalls;

    public bool wallActivated;

    private void OnEnable()
    {
        int num = ExperimentController.Instance.subjectNumber % 2;
        Trial t = ExperimentController.Instance.GetTrialInfo();
        if (!t.stressTrial || (num != 0 && t.isWallTrial) || (num == 0 && !t.isWallTrial))
        {
            enabled = false;
            return;
        }
        ActivateWalls();
    }

    private void Awake()
    {
        vertWalls = new();
        horizWalls = new(); 
        possWalls = new();
        //renderers = new();
        //colliders = new();
        InstantiateWalls();
        DisableWalls();
        enabled = false;
        playedSound = false;
    }

    private void OnDisable()
    {
        temp = null;
        //if(ControllerCollider.Instance != null)
        //{
        //    ControllerCollider.Instance.wallActivated = false;
        //    ControllerCollider.Instance.currWall = "";
        //}
        
    }

    [SerializeField] private TextMeshProUGUI nodeOut; 
    private void Update()
    {
        if (!wallActivated)
        {
            Debug.Log("Inside");
            GridLocation currentNode = NodeExtension.CurrentNode(ExperimentController.Instance.player.transform.position);
            nodeOut.text = currentNode.GetString(); 
            if (prevNode != null && currentNode != prevNode)
            {
                if (horizWalls.ContainsKey(currentNode))
                {
                    if(prevNode.GetX() > currentNode.GetX())
                    {
                        // larger to smaller
                        Activate(horizWalls[currentNode][1]);
                    }
                    else
                    {
                        Activate(horizWalls[currentNode][0]);
                    }
                }
                else if (vertWalls.ContainsKey(currentNode))
                {
                    if (prevNode.GetY() > currentNode.GetY())
                    {
                        // larger to smaller
                        Activate(vertWalls[currentNode][1]);
                    }
                    else
                    {
                        Activate(vertWalls[currentNode][0]);
                    }
                }
            }

            prevNode = currentNode;
        }
        

        //if (ControllerCollider.Instance.wallActivated)
        //{
        //    if (!wallSound.isPlaying & !playedSound)
        //    {
        //        wallSound.Play();
        //        playedSound = true;
        //    }
        //    for (int i = 0; i < possWalls.Count; i++)
        //    {
        //        if (possWalls[i].name == ControllerCollider.Instance.currWall)
        //        {
                    
        //            if (possWalls[i].name.Contains("North"))
        //            {
        //                ExperimentController.Instance.blockedWall = possWalls[i + 1].name;
        //                renderers[i + 1].enabled = true;
        //                colliders[i + 1].isTrigger = false;
        //                enabled = false;
        //            }
        //            else if (possWalls[i].name.Contains("South"))
        //            {
        //                ExperimentController.Instance.blockedWall = possWalls[i - 1].name;
        //                renderers[i - 1].enabled = true;
        //                colliders[i - 1].isTrigger = false;
        //                enabled = false;
        //            }
        //            else if (possWalls[i].name.Contains("East"))
        //            {
        //                ExperimentController.Instance.blockedWall = possWalls[i + 1].name;
        //                renderers[i + 1].enabled = true;
        //                colliders[i + 1].isTrigger = false;
        //                enabled = false;
        //            }
        //            else if (possWalls[i].name.Contains("West"))
        //            {
        //                ExperimentController.Instance.blockedWall = possWalls[i - 1].name;
        //                renderers[i - 1].enabled = true;
        //                colliders[i - 1].isTrigger = false;
        //                enabled = false;
        //            }
        //            break;
        //        }
        //    }
        //}
    }
    

    private void InstantiateWalls()
    {
        GameObject parent = new();
        parent.name = "Walls";

        GridLocation gridLoc;
        foreach (WallPosition wp in wallInfo.wallPositions)
        {
            gridLoc = wp.position.ToGridLocation();
            if (wp.direction == WallDirection.Horizontal)
            {
                
                temp = Instantiate(walls[2], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()), walls[2].transform.rotation);
                temp.name = gridLoc.GetString() + "East";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
                horizWalls.Add(gridLoc, new GameObject[2] { temp, null });

                //renderers.Add(temp.transform.GetComponentInChildren<MeshRenderer>());
                //colliders.Add(temp.transform.GetComponentInChildren<BoxCollider>());


                temp = Instantiate(walls[3], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()), walls[3].transform.rotation);
                temp.name = gridLoc.GetString() + "West";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
                horizWalls[gridLoc][1] = temp;

                //renderers.Add(temp.transform.GetComponentInChildren<MeshRenderer>());
                //colliders.Add(temp.transform.GetComponentInChildren<BoxCollider>());


            }
            else
            {
                temp = Instantiate(walls[0], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()), walls[0].transform.rotation);
                temp.name = gridLoc.GetString() + "North";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
                vertWalls.Add(gridLoc, new GameObject[2] {temp, null});//null });
                //renderers.Add(temp.transform.GetComponentInChildren<MeshRenderer>());
                //colliders.Add(temp.transform.GetComponentInChildren<BoxCollider>());

                temp = Instantiate(walls[1], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()), walls[1].transform.rotation);
                temp.name = gridLoc.GetString() + "South";
                temp.transform.parent = parent.transform;
                possWalls.Add(temp);
                vertWalls[gridLoc][1] = temp;

                //renderers.Add(temp.transform.GetComponentInChildren<MeshRenderer>());
                //colliders.Add(temp.transform.GetComponentInChildren<BoxCollider>());
            }
        }
    }

    public void DisableWalls()
    {
        wallActivated = false;
        foreach (GameObject go in possWalls)
        {
            go.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            go.SetActive(false);
        }
    }

    private void Activate(GameObject wall)
    {
        wall.SetActive(true);
        wall.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        wallActivated = true;
        enabled = false;
        
    }

    private void ActivateWalls()
    {
        playedSound = false;
        GridLocation startLocation = ExperimentController.Instance.GetTrialInfo().start;

        foreach (GameObject go in possWalls)
        {
            float dist = Vector2.Distance(new Vector2(startLocation.GetX(), startLocation.GetY()),
                new Vector2(go.transform.position.x, go.transform.position.z));
            if (dist > 2.5f || (!(startLocation.GetY() == -(Constants.GRID_LENGTH / 2)) && !(startLocation.GetY() == Constants.GRID_LENGTH / 2)))
            {
                if(go.transform.position != new Vector3(startLocation.GetX(), 1f, startLocation.GetY())) go.SetActive(true);
            }
            else
            {
                if(dist > 2.5f || (go.transform.position.z != -(Constants.GRID_LENGTH / 2) && go.transform.position.z != Constants.GRID_LENGTH / 2))
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