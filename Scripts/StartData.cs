using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum Phase
{
    Default = 0,
    Learn = 1,
    Trace = 2,
    Test = 3
}

public class StartData : MonoBehaviour
{
    public static StartData instance;
    
    public bool stressFirst = false;
    public int subjNum = 0;
    public int trialNum = 0;
    public Phase phase = Phase.Default;
    [SerializeField] private TMP_InputField subjNumText;
    [SerializeField] private TMP_InputField trialNumText;

    public bool replayMode; 
    public string replayFile; 
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    

    private void Update()
    {
        if (subjNumText.text == "")
        {
            subjNumText.text = "0";
        }

        if (trialNumText.text == "")
        {
            trialNumText.text = "0";
        }
    }

    public void ChangeStress()
    {
        stressFirst = !stressFirst;
    }

    public void SetSubjNum(int num)
    {
        subjNum = num;
    }

    public void SetTrialNum(int num)
    {
        trialNum = num;
    }

    public void SetPhase(int p)
    {
        phase = (Phase) p;
    }
    

    public void SetData()
    {
        var subjText = subjNumText.text;
        var trialText = trialNumText.text;
        Debug.Log($"subject: {subjText} , trial: {trialText}");
        SetSubjNum(int.Parse(subjText));
        SetTrialNum(int.Parse(trialText));

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ReplayMode()
    {
    //   replayFile = EditorUtility.OpenFilePanel("Select Directory", "", "");
       replayMode = true; 
    }
    
}
