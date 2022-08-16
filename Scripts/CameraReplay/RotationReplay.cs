using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

public class RotationReplay : MonoBehaviour
{
    private StreamReader sr;

    [SerializeField] private TMP_InputField fileInput;
    [SerializeField] private TMP_InputField subjectInput;
    private string defaultPath;
    private string filePath;
    private string camRot = "camera_Rot.csv";

    private int subjectNum = 0;

    private bool paused;

    private List<long> positions;
    private int currentPos;
    private bool processing;
    private bool stepped;

    float prevTime;
    int prevTrialNum;
    bool trialSpecific;

    private void Awake()
    {
        positions = new();
        defaultPath = Application.dataPath + @"\Data\";
        camRot = "camera_Rot.csv";
    }

    public void StartReplay()
    {
        trialSpecific = false;
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        if (filePath[^1] != '/')
        {
            filePath += @"/";
        }
        subjectNum = int.Parse(subjectInput.text);
        camRot = filePath + subjectNum + "_" + camRot;
        if (!File.Exists(camRot))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: camera_Rot.csv");
        }
        else StartCoroutine(Replay());
    }

    public void StartReplay(int trial)
    {
        trialSpecific = true;
        filePath = (fileInput.text != "") ? @fileInput.text : @defaultPath;
        if (filePath[^1] != '/')
        {
            filePath += @"/";
        }
        subjectNum = int.Parse(subjectInput.text);
        camRot = filePath + subjectNum + "_" + camRot;
        if (!File.Exists(camRot))
        {
            Stop();
            Debug.LogError("Invalid File Path or Missing Critical File: camera_Rot.csv");
        }
        else StartCoroutine(Replay(trial));
    }

    private IEnumerator Replay()
    {
        currentPos = 0;
        processing = false;
        paused = false;
        sr = new StreamReader(camRot);
        string[] line;
        prevTime = 0;
        prevTrialNum = -1;
        while (!sr.EndOfStream)
        {
            currentPos++;
            if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            if(prevTrialNum != x)
            {
                float timeElapsed  = 0f;
                while (timeElapsed <= 1f)
                {
                    yield return null;
                    timeElapsed += Time.deltaTime;
                    if (paused || !processing) break;
                }
                prevTrialNum = x;
                prevTime = 0f;
            }
            processing = true;
            yield return ProcessLine(float.Parse(line[1]) - prevTime, line);
            while (paused) yield return null;
        }
        yield return null;
        Stop();
    }

    private IEnumerator Replay(int trial)
    {
        currentPos = 0;
        processing = false;
        paused = false;
        sr = new StreamReader(camRot);
        string[] line;
        int targetTrial = trial;
        prevTime = 0;
        prevTrialNum = -1;
        while (!sr.EndOfStream)
        {
            currentPos++;
            if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            if (targetTrial != x && prevTrialNum == -1) continue;

            if (targetTrial != x && prevTrialNum != -1)
            {
                break;
            }
            prevTrialNum = x;
            processing = true;
            yield return ProcessLine(float.Parse(line[1]) - prevTime, line);
            while (paused) yield return null;
        }
        yield return null;
        Stop();
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
        }
        Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        transform.rotation = Quaternion.Euler(newPos);
        prevTime = float.Parse(line[1]);
    }


    private IEnumerator ProcessLine(float time, string[] line)
    {
        if (int.TryParse(line[0], out int x))
        {
            float timeElapsed = 0;
            Quaternion startPos = transform.rotation;
            Quaternion newPos = Quaternion.Euler(new Vector3(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9])));
            while (timeElapsed <= time)
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
                    transform.rotation = newPos;
                    if (stepped) StepBackward();
                    stepped = false;
                    yield break;
                }
                transform.rotation = Quaternion.Lerp(startPos, newPos, timeElapsed / time);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = newPos;
            if (Mathf.Abs(float.Parse(line[1]) - prevTime) <= time * 1.1f)
            {
                prevTime = float.Parse(line[1]);
            }
        }
        
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
        if(sr != null) sr.Close();
        filePath = @defaultPath;
        camRot = "camera_Rot.csv";
        positions.Clear();
        processing = false;
        stepped = false;
        paused = false;
        currentPos = 0;
        prevTime = 0f;
        prevTrialNum = -1;
    }
}
