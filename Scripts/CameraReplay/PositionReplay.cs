using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class PositionReplay : MonoBehaviour
{

    private StreamReader sr;

    [SerializeField] private TMP_InputField fileInput;
    [SerializeField] private TMP_InputField subjectInput;
    [SerializeField] private GameObject startCanvas;
    private string defaultPath;
    private string filePath;
    private string camPos = "cameraPos.csv";

    private int subjectNum = 0;

    private bool paused;

    [SerializeField] private List<long> positions;
    [SerializeField] private int currentPos;
    private bool processing;
    

    private void Awake()
    {
        positions = new();
        defaultPath = Application.dataPath + @"\Data\";
        camPos = "cameraPos.csv";
    }

    public void StartReplay()
    {
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        subjectNum = int.Parse(subjectInput.text);
        camPos = filePath + subjectNum + "_" + camPos;
        if (!File.Exists(camPos)) {
            filePath = defaultPath;
            Debug.LogError("Invalid File Path or Missing Critical File: cameraPos.csv"); 
        }
        else StartCoroutine(Replay());
    }

    private IEnumerator Replay()
    {
        currentPos = 0;
        processing = false;
        startCanvas.SetActive(false);
        sr = new StreamReader(camPos);
        sr.ReadLine();
        positions.Add(sr.GetPosition());
        string[] line = sr.ReadLine().Split(',');
        currentPos++;
        Process(line);
        float prevTime = float.Parse(line[1]);
        while (!sr.EndOfStream)
        {
            while (paused) yield return null;
            currentPos++;
            if(currentPos >= positions.Count) positions.Add(sr.GetPosition());
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            processing = true;
            yield return ProcessLine(float.Parse(line[1]) - prevTime, line);
            prevTime = float.Parse(line[1]);
        }
        yield return null;
        startCanvas.SetActive(true);
        camPos = "cameraPos.csv";
        positions.Clear();
    }

    private void Process(string[] line)
    {
        Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        transform.position = newPos;
    }


    private IEnumerator ProcessLine(float time, string[] line)
    {
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        while(timeElapsed <= time)
        {
            if (!processing)
            {
                transform.position = newPos;
                yield break;
            }
            while (paused) 
            {
                yield return null; 
            }
            transform.position = Vector3.Lerp(startPos, newPos, timeElapsed/time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = newPos;
        processing = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StepBackward();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StepForward();
        }
    }

    private void StepBackward()
    {
        if (currentPos == 0) return;
        paused = true;
        processing = false;
        sr.SetPosition(positions[currentPos - 1]);
        currentPos--;
        Process(sr.ReadLine().Split(','));
    }

    private void StepForward()
    {
        if (sr.EndOfStream) return;
        paused = true;
        processing = false;
        sr.ReadLine();
        if(currentPos >= positions.Count) positions.Add(sr.GetPosition());
        currentPos++;
        Process(sr.ReadLine().Split(','));
    }
}
