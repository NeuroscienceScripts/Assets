using System;
using Classes;
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

        public string controllerSelection;
        public GridLocation currentNode; 

        private FileHandler fileHandler = new FileHandler();
        private string lastNodePosition = "";

        public bool wallActivated = false;
        public string currWall = "";

        public void OnTriggerEnter(Collider other)
        {
            if (other.name.Length == 2 & lastNodePosition!=other.name & ExperimentController.Instance.recordCameraAndNodes)
            {
                currentNode = new GridLocation("" + other.name[0], int.Parse("" + other.name[1]));
                fileHandler.AppendLine((ExperimentController.Instance.subjectFile).Replace(".csv", "_nodePath.csv"),
                    other.name);
                lastNodePosition = other.name;
                ExperimentController.Instance.retraceNodes++;
            }
            else if(other.name.Length>2 & !other.name.Contains("Cube"))
                controllerSelection = other.name;
            if (other.gameObject.tag == "Wall")
            {
                    wallActivated = true;
                    currWall = other.transform.parent.gameObject.name;
            }
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