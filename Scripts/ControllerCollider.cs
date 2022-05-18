using System;
using Classes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Detects what object the participant/controller is in contact with,
    /// needs to be attached to the controller object 
    /// </summary>
    public class ControllerCollider : MonoBehaviour
    {
        public static ControllerCollider Instance { get; private set; }

        [SerializeField] private Transform controller;

        public string controllerSelection;
        public GridLocation currentNode; 

        private FileHandler fileHandler = new FileHandler();
        private string lastNodePosition = "";

        public bool wallActivated = false;
        public string currWall = "";
        
        private float cooldownTime = 0.25f;
        private float previousTime = 0f;

        public void OnTriggerEnter(Collider other)
        {
            if (other.name.Length == 2 & lastNodePosition == "")
                lastNodePosition = other.name; 
                
            if (other.name.Length == 2 & !lastNodePosition.Equals(other.name) & ExperimentController.Instance.recordCameraAndNodes & Time.realtimeSinceStartup - previousTime >= cooldownTime)
            {
                currentNode = new GridLocation("" + other.name[0], int.Parse("" + other.name[1]));
                fileHandler.AppendLine((ExperimentController.Instance.subjectFile).Replace(ExperimentController.Instance.Date_time+".csv", "_nodePath.csv"),
                    other.name);
                lastNodePosition = other.name;
                ExperimentController.Instance.retraceNodes++;
                previousTime = Time.realtimeSinceStartup;
            }
            else if(other.name.Length>2 & !other.name.Contains("Cube") & !other.CompareTag("Wall") & !other.name.Contains("Floor"))
                controllerSelection = other.name;
            
            
            if (other.CompareTag("Wall"))
            {
                if (other.GetComponent<MeshRenderer>().enabled & wallActivated)
                {
                    // Ran into wall
                    // add variable that will be true if this occurs, and check on Line 560 of ExperimentController for this and print to file
                }
                wallActivated = true;
                currWall = other.transform.parent.gameObject.name;
            } else
            {
                wallActivated = false;
                currWall = "";
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if(other.name.Length>2 & !other.name.Contains("Cube") & !other.tag.Contains("Wall"))
                controllerSelection = "Not_Selected"; 
        }

        /// <summary>
        /// Checks if the next node triggered makes sense... Don't want to count nodes when the headset tracking gets lost
        /// </summary>
        /// <returns></returns>
        private bool CheckValidNodeSwitch(string lastNode, string nextNode)
        {
            if (!lastNode.Equals(nextNode) & lastNode.Length==2 & nextNode.Length==2)
            {
                for(int i=0; i<Constants.LETTERS.Length; i++)
                {
                    if (lastNode[0].ToString() == Constants.LETTERS[i])
                    {
                        if (nextNode[0].ToString() ==
                            Constants.LETTERS[Mathf.Clamp(i - 1, 0, Constants.LETTERS.Length - 1)]
                            || nextNode[0].ToString() ==
                            Constants.LETTERS[Mathf.Clamp(i + 1, 0, Constants.LETTERS.Length - 1)])
                        {
                            int currentNodeNumber = int.Parse(lastNode[1].ToString());
                            if (int.Parse(nextNode[1].ToString()) >= Mathf.Clamp(currentNodeNumber - 1, 0, 10 )
                                & int.Parse(nextNode[1].ToString()) <= Mathf.Clamp(currentNodeNumber + 1, 0, 10))
                                return true; 
                        }
                    }
                }
            }

            return false; 
        }

        public bool PaintingCheck()
        {
            Collider[] collisions = Physics.OverlapSphere(controller.position, 0.15f);
            foreach (var c in collisions)
            {
                if (!c.CompareTag("Painting")) continue;
                controllerSelection = c.name;
                return true;
            }

            return false;
        }

        //Following code will make instance of ControllerCollider persist between scenes
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

}