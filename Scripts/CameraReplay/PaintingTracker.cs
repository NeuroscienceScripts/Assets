using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class PaintingTracker : MonoBehaviour
{
    [SerializeField] private NewReplay replay;
    [SerializeField] private GameObject[] paintings;

    [SerializeField] private LineRenderer lineRender;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float detectionRadius;

    private List<int> trials;

    [SerializeField] private double[] watchTimes;
    private bool started;

    private FileHandler fileHandler;
    private string gazeTimes;
    private string fixationFile;

    private readonly Collider[] _colliders = new Collider[10];

    private void Awake()
    {
        started = false;
    }

    public void StartRec()
    {
        watchTimes = new double[paintings.Length];
        for (int i = 0; i < watchTimes.Length; i++)
        {
            watchTimes[i] = 0d;
        }
        trials = new();
        fileHandler = new();
        started = true;
        int subjectNumber = replay.subjectNum;
        gazeTimes = Application.dataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + subjectNumber + "_painting_gaze.csv";
        fixationFile = Application.dataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + subjectNumber + "_fixations.csv";
        fileHandler.AppendLine(gazeTimes, "trialID,timeInTrial,A1,A6,B2,C6,C7,D1,D4,E6,F1,F6,G2,G5");
        fileHandler.AppendLine(fixationFile, "trialID,timeInTrial,painting");
    }

    private void Update()
    {
        if (!started || replay.paused) return;
        Vector3 pos = lineRender.GetPosition(1);
        int numColliders = Physics.OverlapSphereNonAlloc(pos, detectionRadius, _colliders, mask);
        float t = -1;
        for (int i = 0; i < numColliders; i++)
        {
            for (int j = 0; j < paintings.Length; j++)
            {
                if(paintings[j] == _colliders[i].gameObject)
                {
                    t = Time.deltaTime;
                    watchTimes[j] += t;
                }
            }
        }
    }

    private void OnEnable()
    {
        replay.OnTrialChanged += NextTrial;
        replay.OnNextFrame += NextFrame;
    }

    private void NextTrial(int trialNum, float timeInTrial)
    {
        if (trialNum == -1) { 
            started = false;
            return;
        }
        if (Array.Exists(trials.ToArray(), trial => trial == trialNum)) return;
        fileHandler.AppendLine(gazeTimes, trialNum.ToString() + "," + timeInTrial.ToString() + PrintWatchTimes());
        trials.Add(trialNum);
        for (int i = 0; i < watchTimes.Length; i++)
        {
            watchTimes[i] = 0d;
        }
    }

    private void NextFrame(int trialNum, float timeInTrial, float currentTime)
    {
        Vector3 pos = lineRender.GetPosition(1);
        string painting = "None";
        int numColliders = Physics.OverlapSphereNonAlloc(pos, detectionRadius, _colliders, mask);
        for (int i = 0; i < numColliders; i++)
        {
            for (int j = 0; j < paintings.Length; j++)
            {
                if (paintings[j] == _colliders[i].gameObject)
                {
                    painting = _colliders[i].gameObject.name;
                }
            }
        }
        fileHandler.AppendLine(fixationFile, trialNum.ToString() + "," + timeInTrial.ToString() + "," + currentTime.ToString() + "," + painting);
    }

    private string PrintWatchTimes()
    {
        System.Text.StringBuilder sb = new();
        for (int i = 0; i < watchTimes.Length; i++)
        {
            sb.Append("," + ((float)watchTimes[i]).ToString());
        }
        return sb.ToString();
    }

    private void OnDisable()
    {
        replay.OnTrialChanged -= NextTrial;
        replay.OnNextFrame -= NextFrame;
    }
}
