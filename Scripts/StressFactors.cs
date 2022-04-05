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
        [SerializeField] private GameObject pathBlockObject;
        [SerializeField] private GameObject stressTimer;
        [SerializeField] private float stressTime = 30.0f;
        
        private float lastBeep = 0.0f; 
        
        
        private void Update()
        {
            //TODO: since this function will not work unless there audio sources and an actual game object added, I have just temporarily comment blocked 
            //the current code not involving the dynamic blocking
                if (ExperimentController.Instance.GetTrialInfo().stressTrial)
                {
                /*
                    nonStressSound.Stop();
                    stressSound.Play();

                    stressTimer.GetComponent<TextMeshProUGUI>().text = stressTime -
                        (Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime) + " Seconds";
                    stressTimer.SetActive(true);
                    
                    // Beep plays every 5 seconds until less than 10 seconds left, then a beep a second until 5 seconds left, then two beeps a second
                    float nextBeep = Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime > (stressTime - 10)
                        ? (Time.realtimeSinceStartup - ExperimentController.Instance.trialStartTime > 5 ? 0.5f : 1.0f)
                        : 5.0f;
                    if (Time.realtimeSinceStartup - lastBeep > nextBeep)
                    {
                        stressBeep.Play();
                        lastBeep = Time.realtimeSinceStartup;
                    }
                */
                pathBlockObject.transform.position = new Vector3(
                        ExperimentController.Instance.GetTrialInfo().blockedLocation.GetX(),
                        pathBlockObject.transform.position.y,
                        ExperimentController.Instance.GetTrialInfo().blockedLocation.GetY());
                pathBlockObject.SetActive(true);
            }

                else
                {
                    /*stressSound.Stop();
                    nonStressSound.Play();
                    stressTimer.SetActive(false);*/
                    pathBlockObject.SetActive(false);

                }
        }
    }
}