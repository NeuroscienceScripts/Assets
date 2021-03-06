using System;
using TMPro;
using UnityEngine;
using Classes;

namespace DefaultNamespace
{
    public class StressFactors : MonoBehaviour
    {
        [SerializeField] private AudioSource stressSound;
        [SerializeField] private AudioSource nonStressSound;
        [SerializeField] private AudioSource stressBeep; 
        //[SerializeField] private GameObject pathBlockObject;
        [SerializeField] private GameObject stressTimer;
        [SerializeField] private float startFog = 1.0f; 
        [SerializeField] private float endFog = 2.5f; 
        private float lastBeep = 0.0f; 
        
        
        private void Update()
        {
            if (ExperimentController.Instance.phase == 3 & ExperimentController.Instance.stepInPhase == 3)
            {
                if (ExperimentController.Instance.GetTrialInfo().stressTrial)
                {
                    RenderSettings.fogMode = FogMode.Linear;
                    RenderSettings.fogStartDistance = startFog;
                    RenderSettings.fogEndDistance = endFog; 
                    RenderSettings.fogColor = Color.black;
                    RenderSettings.fog = true; 
                    nonStressSound.Stop();
                    if(!stressSound.isPlaying)
                        stressSound.Play();

                    stressTimer.GetComponent<TextMeshProUGUI>().text = ExperimentController.Instance.stressTimeLimit -
                        (Time.time - ExperimentController.Instance.trialStartTime) + " Seconds";
                    stressTimer.SetActive(true);

                    // Beep plays every 5 seconds until less than 10 seconds left, then a beep a second until 5 seconds left, then two beeps a second
                    float nextBeep = Time.time - ExperimentController.Instance.trialStartTime >
                                     (ExperimentController.Instance.stressTimeLimit- 10)
                        ? (Time.time - ExperimentController.Instance.trialStartTime > 5 ? 0.5f : 1.0f)
                        : 5.0f;
                    if (Time.time - lastBeep > nextBeep)
                    {
                        stressBeep.Play();
                        lastBeep = Time.time;
                    }
                }
                
            }  
            else
             {
                 RenderSettings.fog = false; 
                 stressSound.Stop();
                 if(!nonStressSound.isPlaying & !ExperimentController.Instance.GetTrialInfo().stressTrial)
                     nonStressSound.Play();
                 stressTimer.SetActive(false);
             }
        }
    }
}