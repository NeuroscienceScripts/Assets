using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using DefaultNamespace;
using TMPro;

namespace DynamicBlocking
{
    public class DynamicBlock : MonoBehaviour
    {
        private FileHandler fileHandler = new FileHandler();
        [SerializeField] private WallInfoObject wallInfo;
        [SerializeField] private WallInfoObject learningWallInfo;
        private GameObject temp;

        [SerializeField] private GameObject[] walls; // 0 is north, 1 is south, 2 is east, 3 is west

        [SerializeField] private List<GameObject> possWalls;

        [SerializeField] private List<GameObject> learningWalls;
        //[SerializeField] private List<MeshRenderer> renderers;
        //[SerializeField] private List<BoxCollider> colliders;

        [SerializeField] private AudioSource wallSound;
        private bool playedSound = false;

        private GridLocation prevNode;
        // Key-value pairs 
        private Dictionary<GridLocation, GameObject[]> horizWalls;
        private Dictionary<GridLocation, GameObject[]> vertWalls;

        public bool wallActivated;
        public string testWall;
        private float startTime;


        public event System.Action onWallActivated;

        private void OnEnable()
        {
            int num = ExperimentController.Instance.subjectNumber % 2;
            Trial t = ExperimentController.Instance.GetTrialInfo();
            // Testing phase 
            if (ExperimentController.Instance.phase == 3)
            {
                // If not stress trial OR odd & is wall trial OR even and not wall trial 
                if (!t.stressTrial || (num != 0 && t.isWallTrial) || (num == 0 && !t.isWallTrial))
                {
                    // Wall NOT activated 
                    enabled = false;
                    return;
                }
            }
            // Learning phase 
            if (ExperimentController.Instance.phase == 1)
            {
                ActivateWalls(testWall);
            }
            else ActivateWalls();
        }

        private void Awake()
        {
            vertWalls = new();
            horizWalls = new();
            possWalls = new();
            learningWalls = new();
            //renderers = new();
            //colliders = new();
            // Default no wall 
            wallActivated = false;
            InstantiateWalls();
            DisableWalls();
            enabled = false;
            playedSound = false;
            // Learning phase
            if (ExperimentController.Instance.phase == 1)
            {
                //Data 
                fileHandler.AppendLine(
                    ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                        "_learning.csv"), "Lap, Wall, Time");
            }
        }

        private void OnDisable()
        {
            temp = null;
            wallActivated = false;
            //if(ControllerCollider.Instance != null)
            //{
            //    ControllerCollider.Instance.wallActivated = false;
            //    ControllerCollider.Instance.currWall = "";
            //}

        }

        //[SerializeField] private TextMeshProUGUI nodeOut;

