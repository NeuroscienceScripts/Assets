using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class PositionReplay : MonoBehaviour
{

    [SerializeField] private TMP_InputField fileInput;
    [SerializeField] private TMP_InputField subjectInput;
    [SerializeField] private GameObject startCanvas;
    private string defaultPath;
    private string filePath;
    private string camPos = "cameraPos.csv";
    private int subjectNum = 0;

    private void Awake()
    {
        defaultPath = Application.dataPath + @"\Data\";
        camPos = "cameraPos.csv";
    }

    public void StartReplay()
    {
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        subjectNum = int.Parse(subjectInput.text);
        camPos = filePath + subjectNum + "_" + camPos;
        Debug.Log(camPos);
        if (!File.Exists(camPos)) {
            filePath = defaultPath;
            Debug.LogError("Invalid File Path or Missing Critical File: cameraPos.csv"); 
        }
        else StartCoroutine(Replay());
    }

    private IEnumerator Replay()
    {
        startCanvas.SetActive(false);
        StreamReader sr = new StreamReader(camPos);
        sr.ReadLine();
        string[] line = sr.ReadLine().Split(',');
        Process(0f, line);
        float prevTime = float.Parse(line[1]);
        while (!sr.EndOfStream)
        {
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            yield return ProcessLine(float.Parse(line[1]) - prevTime, line);
            prevTime = float.Parse(line[1]);
        }
        yield return null;
        startCanvas.SetActive(true);
    }

    private void Process(float time, string[] line)
    {
        Vector3 newPos = new Vector3(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        transform.position = Vector3.Lerp(transform.position, newPos, time);
    }

    private IEnumerator ProcessLine(float time, string[] line)
    {
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 newPos = new Vector3(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        while(timeElapsed <= time)
        {
            transform.position = Vector3.Lerp(startPos, newPos, timeElapsed/time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = newPos;
    }
}
