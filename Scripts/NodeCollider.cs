using System;
using Classes;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Detects what object the participant/controller is in contact with,
    /// needs to be attached to the controller object 
    /// </summary>
    public class NodeCollider : MonoBehaviour
    {
        public static NodeCollider Instance { get; private set; }

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
            if (other.name.Length == 2 & lastNodePosition!=other.name & ExperimentController.Instance.recordCameraAndNodes & Mathf.Abs(Time.realtimeSinceStartup - previousTime) >= cooldownTime & !other.CompareTag("Wall"))
            {
                currentNode = new GridLocation("" + other.name[0], int.Parse("" + other.name[1]));
                Debug.Log(other.name);
                fileHandler.AppendLine((ExperimentController.Instance.subjectFile).Replace(ExperimentController.Instance.Date_time+".csv", "_nodePath.csv"),
                    other.name);
                lastNodePosition = other.name;
                ExperimentController.Instance.retraceNodes++;
                previousTime = Time.realtimeSinceStartup;
            }
            // else if(other.name.Length>2 & !other.name.Contains("Cube"))
            //     controllerSelection = other.name;
            // if (other.gameObject.tag == "Wall" & ExperimentController.Instance.stepInPhase == 3)
            // {
            //         wallActivated = true;
            //         currWall = other.transform.parent.gameObject.name;
            // } else
            // {
            //     wallActivated = false;
            //     currWall = "";
            // }
        }

        public void OnTriggerExit(Collider other)
        {
            if(other.name.Length>2 & !other.name.Contains("Cube"))
                controllerSelection = "Not_Selected"; 
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