        private void Update()
        {
            // If wall is not activated AND component is enabled 
            if (!wallActivated && enabled)
            {
                // Retrieves player's current node 
                GridLocation currentNode =
                    NodeExtension.CurrentNode(ExperimentController.Instance.player.transform.position);
                Debug.Log(currentNode);
                //nodeOut.text = currentNode.GetString();
                // If player move to different node
                if (prevNode != null && currentNode != null && currentNode != prevNode)
                {
                    // Player moved to node with horizontal walls 
                    if (horizWalls.ContainsKey(currentNode))
                    {
                        // Depending on player's past and present node, calls appropiate wall 
                        if (prevNode.GetX() > currentNode.GetX())
                        {
                            // larger to smaller
                            Activate(horizWalls[currentNode][1]);
                        }
                        else
                        {
                            Activate(horizWalls[currentNode][0]);
                        }
                    }
                    // Player moved to node with vertical walls 
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
                // Update player node
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
            // Automatically iterates through a collection w/out need for index management 
            foreach (WallPosition wp in wallInfo.wallPositions)
            {
                //Converts position to grid location
                gridLoc = wp.position.ToGridLocation();
                
                // If wall's direction is horizontal 
                if (wp.direction == WallDirection.Horizontal)
                {
                    temp = Instantiate(walls[2], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[2].transform.rotation);
                    temp.name = gridLoc.GetString() + "East";
                    temp.transform.parent = parent.transform;
                    possWalls.Add(temp);
                    horizWalls.Add(gridLoc, new GameObject[2] {temp, null});

                    temp = Instantiate(walls[3], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[3].transform.rotation);
                    temp.name = gridLoc.GetString() + "West";
                    temp.transform.parent = parent.transform;
                    possWalls.Add(temp);
                    horizWalls[gridLoc][1] = temp;
                }
                // If wall's direction is vertical 
                else
                {
                    temp = Instantiate(walls[0], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[0].transform.rotation);
                    temp.name = gridLoc.GetString() + "North";
                    temp.transform.parent = parent.transform;
                    possWalls.Add(temp);
                    vertWalls.Add(gridLoc, new GameObject[2] {temp, null});

                    temp = Instantiate(walls[1], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[1].transform.rotation);
                    temp.name = gridLoc.GetString() + "South";
                    temp.transform.parent = parent.transform;
                    possWalls.Add(temp);
                    vertWalls[gridLoc][1] = temp;
                }
            }
            // For learning phase?? [Delete]
            foreach (WallPosition wp in learningWallInfo.wallPositions)
            {
                gridLoc = wp.position.ToGridLocation();
                if (wp.direction == WallDirection.Horizontal)
                {
                    if (horizWalls.ContainsKey(gridLoc)) continue;
                    temp = Instantiate(walls[2], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[2].transform.rotation);
                    temp.name = gridLoc.GetString() + "East";
                    temp.transform.parent = parent.transform;
                    learningWalls.Add(temp);
                    horizWalls.Add(gridLoc, new GameObject[2] {temp, null});

                    temp = Instantiate(walls[3], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[3].transform.rotation);
                    temp.name = gridLoc.GetString() + "West";
                    temp.transform.parent = parent.transform;
                    learningWalls.Add(temp);
                    horizWalls[gridLoc][1] = temp;
                }
                else
                {
                    if (vertWalls.ContainsKey(gridLoc)) continue;
                    temp = Instantiate(walls[0], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[0].transform.rotation);
                    temp.name = gridLoc.GetString() + "North";
                    temp.transform.parent = parent.transform;
                    learningWalls.Add(temp);
                    vertWalls.Add(gridLoc, new GameObject[2] {temp, null});

                    temp = Instantiate(walls[1], new Vector3(gridLoc.GetX(), 1, gridLoc.GetY()),
                        walls[1].transform.rotation);
                    temp.name = gridLoc.GetString() + "South";
                    temp.transform.parent = parent.transform;
                    learningWalls.Add(temp);
                    vertWalls[gridLoc][1] = temp;
                }
            }
        }

        public void DisableWalls()
        {
            foreach (GameObject go in possWalls)
            {
                // Gets first child of each GameObject, makes wall transparent & doesn't restrict pathway
                go.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                go.transform.GetChild(0).GetComponent<Collider>().isTrigger = true;
                go.SetActive(false);
            }
            foreach (GameObject go in learningWalls)
            {
                go.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                go.transform.GetChild(0).GetComponent<Collider>().isTrigger = true;
                go.SetActive(false);
            }
        }

        private void Activate(GameObject wall)
        {
            // If wall object not currently active
            if (!wall.activeSelf) return;
            // Play wall sound 
            if (!wallSound.isPlaying & !playedSound)
            {
                wallSound.Play();
                playedSound = true;
            }
            // Wall visible & can physically collide with player
            wall.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            wallActivated = true;
            wall.transform.GetChild(0).GetComponent<Collider>().isTrigger = false;
            ExperimentController.Instance.blockedWall = wall.name;
            // If learning phase and not start trials yet OR on third trial 
            if ((ExperimentController.Instance.phase==1)&((ExperimentController.Instance.currentTrial == 0) || (ExperimentController.Instance.currentTrial == 3)))
            {
                startTime = ExperimentController.Instance.trialStartTime;
            }
            if (ExperimentController.Instance.phase == 1)
            {
                fileHandler.AppendLine(
                    ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                        "_learning.csv"), ExperimentController.Instance.currentTrial + "," + wall.name + "," +
                                          (Time.realtimeSinceStartup - startTime));
            }
            onWallActivated?.Invoke();
            enabled = false;
        }

        private void ActivateWalls()
        {
            playedSound = false;
            GridLocation startLocation = ExperimentController.Instance.GetTrialInfo().start;
            print("activate");
            foreach (GameObject go in possWalls)
            {
                // Calculates distance between start & current location of player
                float dist = Vector2.Distance(new Vector2(startLocation.GetX(), startLocation.GetY()),
                    new Vector2(go.transform.position.x, go.transform.position.z));
                // CONFUSED 
                // If distance greater than 2.5 units OR start location's y coord is not equal to negative half of grid length and not equal to half of grid length
                if (dist > 2.5f || (!(startLocation.GetY() == -(Constants.GRID_LENGTH / 2)) &&
                                    !(startLocation.GetY() == Constants.GRID_LENGTH / 2)))
                {
                    // If position of player is NOT equal to x and y of start location & y-coord is 1, wall is VISIBLE 
                    if (go.transform.position != new Vector3(startLocation.GetX(), 1f, startLocation.GetY()))
                        go.SetActive(true);
                }
                else
                {
                    // If distance is greater than 2.5 units OR z coord is NOT equal to ___
                    if (dist > 2.5f || (go.transform.position.z != -(Constants.GRID_LENGTH / 2) &&
                                        go.transform.position.z != Constants.GRID_LENGTH / 2))
                    {
                        if (go.transform.position != new Vector3(startLocation.GetX(), 1f, startLocation.GetY()))
                            go.SetActive(true);
                    }
                }
            }
        }

        private void ActivateWalls(string wallName)
        {
            playedSound = false;
            foreach (GameObject go in learningWalls)
            {
                if (go.name.Contains(wallName))
                {
                    go.SetActive(true);
                }
            }

            foreach (GameObject go in possWalls)
            {
                if(go.name.Contains(wallName)) go.SetActive(true);
            }
        }
    }
}