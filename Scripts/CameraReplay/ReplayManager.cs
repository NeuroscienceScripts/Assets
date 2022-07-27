using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ReplayManager : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private TMP_Dropdown replayOptions;

    [SerializeField] private NewReplay newReplay;
    [SerializeField] private PositionReplay posReplay;
    [SerializeField] private RotationReplay rotReplay;
    [SerializeField] private PaintingTracker paintingTracker;

    private void Awake()
    {
        startBtn.onClick.AddListener(StartReplay);
    }

    private void StartReplay()
    {
        if(replayOptions.value == 0)
        {
            newReplay.StartReplay();
            posReplay.enabled = false;
            rotReplay.enabled = false;
            newReplay.isRecordingGaze = false;
        }
        else if(replayOptions.value == 1)
        {
            posReplay.StartReplay();
            rotReplay.StartReplay();
            newReplay.enabled = false;
        }else
        {
            newReplay.StartReplay();
            paintingTracker.StartRec();
            posReplay.enabled = false;
            rotReplay.enabled = false;
            newReplay.isRecordingGaze = true;
        }
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartReplay);
    }
}
