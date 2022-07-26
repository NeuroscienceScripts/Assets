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

    private double[] watchTimes;
    private bool started;

    private FileHandler fileHandler;
    private string subjectFile;

    private readonly Collider[] _colliders = new Collider[10];

    private void Awake()
    {
        watchTimes = new double[paintings.Length];
        for (int i = 0; i < watchTimes.Length; i++)
        {
            watchTimes[i] = 0d;
        }
        started = false;
        fileHandler = new();
        trials = new();
        
    }

    public void StartRec()
    {
        started = true;
        int subjectNumber = replay.subjectNum;
        subjectFile = Application.dataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + subjectNumber + ".csv";
        fileHandler.AppendLine(subjectFile, "Trial_ID, A1, A6, B2, C6, C7, D1, D4, E6, F1, F6, G2, G5");
    }

    private void Update()
    {
        if (!started || replay.paused) return;
        Vector3 pos = lineRender.GetPosition(1);
        int numColliders = Physics.OverlapSphereNonAlloc(pos, detectionRadius, _colliders);
        for (int i = 0; i < numColliders; i++)
        {
            for (int j = 0; j < paintings.Length; j++)
            {
                if(paintings[j] == _colliders[i].gameObject)
                {
                    watchTimes[j] += Time.deltaTime;
                }
            }
        }
    }

    private void OnEnable()
    {
        replay.OnTrialChanged += NextTrial;
    }

    private void NextTrial(int trialNum)
    {
        if (Array.Exists(trials.ToArray(), trial => trial == trialNum)) return;
        fileHandler.AppendLine(subjectFile, trialNum.ToString() + PrintWatchTimes());
        trials.Add(trialNum);
        for (int i = 0; i < watchTimes.Length; i++)
        {
            watchTimes[i] = 0d;
        }
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
    }
}
