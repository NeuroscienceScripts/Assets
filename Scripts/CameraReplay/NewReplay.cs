using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Classes;
using System;
using Tobii.XR.GazeModifier;

public class NewReplay : MonoBehaviour
{
    private StreamReader sr;

    [SerializeField] private TMP_InputField fileInput;
    [SerializeField] private TMP_InputField subjectInput;
    [SerializeField] private TMP_InputField recordListInput;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private TextMeshProUGUI trialDisplay;
    [SerializeField] private TextMeshProUGUI timeDisplay;
    [SerializeField] private GameObject infoCanvas;

    [SerializeField] private FogReplay fog;
    [SerializeField] private bool fogToggle;
    private bool fogOn;

    [SerializeField] private Material shiftShaderMaterial;
    [SerializeField] private Vector3 gazeVector;
    private bool started;
    [SerializeField, Range(0.01f, 0.1f)] private float scotomaSize;
    public float averageEyeMovementMagnitude;
    public float averageMovementWallBlock;
    public float rawEyeMagnitude;
    private int stepCount, wallStepCount;


    [SerializeField] private Camera firstPerson;
    [SerializeField] private Camera thirdPerson;
    private bool inFirstPerson;

    [SerializeField] private LineRenderer lineRender;
    [SerializeField] private float lineWidth;
    [SerializeField] private LayerMask wallLayerMask;

    private float maxLineLength;

    private string defaultPath;
    private string filePath;
    private string camInfo = "camera_tracker.csv";

    public int subjectNum = 0;

    public bool paused;

    private List<long> positions;
    private int currentPos;
    private bool processing;
    private bool stepped;

    float prevTime;
    int prevTrialNum;
    float[] timeInTrials;
    bool trialSpecific;
    float timeInTrial;
    int timeIndex;

    private string[][] wallPositions;
    [SerializeField] private Transform[] walls;
    private Vector3[] wallDirections;
    [SerializeField] private GameObject wall;
    private bool hidden;
    private Dictionary<int, int> wallSpawns;
    private Dictionary<int, string> stressTrials;


    [SerializeField] private GameObject playerModel;

    public event Action<int, float> OnTrialChanged;
    public event Action<int, float, float> OnNextFrame;
    public event Action<int, float> OnNextFrameLearning;
    private bool isRecordingGaze;
    private bool isLearningPhase;
    [SerializeField] private PaintingTracker paintingTracker;
    [SerializeField] private EyeDataTracker eyeDataTracker;

    private void Awake()
    {
        maxLineLength = 5f;
        positions = new();
        wallSpawns = new();
        stressTrials = new();
        started = false;
        hidden = false;
        stepped = false;
        fogOn = false;

        inFirstPerson = true;
        thirdPerson.enabled = false;
        firstPerson.enabled = true;

        defaultPath = Application.dataPath + @"\Data\";
        camInfo = "camera_tracker.csv";
        wallDirections = new Vector3[26];
        Vector3[] linePositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRender.SetPositions(linePositions);
        lineRender.startWidth = lineWidth;
        lineRender.endWidth = lineWidth;
        for (int i = 0; i < 4; ++i)
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
        timeIndex = 1;
        isLearningPhase = false;
        isRecordingGaze = false;
        trialSpecific = false;
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        if (filePath[^1] != '/')
        {
            filePath += @"/";
        }
        subjectNum = int.Parse(subjectInput.text);
        camInfo = filePath + subjectNum + "_" + camInfo;
        if (!File.Exists(camInfo))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: camera_tracker.csv");
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

    public void StartLearningReplay()
    {
        timeIndex = 1;
        isRecordingGaze = false;
        isLearningPhase = true;
        trialSpecific = false;
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        if (filePath[^1] != '/')
        {
            filePath += @"/";
        }
        subjectNum = int.Parse(subjectInput.text);
        camInfo = filePath + subjectNum + "_" + camInfo;
        if (!File.Exists(camInfo))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: camera_tracker.csv");
        }
        else
        {
            StartCoroutine(LearningReplay());
        }
    }

