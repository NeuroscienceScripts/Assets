using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.UI;

public class SceneManager : MonoBehaviour
{
    public void SwitchScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
