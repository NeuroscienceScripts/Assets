using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private TMP_Dropdown replayOptions;

    [SerializeField] private GameObject subjectNumInput;
    [SerializeField] private GameObject recordPathInput;
    [SerializeField] private TMP_InputField trialNumInput;

    [SerializeField] private NewReplay newReplay;
    [SerializeField] private PositionReplay posReplay;
    [SerializeField] private RotationReplay rotReplay;

    private void Awake()
    {
        startBtn.onClick.AddListener(StartReplay);
    }

    private void StartReplay()
    {
        
        if(replayOptions.value == 0)
        {
            if (trialNumInput.text != "") newReplay.StartReplay(int.Parse(trialNumInput.text));
            else newReplay.StartReplay();
            posReplay.enabled = false;
            rotReplay.enabled = false;
        }
        else if(replayOptions.value == 1)
        {
            posReplay.StartReplay();
            rotReplay.StartReplay();
            newReplay.enabled = false;
        }else if(replayOptions.value == 2)
        {
            newReplay.StartRecordingReplay();
            posReplay.enabled = false;
            rotReplay.enabled = false;
        }
    }

    public void ChangeTexts()
    {
        if(replayOptions.value < 2)
        {
            subjectNumInput.SetActive(true);
            recordPathInput.SetActive(false);
            trialNumInput.gameObject.SetActive(true);
        }
        else
        {
            subjectNumInput.SetActive(false);
            recordPathInput.SetActive(true);
            trialNumInput.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartReplay);
    }
}
