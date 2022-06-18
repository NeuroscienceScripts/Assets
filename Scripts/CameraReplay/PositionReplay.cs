using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Classes;

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

    private List<long> positions;
    private int currentPos;
    private bool processing;

    private string[][] wallPositions;
    [SerializeField] private Transform[] walls;
    private Vector3[] wallDirections;
    [SerializeField] private GameObject wall;
    private bool hidden;
    private Dictionary<int, int> wallSpawns;

    private void Awake()
    {
        positions = new();
        hidden = false;
        defaultPath = Application.dataPath + @"\Data\";
        camPos = "cameraPos.csv";
        wallDirections = new Vector3[4];
        for(int i = 0; i < 4; ++i)
        {
            wallDirections[i] = walls[i].GetChild(0).transform.position;
        }
        HideWall();
    }

    public void StartReplay()
    {
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        subjectNum = int.Parse(subjectInput.text);
        camPos = filePath + subjectNum + "_" + camPos;
        if (!File.Exists(camPos))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: cameraPos.csv");
        }
        else
        {
            GetWallPositions();
            StartCoroutine(Replay());
        }
    }

    private void GetWallPositions()
    {
        wallPositions = new string[Constants.NUM_OF_TRIALS][];
        sr = new StreamReader(filePath + subjectNum + ".csv");
        sr.ReadLine();
        while (!sr.EndOfStream)
        {
            string[] line = sr.ReadLine().Split(',');
            int trialNum = int.Parse(line[0]);
            string[] pos = line[8] != "N/A" ? new string[] { line[8].Substring(0, 2), line[8][2..] } : new string[] { "N/A", "N/A" };
            wallPositions[trialNum] = pos;
        }
        sr.Close();
    }

    private IEnumerator Replay()
    {
        currentPos = 0;
        processing = false;
        paused = false;
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
        Stop();
    }

    private void ShowWall(int trialNum)
    {
        if (!hidden) return;
        wall.SetActive(true);
        hidden = false;
        switch (wallPositions[trialNum][1])
        {
            case "North":
                wall.transform.position = wallDirections[0];
                wall.transform.localScale = new Vector3(1.5f, 10f, 0.1f);
                break;
            case "South":
                wall.transform.position = wallDirections[1];
                wall.transform.localScale = new Vector3(1.5f, 10f, 0.1f);
                break;
            case "East":
                wall.transform.position = wallDirections[2];
                wall.transform.localScale = new Vector3(0.1f, 10f, 1.5f);
                break;
            case "West":
                wall.transform.position = wallDirections[3];
                wall.transform.localScale = new Vector3(0.1f, 10f, 1.5f);
                break;
            default:
                HideWall();
                break;
        }
        GridLocation gl = new(wallPositions[trialNum][0][0].ToString(), int.Parse(wallPositions[trialNum][0][1].ToString()) );
        wall.transform.position = new Vector3(wall.transform.position.x + gl.GetX(), wall.transform.position.y, wall.transform.position.z + gl.GetY());
    }

    private void HideWall()
    {
        if (hidden) return;
        wall.SetActive(false);
        hidden = true;
    }

    private void Process(string[] line)
    {
        if (!int.TryParse(line[0], out int x)) return;
        Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        transform.position = newPos;
        CheckWall(x);
    }

    private IEnumerator ProcessLine(float time, string[] line)
    {
        if (int.TryParse(line[0], out int x))
        {
            float timeElapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
            while (timeElapsed <= time)
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
                transform.position = Vector3.Lerp(startPos, newPos, timeElapsed / time);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = newPos;
            CheckWall(x);
        }
        processing = false;
    }

    private void CheckWall(int trialNum)
    {
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), wallDirections[trialNum]) <= 0.3f)
        {
            ShowWall(trialNum);
            if (!wallSpawns.ContainsKey(trialNum))
            {
                wallSpawns.Add(trialNum, currentPos);
            }

        }
        if (!wallSpawns.ContainsKey(trialNum))
        {
            HideWall();
            return;
        }
        if(currentPos < wallSpawns[trialNum])
        {
            HideWall();
        }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Stop();
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
        if(currentPos >= positions.Count) positions.Add(sr.GetPosition());
        currentPos++;
        Process(sr.ReadLine().Split(','));
    }

    private void Stop()
    {
        StopAllCoroutines();
        sr.Close();
        HideWall();
        filePath = @defaultPath;
        startCanvas.SetActive(true);
        camPos = "cameraPos.csv";
        positions.Clear();
        wallSpawns.Clear();
        processing = false;
        paused = false;
        currentPos = 0;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
