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


    private void Awake()
    {
        positions = new();
        defaultPath = Application.dataPath + @"\Data\";
        camRot = "camera_Rot.csv";
    }

    public void StartReplay()
    {
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

    private IEnumerator Replay()
    {
        currentPos = 0;
        processing = false;
        paused = false;
        sr = new StreamReader(camRot);
        positions.Add(sr.GetPosition());
        string[] line = sr.ReadLine().Split(',');
        currentPos++;
        Process(line);
        float prevTime = float.Parse(line[1]);
        while (!sr.EndOfStream)
        {
            while (paused) yield return null;
            currentPos++;
            if (currentPos >= positions.Count) positions.Add(sr.GetPosition());
            line = sr.ReadLine().Split(',');
            if (!int.TryParse(line[0], out int x)) break;
            processing = true;
            yield return ProcessLine(float.Parse(line[1]) - prevTime, line);
            prevTime = float.Parse(line[1]);
        }
        yield return null;
        Stop();
    }

    private void Process(string[] line)
    {
        if (!int.TryParse(line[0], out int x)) return;
        Vector3 newPos = new(float.Parse(line[7]), float.Parse(line[8]), float.Parse(line[9]));
        transform.rotation = Quaternion.Euler(newPos);
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
                if (!processing)
                {
                    transform.rotation = newPos;
                    yield break;
                }
                while (paused)
                {
                    yield return null;
                }
                transform.rotation = Quaternion.Lerp(startPos, newPos, timeElapsed / time);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            transform.rotation = newPos;
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
        paused = false;
        currentPos = 0;
    }
}