    public void StartReplay(int trial)
    {
        timeIndex = 1;
        isLearningPhase = false;
        isRecordingGaze = false;
        trialSpecific = true;
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        if (filePath[^1] != '/')
        {
            filePath += @"/";
        }
        subjectNum = int.Parse(subjectInput.text);
        camInfo = filePath + subjectNum + "_" + camInfo;
        if (!File.Exists(camInfo))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: camera_tracker.csv");
        }
        else if (!File.Exists(filePath + subjectNum + ".csv"))
        {
            Stop();
            Debug.LogError($"Invalid File Path or Missing Critical File: {subjectNum}.csv");
        }
        else
        {
            GetWallPositions();
            StartCoroutine(Replay(trial));
        }
    }

    public void StartRecordingReplay()
    {
        timeIndex = 1;
        isLearningPhase = false;
        trialSpecific = false;
        string recordPath = string.IsNullOrEmpty(recordListInput.text) || recordListInput.text == "" ? "Assets/subjectList.csv" : recordListInput.text;
        Debug.Log(recordPath);
        if (!File.Exists(recordPath))
        {
            Stop();
           Debug.LogError("Invalid File Path/Missing File");
        }
        else
        {
            StartCoroutine(RecordReplay(recordPath));
        }
    }
    public void StartLearnRecordingReplay()
    {
        timeIndex = 1;
        isLearningPhase = true;
        trialSpecific = false;
        string recordPath = string.IsNullOrEmpty(recordListInput.text) || recordListInput.text == "" ? "Assets/subjectList.csv" : recordListInput.text;
        Debug.Log(recordPath);
        if (!File.Exists(recordPath))
        {
            Stop();
            Debug.LogError("Invalid File Path/Missing File");
        }
        else
        {
            StartCoroutine(LearningRecordReplay(recordPath));
        }
    }

    private IEnumerator RecordReplay(string recordPath)
    {
        StreamReader s = new(recordPath);
        while (!s.EndOfStream)
        {
            isRecordingGaze = true;
            defaultPath = "Assets/ICB-camera-tracker/";
            filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
            if (filePath[^1] != '/')
            {
                filePath += @"/";
            }
            subjectNum = int.Parse(s.ReadLine());
            camInfo = filePath + subjectNum + "_" + camInfo;
            if (!File.Exists(camInfo))
            {
                RecordStop();
                startCanvas.SetActive(false);
                Debug.LogError("Invalid File Path or Missing Critical File:"  + subjectNum + " camera_tracker.csv");
                continue;
            }
            else if (!File.Exists(filePath + subjectNum + ".csv"))
            {
                RecordStop();
                startCanvas.SetActive(false);
                Debug.LogError($"Invalid File Path or Missing Critical File: {subjectNum}.csv");
                continue;
            }
            else
            {
                GetWallPositions();
                paintingTracker.StartRec();
                eyeDataTracker.StartRec();
                yield return Replay();
                startCanvas.SetActive(false);
            }
        }
        Stop();
    }

    private IEnumerator LearningRecordReplay(string recordPath)
    {
        StreamReader s = new(recordPath);
        while (!s.EndOfStream)
        {
            isRecordingGaze = true;
            defaultPath = "Assets/ICB-camera-tracker/";
            filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
            if (filePath[^1] != '/')
            {
                filePath += @"/";
            }
            subjectNum = int.Parse(s.ReadLine());
            camInfo = filePath + subjectNum + "_" + camInfo;
            if (!File.Exists(camInfo))
            {
                RecordStop();
                startCanvas.SetActive(false);
                Debug.LogError("Invalid File Path or Missing Critical File:" + subjectNum + " camera_tracker.csv");
                continue;
            }
            else
            {
                paintingTracker.StartRec();
                eyeDataTracker.StartRec();
                yield return LearningReplay();
                startCanvas.SetActive(false);
            }
        }
        Stop();
    }

    private void GetWallPositions()
    {
        wallPositions = new string[Constants.NUM_OF_TRIALS][];
        timeInTrials = new float[Constants.NUM_OF_TRIALS];
        sr = new StreamReader(filePath + subjectNum + ".csv");
        sr.ReadLine();
        while (!sr.EndOfStream)
        {
            string[] line = sr.ReadLine().Split(',');
            int trialNum = int.Parse(line[0]);
            string[] pos = !line[8].Contains("N/A") ? new string[] { line[8].Substring(0, 2), line[8][2..] } : new string[] { "N/A", "N/A" };
            wallPositions[trialNum] = pos;
            timeInTrials[trialNum] = float.Parse(line[1]);
            if (line.Length > 9)
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
        stepCount = 0;
        wallStepCount = 0;
        processing = false;
        paused = false;
        startCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        sr = new StreamReader(camInfo);
        string[] line;
        prevTime = 0;
        prevTrialNum = -1;
        while (!sr.EndOfStream)
        {
            started = true;
            currentPos++;
            if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
            stepCount++;
            if (!hidden) wallStepCount++;
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            
            if (prevTrialNum != x)
            {
                float timeElapsed = 0f;
                averageEyeMovementMagnitude /= stepCount;
                if (prevTrialNum >= 0 && wallPositions[prevTrialNum][0] == "N/A") averageMovementWallBlock = -1;
                else averageMovementWallBlock /= wallStepCount;
                stepCount = 0;
                wallStepCount = 0;
                if(prevTrialNum >= 0 && isRecordingGaze) OnTrialChanged?.Invoke(prevTrialNum, timeInTrials[prevTrialNum]);
                while (timeElapsed <= 0.5f)
                {
                    yield return null;
                    timeElapsed += Time.deltaTime;
                    if (paused || !processing) break;
                }
                averageEyeMovementMagnitude = 0;
                averageMovementWallBlock = 0;
                gazeVector = Vector3.forward;
                trialDisplay.text = $"Trial: {x}";
                prevTrialNum = x;
                prevTime = 0f;
                if (stressTrials[x] == "True" && fogToggle)
                {
                    fog.On();
                    fogOn = true;
                }
                else
                {
                    fog.Off();
                    fogOn = false;
                }
            }
            processing = true;
            float currentTime = float.Parse(line[1]);
            float dTime = currentTime - prevTime;
            yield return ProcessLine(dTime, line);
            if (isRecordingGaze) OnNextFrame?.Invoke(prevTrialNum, timeInTrials[prevTrialNum],currentTime);
            while (paused) yield return null;
        }
        yield return null;
        if (!isRecordingGaze) Stop();
        else 
        {
            averageEyeMovementMagnitude /= stepCount;
            if (prevTrialNum >= 0 && wallPositions[prevTrialNum][0] == "N/A") averageMovementWallBlock = -1;
            else averageMovementWallBlock /= wallStepCount;
            Debug.Log(prevTrialNum);
            Debug.Log(timeInTrials.Length);
            if(timeInTrials.Length>=prevTrialNum && prevTrialNum>=0) OnTrialChanged?.Invoke(prevTrialNum, timeInTrials[prevTrialNum]);
            RecordStop(); 
        }
    }

    private IEnumerator LearningReplay()
    {
        currentPos = 0;
        stepCount = 0;
        processing = false;
        paused = false;
        startCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        sr = new StreamReader(camInfo);
        string[] line;
        prevTime = 0;
        prevTrialNum = -1;
        while (!sr.EndOfStream)
        {
            started = true;
            currentPos++;
            if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
            stepCount++;
            if (!hidden) wallStepCount++;
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;

            if (prevTrialNum != x)
            {
                float timeElapsed = 0f;
                averageEyeMovementMagnitude /= stepCount;
                stepCount = 0;
                timeInTrial = float.Parse(line[1]);
                if (prevTrialNum >= 0 && isRecordingGaze) OnTrialChanged?.Invoke(prevTrialNum, timeInTrial);
                while (timeElapsed <= 0.5f)
                {
                    yield return null;
                    timeElapsed += Time.deltaTime;
                    if (paused || !processing) break;
                }
                averageEyeMovementMagnitude = 0;
                gazeVector = Vector3.forward;
                trialDisplay.text = $"Trial: {x}";
                prevTrialNum = x;
                prevTime = 0f;
                timeInTrial = 0f;
            }
            processing = true;
            float currentTime = float.Parse(line[1]);
            timeInTrial = currentTime;
            float dTime = currentTime - prevTime;
            yield return ProcessLine(dTime, line);
            if (isRecordingGaze) OnNextFrameLearning?.Invoke(prevTrialNum, currentTime);
            while (paused) yield return null;
        }
        yield return null;
        if (!isRecordingGaze) Stop();
        else
        {
            averageEyeMovementMagnitude /= stepCount;
            if (timeInTrials.Length >= prevTrialNum && prevTrialNum >= 0) OnTrialChanged?.Invoke(prevTrialNum, timeInTrial);
            RecordStop();
        }
    }

    private IEnumerator Replay(int trial)
    {
        currentPos = 0;
        processing = false;
        paused = false;
        startCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        sr = new StreamReader(camInfo);
        string[] line;
        int targetTrial = trial;
        prevTime = 0;
        prevTrialNum = -1;
        while (!sr.EndOfStream)
        {
            started = true;
            currentPos++;
            if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            if (targetTrial != x && prevTrialNum == -1) continue;

            if (targetTrial != x && prevTrialNum != -1)
            {
                break;
            }
            trialDisplay.text = $"Trial: {x}";
            prevTrialNum = x;
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
        GridLocation gl = new(wallPositions[trialNum][0][0].ToString(), int.Parse(wallPositions[trialNum][0][1].ToString()));
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
        if (x != prevTrialNum)
        {
            if (trialSpecific)
            {
                currentPos--;
                return;
            }
            prevTrialNum = x;
            trialDisplay.text = $"Trial: {prevTrialNum}";
            if (stressTrials[x] == "True" && fogToggle)
            {
                fog.On();
                fogOn = true;
            }
            else
            {
                fog.Off();
                fogOn = false;
            }
        }
        Quaternion newRot = Quaternion.Euler(new Vector3(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9])));
        Vector3 newPos = new(float.Parse(line[10]), float.Parse(line[11]), float.Parse(line[12]));
        Vector3 gaze = new(float.Parse(line[15]), float.Parse(line[16]), float.Parse(line[17]));
        transform.SetPositionAndRotation(newPos, newRot);
        playerModel.transform.localScale = new Vector3(0.3f, float.Parse(line[8]), 0.3f);
        playerModel.transform.localPosition = new Vector3(0, float.Parse(line[8]) / -2f, 0);
        float eyeMovement = Vector3.Distance(gazeVector, gaze);
        gazeVector = gaze;
        prevTime = float.Parse(line[timeIndex]);
        CheckWall(x);
        rawEyeMagnitude = eyeMovement;
        averageEyeMovementMagnitude += eyeMovement;
        if (!hidden)
            averageMovementWallBlock += eyeMovement;
        else
        {
            averageMovementWallBlock = 0.0f;
        }
    }

    private IEnumerator ProcessLine(float time, string[] line)
    {
        if (int.TryParse(line[0], out int x))
        {
            float timeElapsed = 0;
            Quaternion startRot = transform.rotation;
            Quaternion newRot = Quaternion.Euler(new Vector3(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9])));
            Vector3 startPos = transform.position;
            Vector3 newPos = new(float.Parse(line[10]), float.Parse(line[11]), float.Parse(line[12]));
            Vector3 gaze = new(float.Parse(line[15]), float.Parse(line[16]), float.Parse(line[17]));

            Vector3 playerScale = new(0.3f, float.Parse(line[8]), 0.3f);
            float eyeMovement = Vector3.Distance(gazeVector.normalized, gaze.normalized);
            
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
                    transform.rotation = newRot;
                    if (stepped) StepBackward();
                    stepped = false;
                    yield break;
                }
                transform.SetPositionAndRotation(Vector3.Lerp(startPos, newPos, timeElapsed / time), Quaternion.Lerp(startRot, newRot, timeElapsed / time));

                playerModel.transform.localScale = Vector3.Lerp(playerModel.transform.localScale, playerScale, timeElapsed / time);
                playerModel.transform.localPosition = new Vector3(0, playerModel.transform.localScale.y / -2f, 0);
                timeElapsed += Time.deltaTime;
                gazeVector = Vector3.Lerp(gazeVector, gaze, timeElapsed / time);
                yield return null;
            }
            transform.SetPositionAndRotation(newPos, newRot);
            playerModel.transform.localScale = playerScale;
            playerModel.transform.localPosition = new Vector3(0, playerModel.transform.localScale.y / -2f, 0);
            gazeVector = gaze;
            CheckWall(x);
            rawEyeMagnitude = eyeMovement;
            averageEyeMovementMagnitude += eyeMovement;
            if (!hidden)
                averageMovementWallBlock += eyeMovement;
            else
            {
                averageMovementWallBlock = 0.0f;
            }
            if (Mathf.Abs(float.Parse(line[timeIndex]) - prevTime) <= time * 1.1f)
            {
                prevTime = float.Parse(line[timeIndex]);
            }
        }
        processing = false;
    }

    private void CheckWall(int trialNum)
    {
        if (isLearningPhase) return;
        if (wallPositions[trialNum][1].Contains("N/A"))
        {
            HideWall();
            return;
        }
        GridLocation gl = new(wallPositions[trialNum][0][0].ToString(), int.Parse(wallPositions[trialNum][0][1].ToString()));
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(gl.GetX(), gl.GetY())) <= 0.35f)
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
        if (currentPos < wallSpawns[trialNum])
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
        if (!started)
        {
            return;
        }
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
            if (inFirstPerson) Stop();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            infoCanvas.SetActive(!infoCanvas.activeInHierarchy);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            fogToggle = !fogToggle;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CameraSwap();
        }
        DrawGaze();
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
        if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
        currentPos++;
        Process(sr.ReadLine().Split(','));
    }

    private void Stop()
    {
        StopAllCoroutines();
        if (sr != null) sr.Close();
        HideWall();
        filePath = @defaultPath;
        startCanvas.SetActive(true);
        camInfo = "camera_tracker.csv";
        positions.Clear();
        wallSpawns.Clear();
        stressTrials.Clear();
        processing = false;
        inFirstPerson = true;
        thirdPerson.enabled = false;
        firstPerson.enabled = true;
        paused = false;
        currentPos = 0;
        prevTime = 0f;
        prevTrialNum = -1;
        stepped = false;
        started = false;
        Vector3[] linePositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRender.SetPositions(linePositions);
        lineRender.startWidth = lineWidth;
        lineRender.endWidth = lineWidth;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        playerModel.transform.localPosition = Vector3.zero;
        if(isRecordingGaze) OnTrialChanged?.Invoke(-1, 0);
        isRecordingGaze = false;
        isLearningPhase = false;
        timeInTrial = 0;
    }

    private void RecordStop()
    {
        if (sr != null) sr.Close();
        HideWall();
        filePath = @defaultPath;
        startCanvas.SetActive(true);
        camInfo = "camera_tracker.csv";
        positions.Clear();
        wallSpawns.Clear();
        stressTrials.Clear();
        processing = false;
        inFirstPerson = true;
        thirdPerson.enabled = false;
        firstPerson.enabled = true;
        paused = false;
        currentPos = 0;
        prevTime = 0f;
        prevTrialNum = -1;
        stepped = false;
        started = false;
        Vector3[] linePositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        lineRender.SetPositions(linePositions);
        lineRender.startWidth = lineWidth;
        lineRender.endWidth = lineWidth;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        playerModel.transform.localPosition = Vector3.zero;
        if (isRecordingGaze) OnTrialChanged?.Invoke(-1, 0);
        isRecordingGaze = false;
        isLearningPhase = false;
        timeInTrial = 0;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!started || !inFirstPerson) Graphics.Blit(src, dest);
        Vector3 usedDirection = gazeVector; //  <----------  Make this the gaze vector

        float aspectRatio = (float)src.height / src.width;
        float convertToUnitSphere = (float)Mathf.Sqrt(1.0f / usedDirection.z);

        shiftShaderMaterial.SetFloat("gazeY", (usedDirection.y * convertToUnitSphere) + 0.5f);
        shiftShaderMaterial.SetFloat("gazeX", (usedDirection.x * convertToUnitSphere * aspectRatio) + 0.5f);
        shiftShaderMaterial.SetFloat("aspectRatio", aspectRatio);
        shiftShaderMaterial.SetFloat("scotomaSize", scotomaSize);

        RenderTexture temp = src;
        Graphics.Blit(src, temp, shiftShaderMaterial);
        Graphics.Blit(temp, dest);

    }

    private void CameraSwap()
    {
        if (inFirstPerson)
        {
            inFirstPerson = false;
            firstPerson.enabled = false;
            thirdPerson.enabled = true;
        }
        else
        {
            inFirstPerson = true;
            firstPerson.enabled = true;
            thirdPerson.enabled = false;
        }
    }

    private void DrawGaze()
    {
        if (inFirstPerson && !isRecordingGaze) {
            lineRender.SetPositions(new Vector3[2] { Vector3.zero, Vector3.zero });
            return;
        }
        Vector3 actualDir = (transform.rotation * gazeVector).normalized;
        Ray ray = new(transform.position, actualDir);
        float lineLength = fogOn ? 2.5f : maxLineLength;
        Vector3 endPos = transform.position + (actualDir * lineLength);
        if (Physics.Raycast(ray, out RaycastHit hit, lineLength, wallLayerMask))
        {
            endPos = hit.point;
        }
        lineRender.SetPosition(0, transform.position);
        lineRender.SetPosition(1, endPos);
    }

    private void OnApplicationQuit()
    {
        Stop();
    }
}
