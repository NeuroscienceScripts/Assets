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
    [SerializeField] private TextMeshProUGUI trialDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private GameObject infoCanvas;

    [SerializeField] private FogReplay fog;
    [SerializeField] private bool fogToggle;

    [SerializeField] private Material shiftShaderMaterial;
    private Vector3 gazeVector;
    private bool started;

    private string defaultPath;
    private string filePath;
    private string camPos = "cameraPos.csv";

    private int subjectNum = 0;

    private bool paused;

    private List<long> positions;
    private int currentPos;
    private bool processing;
    private bool stepped;

    float prevTime;
    int prevTrialNum;

    private string[][] wallPositions;
    [SerializeField] private Transform[] walls;
    private Vector3[] wallDirections;
    [SerializeField] private GameObject wall;
    private bool hidden;
    private Dictionary<int, int> wallSpawns;
    private Dictionary<int, string> stressTrials;

    [SerializeField] private GameObject playerModel;

    private void Awake()
    {
        started = false;
        positions = new();
        wallSpawns = new();
        stressTrials = new();
        hidden = false;
        stepped = false;
        defaultPath = Application.dataPath + @"\Data\";
        camPos = "cameraPos.csv";
        wallDirections = new Vector3[26];
        for(int i = 0; i < 4; ++i)
        {
            wallDirections[i] = walls[i].GetChild(0).transform.position;
        }
        trialDisplay.text = "Trial: 0";
        infoCanvas.SetActive(false);
        fog.Off();
        HideWall();
    }

    public void StartReplay()
    {
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        if (filePath[^1] != '/')
        {
            filePath += @"/";
        }
        subjectNum = int.Parse(subjectInput.text);
        camPos = filePath + subjectNum + "_" + camPos;
        if (!File.Exists(camPos))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: cameraPos.csv");
        }
        else if (!File.Exists(filePath + subjectNum + ".csv"))
        {
            Stop();
            Debug.LogError($"Invalid File Path or Missing Critical File: {subjectNum}.csv");
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
        string s = sr.ReadLine();
        while (!sr.EndOfStream)
        {
            string[] line = sr.ReadLine().Split(',');
            int trialNum = int.Parse(line[0]);
            string[] pos = !line[8].Contains("N/A") ? new string[] { line[8].Substring(0, 2), line[8][2..] } : new string[] { "N/A", "N/A" };
            wallPositions[trialNum] = pos;
            if(line.Length > 9)
            {
                stressTrials[trialNum] = line[9];
            }
            else
            {
                stressTrials[trialNum] = "False";
            }
        }
        sr.Close();
    }

    private IEnumerator Replay()
    {
        currentPos = 0;
        processing = false;
        paused = false;
        startCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        sr = new StreamReader(camPos);
        string[] line;
        prevTime = 0;
        prevTrialNum = -1;
        while (!sr.EndOfStream)
        {
            started = true;
            currentPos++;
            if(currentPos >= positions.Count) positions.Add(sr.GetPosition());
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            if (prevTrialNum != x)
            {
                float timeElapsed = 0f;
                while (timeElapsed <= 1f)
                {
                    yield return null;
                    timeElapsed += Time.deltaTime;
                    if (paused || !processing) break;
                }
                trialDisplay.text = $"Trial: {x}";
                prevTrialNum = x;
                prevTime = 0f;
                if(stressTrials[x] == "True" && fogToggle)
                {
                    fog.On();
                }
                else
                {
                    fog.Off();
                }
            }
            processing = true;
            float currentTime = float.Parse(line[1]);
            yield return ProcessLine(currentTime - prevTime, line);
            while (paused) yield return null;
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
        GridLocation gl = new( wallPositions[trialNum][0][0].ToString(), int.Parse(wallPositions[trialNum][0][1].ToString()) );
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
        if(x != prevTrialNum)
        {
            prevTrialNum = x;
            trialDisplay.text = $"Trial: {prevTrialNum}";
            if (stressTrials[x] == "True" && fogToggle)
            {
                fog.On();
            }
            else
            {
                fog.Off();
            }
        }
        Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        transform.position = newPos;
        playerModel.transform.localScale = new Vector3(0.3f, float.Parse(line[8]), 0.3f);
        playerModel.transform.localPosition = new Vector3(0, float.Parse(line[8])/ -2f, 0);
        prevTime = float.Parse(line[1]);
        CheckWall(x);
    }

    private IEnumerator ProcessLine(float time, string[] line)
    {
        if (int.TryParse(line[0], out int x))
        {
            float timeElapsed = 0;
            Vector3 startPos = transform.position;
            Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
            Vector3 playerScale = new(0.3f, float.Parse(line[8]), 0.3f);
            while (timeElapsed <= time && time != 0)
            {
                while (paused)
                {
                    if (!processing)
                    {
                        yield break;
                    }
                    yield return null;
                }
                if (!processing)
                {
                    transform.position = newPos;
                    if (stepped) StepBackward();
                    stepped = false;
                    yield break;
                }
                transform.position = Vector3.Lerp(startPos, newPos, timeElapsed / time);
                playerModel.transform.localScale = Vector3.Lerp(playerModel.transform.localScale, playerScale, timeElapsed / time);
                playerModel.transform.localPosition = new Vector3(0, playerModel.transform.localScale.y / -2f, 0);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.position = newPos;
            playerModel.transform.localScale = playerScale;
            playerModel.transform.localPosition = new Vector3(0, playerModel.transform.localScale.y / -2f, 0);
            CheckWall(x);
            if (Mathf.Abs(float.Parse(line[1]) - prevTime) <= time * 1.1f)
            {
                prevTime = float.Parse(line[1]);
            }
        }
        processing = false;
    }

    private void CheckWall(int trialNum)
    {
        if (wallPositions[trialNum][1].Contains("N/A"))
        {
            HideWall();
            return;
        }
        GridLocation gl = new(wallPositions[trialNum][0][0].ToString(), int.Parse(wallPositions[trialNum][0][1].ToString()));
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(gl.GetX(), gl.GetY())) <= 0.3f)
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
        else
        {
            ShowWall(trialNum);
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
            if (!paused) StepBackward();
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
        if (Input.GetKeyDown(KeyCode.H))
        {
            infoCanvas.SetActive(!infoCanvas.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            fogToggle = !fogToggle;
        }
        timeDisplay.text = $"Time: {prevTime:0.000}";
    }

    private void StepBackward()
    {
        if (currentPos == 0) return;
        paused = true;
        processing = false;
        sr.SetPosition(positions[currentPos - 1]);
        currentPos--;
        Process(sr.ReadLine().Split(','));
        stepped = true;
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
        started = false;
        if(sr != null) sr.Close();
        HideWall();
        filePath = @defaultPath;
        startCanvas.SetActive(true);
        camPos = "cameraPos.csv";
        positions.Clear();
        wallSpawns.Clear();
        stressTrials.Clear();
        processing = false;
        paused = false;
        currentPos = 0;
        prevTime = 0f;
        prevTrialNum = -1;
        stepped = false;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        playerModel.transform.localPosition = Vector3.zero;
    }

}
