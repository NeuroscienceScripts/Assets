using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

namespace VR
{
    /// <summary>
    /// Attach this to your camera to automatically switch between
    /// keyboard movement and headset.
    /// Checks every 'checkHeadsetFrequency' milliseconds to see
    /// if a headset is active.
    /// If headset active: enable XR and SteamVR Camera
    /// If headset not active: disable XR and use keyboard controls
    /// </summary>
    public class AutomaticDesktopVsVR : MonoBehaviour
    {
        [SerializeField] private int checkHeadsetFrequency = 30000;
        private float lastCheck;
        private SimpleFirstPersonMovement firstPerson;
        private SteamVR_CameraHelper cameraHelper;

        void Start()
        {
            firstPerson = gameObject.GetComponent<SimpleFirstPersonMovement>();
            cameraHelper = gameObject.GetComponent<SteamVR_CameraHelper>();
            CheckHeadset();
        }

        void CheckHeadset()
        {
            Debug.Log(SteamVR.connected.ToArray().ToCommaSeparatedString());
            bool headsetConnected = SteamVR.connected[0];
            XRSettings.enabled = headsetConnected;
            cameraHelper.enabled = headsetConnected;
            firstPerson.enabled = !headsetConnected;
            Debug.Log(SteamVR.connected[0]);
        }

        private void Update()
        {
            if (Time.realtimeSinceStartup - lastCheck > checkHeadsetFrequency / 1000.0f)
            {
                CheckHeadset();
                lastCheck = Time.realtimeSinceStartup;
            }
        }
    }
}