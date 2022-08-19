using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EyeDataTracker : MonoBehaviour
{

    [SerializeField] private NewReplay replay;
    private List<int> trials;

    private bool started;

    private FileHandler fileHandler;
    private string subjectFile;

    private void OnEnable()
    {
        replay.OnTrialChanged += NextTrial;
    }

    private void OnDisable()
    {
        replay.OnTrialChanged -= NextTrial;
    }
    public void StartRec()
    {
        trials = new();
        fileHandler = new();
        started = true;
        int subjectNumber = replay.subjectNum;
        subjectFile = Application.dataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + subjectNumber + "_eye_data.csv";
        fileHandler.AppendLine(subjectFile, "trialID,timeInTrial,avgEyeMovementMagnitude");
    }

    private void NextTrial(int trialNum, float timeInTrial)
    {
        fileHandler.AppendLine(subjectFile, trialNum + "," + timeInTrial + "," + replay.averageEyeMovementMagnitude);
    }
}
