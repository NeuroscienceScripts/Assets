using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    
    // Start is called before the first frame update
    void Start()
    {
        StartCanvas.enabled = true;
        UserCanvas.enabled = false;
        Maze.SetActive(false);
    }

    public void Startup()
    {
        subjectNo = Int32.Parse((string) subjectNum.text.ToString());
        if (subjectNo != 0)
        {
            StartCanvas.enabled = false;
            UserCanvas.enabled = true;
            Maze.SetActive(true);
            userText.text = "Hold left click to move forward.\n Use the scroll-wheel to change the mouse sensitivity.\nGet to the yellow finish line";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) & startTime==0.0f)
        {
            userText.text = "";
            startTime = Time.realtimeSinceStartup;
        }

    }

    private void OnCollisionEnter(Collision other)
    {
        userText.text = "Congrats!";
        timeTaken = Time.realtimeSinceStartup - startTime;
        Time.timeScale = 0;
    }
}
