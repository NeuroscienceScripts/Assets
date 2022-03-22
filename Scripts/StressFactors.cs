using System;
using TMPro;
using UnityEngine;

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
            if (ExperimentController.Instance.GetTrialInfo().stressTrial)
            {
                nonStressSound.Stop();
                stressSound.Play();
                pathBlockObject.transform.position = new Vector3(
                    ExperimentController.Instance.GetTrialInfo().blockedLocation.GetX(),
                    pathBlockObject.transform.position.y,
                    ExperimentController.Instance.GetTrialInfo().blockedLocation.GetY());
                pathBlockObject.SetActive(true);

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
            }
            
            else
            {
                stressSound.Stop();
                nonStressSound.Play();
                pathBlockObject.SetActive(false);
                stressTimer.SetActive(false); 

            }
        }
    }
}