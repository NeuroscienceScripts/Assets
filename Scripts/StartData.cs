using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.XR;
using UnityEngine.XR;

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
    [SerializeField] private TextMeshProUGUI subjNumText;
    [SerializeField] private TextMeshProUGUI trialNumText;

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

    private void OnEnable()
    {
        XRSettings.enabled = false;
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
        SetSubjNum(0);
        SetTrialNum(0);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        XRSettings.enabled = true;

    }
}
