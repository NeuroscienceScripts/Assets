using System;
using TMPro;
using UnityEngine;
using Classes;

namespace DefaultNamespace
{
    public class StressFactors : MonoBehaviour
    {
        // AudioSource 
        [SerializeField] private AudioSource stressSound;
        [SerializeField] private AudioSource nonStressSound;
        [SerializeField] private AudioSource stressBeep;
        [SerializeField] private AudioSource spatialAudio;
        //[SerializeField] private GameObject pathBlockObject;
        [SerializeField] private GameObject stressTimer;
        // Distance that fog extends 
        [SerializeField] private float startFog = 1.0f; 
        [SerializeField] private float endFog = 2.5f; 
        private float lastBeep = 0.0f;
        private void Update()
        {
            // Learning phase  
            if (ExperimentController.Instance.phase == 1)
            {
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogStartDistance = startFog;
                RenderSettings.fogEndDistance = endFog;
                RenderSettings.fogColor = Color.gray;
                RenderSettings.fog = true;
                stressSound.Stop();
                stressTimer.SetActive(false);
            }
            // Testing phase: begin trials (stepInPhase 1 and 2 are for orienting) 
            if (ExperimentController.Instance.phase == 3 & ExperimentController.Instance.stepInPhase == 3)
            {
                // 12 stressful trials 
                if (ExperimentController.Instance.GetTrialInfo().stressTrial)
                {
                    // 6 with spatial audio 
                    if (ExperimentController.Instance.GetTrialInfo().hasAudio)
                    {
                        RenderSettings.fogMode = FogMode.Linear;
                        RenderSettings.fogStartDistance = startFog;
                        RenderSettings.fogEndDistance = endFog;
                        RenderSettings.fogColor = Color.black;
                        RenderSettings.fog = true;
                        nonStressSound.Stop();
                        if (!stressSound.isPlaying)
                            stressSound.Play();
                        // Beep plays every 5 seconds until less than 10 seconds left, then a beep a second until 5 seconds left, then two beeps a second
                        float nextBeep = Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime >
                                         (ExperimentController.Instance.stressTimeLimit - 10)
                            ? (Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime > 5
                                ? 0.5f
                                : 1.0f)
                            : 5.0f;
                        if (Time.realtimeSinceStartup - lastBeep > nextBeep)
                        {
                            stressBeep.Play();
                            lastBeep = Time.realtimeSinceStartup;
                        }
                        // Why is spatial audio not playing!!!
                        if (!spatialAudio.isPlaying)
                            spatialAudio.Play();
                        stressTimer.GetComponent<TextMeshProUGUI>().text =
                            ExperimentController.Instance.stressTimeLimit -
                            (Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime) + " Seconds";
                        stressTimer.SetActive(true);

                    }
                    // 6 without spatial audio
                    else
                    {
                        RenderSettings.fogMode = FogMode.Linear;
                        RenderSettings.fogStartDistance = startFog;
                        RenderSettings.fogEndDistance = endFog;
                        RenderSettings.fogColor = Color.black;
                        RenderSettings.fog = true;
                        nonStressSound.Stop();
                        // Beep plays every 5 seconds until less than 10 seconds left, then a beep a second until 5 seconds left, then two beeps a second
                        float nextBeep = Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime >
                                         (ExperimentController.Instance.stressTimeLimit - 10)
                            ? (Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime > 5
                                ? 0.5f
                                : 1.0f)
                            : 5.0f;
                        if (Time.realtimeSinceStartup - lastBeep > nextBeep)
                        {
                            stressBeep.Play();
                            lastBeep = Time.realtimeSinceStartup;
                        }
                        // Stop S audio
                        spatialAudio.Stop();
                        if (!stressSound.isPlaying)
                            stressSound.Play();
                        stressTimer.GetComponent<TextMeshProUGUI>().text =
                            ExperimentController.Instance.stressTimeLimit -
                            (Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime) + " Seconds";
                        stressTimer.SetActive(true);
                    }
                }

            }
            // 12 non-stressful trials
            else
            {
                RenderSettings.fog = false;
                stressSound.Stop();
                //Stop S audio
                spatialAudio.Stop();
                if (!nonStressSound.isPlaying & !ExperimentController.Instance.GetTrialInfo().stressTrial)
                    nonStressSound.Play();
                stressTimer.SetActive(false);
            }
        }
    }
}
