using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userText; 
    // Start is called before the first frame update
    void Start()
    {
        userText.text = "Get to the yellow finish line"; 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
            userText.text = ""; 
    }

    private void OnCollisionEnter(Collision other)
    {
        userText.text = "Congrats!";
        Time.timeScale = 0;
    }
}
