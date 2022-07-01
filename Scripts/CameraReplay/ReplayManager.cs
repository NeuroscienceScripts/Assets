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

    private void Awake()
    {
        startBtn.onClick.AddListener(StartReplay);
    }

    private void StartReplay()
    {
        if(replayOptions.value == 0)
        {
            newReplay.StartReplay();
        }
        else
        {
            posReplay.StartReplay();
            rotReplay.StartReplay();
        }
    }
}
