using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogReplay : MonoBehaviour
{

    [SerializeField] private float startFog = 1.0f;
    [SerializeField] private float endFog = 2.5f;
    public void On()
    {
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = startFog;
        RenderSettings.fogEndDistance = endFog;
        RenderSettings.fogColor = Color.black;
        RenderSettings.fog = true;
    }

    public void Off()
    {
        RenderSettings.fog = false;
    }
}
