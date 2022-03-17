using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Only used to keep the first person controller (non-VR) from being active before the subject info has been updated
    /// and RunStartup() of ExperimentController.cs is called
    /// </summary>
    public class RunAfterStart : MonoBehaviour
    {
        [SerializeField] private FirstPersonMovement firstPersonMovement;
        [SerializeField] private FirstPersonLook firstPersonLook;

        void Update()
        {
            if (ExperimentController.Instance.introCanvas.enabled)
            {
                firstPersonLook.enabled = false;
                firstPersonMovement.enabled = false; 
            }
            else
            {
                firstPersonLook.enabled = true;
                firstPersonMovement.enabled = true; 
            }
        }
    }
}