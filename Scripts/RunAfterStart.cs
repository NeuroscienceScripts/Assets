using UnityEngine;
using Valve.VR;
 

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

        [SerializeField]
        private SteamVR_CameraHelper cameraHelperVR;

        void Update()
        {
            // if (ExperimentController.Instance.introCanvas.enabled)
            // {
            //     firstPersonLook.enabled = false;
            //     firstPersonMovement.enabled = false;
            // }
            // else
            // {
                if (ExperimentController.Instance.desktopMode)
                {
                    firstPersonLook.enabled = true;
                    firstPersonMovement.enabled = true;
                }
                else
                 {
                    cameraHelperVR.enabled = true;
                }
            //}
        }
    }
}