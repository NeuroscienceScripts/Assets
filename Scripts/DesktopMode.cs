using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopMode : MonoBehaviour
{
    //Always keep CurvedUIInputModule disabled before Play

    [SerializeField] private StandaloneInputModule input;
    [SerializeField] private CurvedUIInputModule curvedUI;
    
    void Update()
    {
        if (ExperimentController.Instance.desktopMode)
        {
            input.enabled = true;
            curvedUI.enabled = false;
        }
        else
        {
            input.enabled = false;
            curvedUI.enabled = true;
        }
    }
}
