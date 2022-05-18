using UnityEngine;
using Tobii.XR;

namespace DefaultNamespace
{
    public class TobiiEyeTracking : MonoBehaviour
    {
        private FileHandler fileHandler = new FileHandler();
        private float recordFrequency = 0.15f;
        private float lastRecord = 0.0f;
        private bool smoothMove = true;
        private float smoothMoveSpeed = 18.0f;
        private Vector3 lastGazeDirection = new Vector3(0.0f, 0.0f, 0.0f);

        private void Update()
        {
            if (ExperimentController.Instance.recordCameraAndNodes)
            {
                var provider = TobiiXR.Internal.Provider;
                var eyeTrackingData = new TobiiXR_EyeTrackingData();
                provider.GetEyeTrackingDataLocal(eyeTrackingData);

                var interpolatedGazeDirection = Vector3.Lerp(lastGazeDirection, eyeTrackingData.GazeRay.Direction,
                    smoothMoveSpeed * Time.unscaledDeltaTime);
                var usedDirection = smoothMove
                    ? interpolatedGazeDirection.normalized
                    : eyeTrackingData.GazeRay.Direction.normalized;
                lastGazeDirection = usedDirection;

                if (Time.realtimeSinceStartup - lastRecord > recordFrequency &
                    ExperimentController.Instance.recordCameraAndNodes)
                {
                    var vrCamera = gameObject.GetComponent<Camera>();
                    var screenPos =
                        vrCamera.WorldToScreenPoint(vrCamera.transform.position +
                                                    vrCamera.transform.rotation * usedDirection);
                    fileHandler.AppendLine(
                        ExperimentController.Instance.subjectFile.Replace(".csv", "_gazeTracking.csv"),
                        ExperimentController.Instance.PrintStepInfo() + "," + screenPos.x + "," +
                        screenPos.y + "," + screenPos.z);
                }
            }
        }
    }
}