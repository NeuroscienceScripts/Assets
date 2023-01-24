using System;
using System.Collections;
using System.Collections.Generic;
using Tobii.XR;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VR
{
    public class CameraTracker : MonoBehaviour
    {
        private FileHandler fileHandler = new FileHandler();
        private float recordHeadTimer = 0.0f;
        public float timeInterval = .2f; //how often to record head position

        // public Material pointShader;
        public Camera vrCamera;
        public float smoothMove = 80;
        

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (Time.time - recordHeadTimer > timeInterval & ExperimentController.Instance.recordCameraAndNodes)
            {
                var provider = TobiiXR.Internal.Provider;
                var eyeTrackingData = new TobiiXR_EyeTrackingData();
                provider.GetEyeTrackingDataLocal(eyeTrackingData);
                var smoothMoveSpeed = true;

                var interpolatedGazeDirection = Vector3.Lerp(ExperimentController.Instance._lastGazeDirection, eyeTrackingData.GazeRay.Direction,
                    (smoothMove) * Time.unscaledDeltaTime);
                var usedDirection = smoothMoveSpeed
                    ? interpolatedGazeDirection.normalized
                    : eyeTrackingData.GazeRay.Direction.normalized;
                ExperimentController.Instance._lastGazeDirection = usedDirection;

                var screenPos =
                    vrCamera.WorldToScreenPoint(vrCamera.transform.position +
                                                vrCamera.transform.rotation * usedDirection);

                float gazeX = screenPos.x / src.width;
                float gazeY = screenPos.y / src.height;

                fileHandler.AppendLine(
                    ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                        "_camera_tracker.csv"),
                    ExperimentController.Instance.PrintStepInfo() + "," +
                    ExperimentController.Instance.GetTrialInfo().ToString() +
                    "," + gameObject.transform.rotation.eulerAngles.x.ToString() + "," +
                    gameObject.transform.rotation.eulerAngles.y.ToString() +
                    "," + gameObject.transform.rotation.eulerAngles.z.ToString() + "," +
                    gameObject.transform.position.x.ToString() + "," +
                    gameObject.transform.position.y.ToString() + "," + gameObject.transform.position.z.ToString() +
                    "," +
                    gazeX + "," + gazeY + "," + usedDirection.x + "," + usedDirection.y + "," + usedDirection.z);
                recordHeadTimer = Time.time;

            }

            Graphics.Blit(src, dest);

        }

        // private void Update()
        // {
        //     if (Time.time - recordHeadTimer > timeInterval & ExperimentController.Instance.recordCameraAndNodes)
        //     {
        //         var src = vrCamera.activeTexture;
        //         var provider = TobiiXR.Internal.Provider;
        //         var eyeTrackingData = new TobiiXR_EyeTrackingData();
        //         provider.GetEyeTrackingDataLocal(eyeTrackingData);
        //         var smoothMoveSpeed = true; 
        //         
        //         var interpolatedGazeDirection = Vector3.Lerp(ExperimentController.Instance._lastGazeDirection, eyeTrackingData.GazeRay.Direction, 
        //             (smoothMove) * Time.unscaledDeltaTime);
        //         var usedDirection = smoothMoveSpeed ? interpolatedGazeDirection.normalized : eyeTrackingData.GazeRay.Direction.normalized;
        //         ExperimentController.Instance._lastGazeDirection = usedDirection; 
        //         
        //         var screenPos = vrCamera.WorldToScreenPoint(vrCamera.transform.position + vrCamera.transform.rotation * usedDirection);
        //         
        //         float gazeX = screenPos.x / src.width;
        //         float gazeY = screenPos.y / src.height;
        //         
        //         pointShader.SetFloat("aspectRatio", (float) src.height / src.width);
        //         pointShader.SetFloat("scotomaSize", .05f);
        //         pointShader.SetFloat("gazeX", gazeX); 
        //         pointShader.SetFloat("gazeY", gazeY); 
        //         fileHandler.AppendLine( ExperimentController.Instance.subjectFile.Replace(".csv", "_camera_Rot.csv"),
        //             ExperimentController.Instance.PrintStepInfo() + "," + 
        //             ExperimentController.Instance.GetTrialInfo().ToString()+ ","+ gameObject.transform.rotation.eulerAngles.x.ToString() +"," +
        //             gameObject.transform.rotation.eulerAngles.y.ToString() + "," + gameObject.transform.rotation.eulerAngles.z.ToString() );
        //         fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "_cameraPos.csv"),
        //             ExperimentController.Instance.PrintStepInfo() + "," + ExperimentController.Instance.GetTrialInfo().ToString()+ ","+
        //             gameObject.transform.position.x.ToString() +"," + gameObject.transform.position.y.ToString() + "," + gameObject.transform.position.z.ToString() );
        //         fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "_gazeInfo.csv"), gazeX + "," + gazeY);
        //         recordHeadTimer = Time.time; 
        //         
        //     }
        // }
    }
}
