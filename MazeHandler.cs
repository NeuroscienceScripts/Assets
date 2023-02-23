using System;
using System.Collections;
using System.Collections.Generic;
using Classes;
using TMPro;
using UnityEngine;
using VR;

public class MazeHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userText;
    [SerializeField] public Canvas StartCanvas;
    [SerializeField] public Canvas UserCanvas;
    [SerializeField] public GameObject Maze;
    [SerializeField] public float startTime = 0.0f;
    [SerializeField] public float timeTaken = 0.0f;
    [SerializeField] private TMP_InputField subjectNum;
    [SerializeField] public int subjectNo;
    [SerializeField] public GameObject player; 
    [SerializeField] private GameObject moveForwardArrow;
    public int stepInPhase = 0;
    
    private Vector3[] arrowPath =
    {
        // x location, z location, rotation (east=0, south=90, west=180, north=270)
        new Vector3(0.0f, 2.5f, -90.0f),
        new Vector3(0.0f, 7.5f, -180.0f),
        new Vector3(-6.0f, 7.5f, -180.0f),
        new Vector3(-13.0f, 7.5f, 90.0f),
        new Vector3(-13.0f, 0.0f, 90.0f),
        new Vector3(-13.0f, -7.0f, 0.0f),
        new Vector3(-6.0f, -7.0f, 0.0f),
        new Vector3(0.0f, -7.0f, -90.0f)
    };
    
    // Start is called before the first frame update
    void Start()
    {
        StartCanvas.enabled = true;
        UserCanvas.enabled = false;
        Maze.SetActive(false);
    }

    public void Startup()
    {
        float arrowHeight = moveForwardArrow.transform.position.y;
        player.GetComponent<SimpleFirstPersonMovement>().active = false;
        subjectNo = Int32.Parse((string) subjectNum.text.ToString());
        if (subjectNo != 0)
        {
            StartCanvas.enabled = false;
            UserCanvas.enabled = true;
            Maze.SetActive(true);
            moveForwardArrow.SetActive(true);
            moveForwardArrow.transform.position =
                new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
            moveForwardArrow.transform.rotation = Quaternion.Euler(
                moveForwardArrow.transform.rotation.eulerAngles.x,
                arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);
            player.transform.localRotation = Quaternion.Euler(0,0,0);
            player.GetComponent<SimpleFirstPersonMovement>().active = true;
            userText.text = "Left-click mouse to move forward.\n move mouse to left to turn left.\n move mouse forward to look up.\nUse the scroll-wheel to change the mouse sensitivity.";
        }
    }

    private void Update()
    {
        float arrowHeight = moveForwardArrow.transform.position.y;
        if (stepInPhase < arrowPath.Length)
        {
            if (Input.GetMouseButton(0) & startTime==0.0f)
            {
                startTime = Time.realtimeSinceStartup;
            }
            
            if ((Mathf.Abs(player.transform.position.x - moveForwardArrow.transform.position.x) < 0.5f) && (Mathf.Abs(player.transform.position.z - moveForwardArrow.transform.position.z) < 0.5f))
            {
                stepInPhase++;
                moveForwardArrow.transform.position =
                    new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
                moveForwardArrow.transform.rotation = Quaternion.Euler(
                    moveForwardArrow.transform.rotation.eulerAngles.x,
                    arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);
            }
        }
        else
        {
            stepInPhase = 0;
            moveForwardArrow.transform.position =
                new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
            moveForwardArrow.transform.rotation = Quaternion.Euler(
                moveForwardArrow.transform.rotation.eulerAngles.x,
                arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);
        }
        
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     float arrowHeight = moveForwardArrow.transform.position.y;
    //     userText.text = "Congrats!";
    //     stepInPhase = 0;
    //     moveForwardArrow.transform.position =
    //         new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
    //     moveForwardArrow.transform.rotation = Quaternion.Euler(
    //         moveForwardArrow.transform.rotation.eulerAngles.x,
    //         arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);
    //     timeTaken = Time.realtimeSinceStartup - startTime;
    //     // Time.timeScale = 0;
    // }
}
