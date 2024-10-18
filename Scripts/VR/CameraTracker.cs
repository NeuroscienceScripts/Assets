using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using ViveSR.anipal;

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
        private bool headerWritten = false;
        private bool headerWrittenMouse = false;
        public GameObject playerCam;
        private float startTime;
        VerboseData verboseData = new VerboseData();
        // private static EyeData_v2 eyeData = new EyeData_v2();
        private bool eye_callback_registered = false;
        Vector3 gazeOriginCombinedLocal, gazeDirectionCombinedLocal;

        private void WriteHeader()
        {
            Debug.Log(ExperimentController.Instance.subjectFile);
            fileHandler.AppendLine(
                ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                    "_camera_tracker.csv"), "Trial_ID,TrialTime,Phase,TrialNumber,StepInPhase,Start,End,Augmentation," +
                                            "CamRotX,CamRotY,CamRotZ,CamPosX,CamPosY,CamPosZ,ScreenGazeX,ScreenGazeY,WorldGazeX,WorldGazeY,WorldGazeZ,GetScreenFixationPointX(),GetScreenFixationPointY(),GazeFixation1(),GazeFixation2(),GazeFixation3(),GetGazeCombinedGazeRayLocal1(),GetGazeCombinedGazeRayLocal2(),GetGazeCombinedGazeRayLocal3(),LeftEyePosition1(),LeftEyePosition2(),LeftEyePosition3(),RightEyePosition1(),RightEyePosition2(),RightEyePosition3(),LeftEyeRotation1(),LeftEyeRotation2(),LeftEyeRotation3(),RightEyeRotation1(),RightEyeRotation2(),RightEyeRotation3(),LeftEyePupilSize(),RightEyePupilSize(),LeftEyeOpenAmount(),RightEyeOpenAmount()");
        }
        
        private void WriteHeaderLearning()
        {
            Debug.Log(ExperimentController.Instance.subjectFile);
            fileHandler.AppendLine(
                ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                    "_learning_camera_tracker.csv"), "Phase,LapNumber,StepInPhase,TrialTime,CamRotX,CamRotY,CamRotZ,CamPosX,CamPosY,CamPosZ");
        }
        
        private void WriteHeaderMouse()
        {
            Debug.Log(MazeHandler.Instance.subjectFile);
            fileHandler.AppendLine(
                MazeHandler.Instance.subjectFile.Replace(MazeHandler.Instance.Date_time + ".csv",
                    "_mouse_camera_tracker.csv"), "TrialTime,StepInPhase,CamRotX,CamRotY,CamRotZ,CamPosX,CamPosY,CamPosZ");
        }
        
        // private static void EyeCallback(ref EyeData_v2 eye_data)
        // {
        //     eyeData = eye_data;
        // }

        private void OnDestroy()
        {
            SRanipal_API.Release(SRanipal_Eye_v2.ANIPAL_TYPE_EYE_V2);
        }

        private void Start()
        {
            SRanipal_API.Initial(SRanipal_Eye.ANIPAL_TYPE_EYE, IntPtr.Zero);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            if (Time.time - recordHeadTimer > timeInterval & ExperimentController.Instance.recordCameraAndNodes)
            {
                if (ExperimentController.Instance.isTraining)
                {
                    if (!headerWrittenMouse)
                    {
                        headerWrittenMouse = true;
                        WriteHeaderMouse();
                    }

                    fileHandler.AppendLine(
                        MazeHandler.Instance.subjectFile.Replace(MazeHandler.Instance.Date_time + ".csv",
                            "_mouse_camera_tracker.csv"),
                        Time.realtimeSinceStartup + "," + MazeHandler.Instance.stepInPhase +
                        "," + gameObject.transform.rotation.eulerAngles.x.ToString() + "," +
                        gameObject.transform.rotation.eulerAngles.y.ToString() +
                        "," + gameObject.transform.rotation.eulerAngles.z.ToString() + "," +
                        gameObject.transform.position.x.ToString() + "," +
                        gameObject.transform.position.y.ToString() + "," +
                        gameObject.transform.position.z.ToString());
                    recordHeadTimer = Time.time;
                }
                else
                {
                    if (!playerCam.GetComponent<SimpleFirstPersonMovement>().enabled)
                    {
                        if (!headerWritten)
                        {
                            headerWritten = true;
                            WriteHeader();
                        }
                        
                        // if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        //     SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;
                        //
                        // if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                        // {
                        //     SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        //     eye_callback_registered = true;
                        // }
                        // else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                        // {
                        //     SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        //     eye_callback_registered = false;
                        // }

                        if (SRanipal_Eye_v2.GetVerboseData(out verboseData)) { }
                        else { Debug.LogWarning("Failed to find SRanipal Framework V1 (verboseData), do you have the SDK installed?"); }
                        
                        var smoothMoveSpeed = true;
                        
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOriginCombinedLocal,
                            out gazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out gazeOriginCombinedLocal,
                            out gazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out gazeOriginCombinedLocal,
                            out gazeDirectionCombinedLocal)) { }
                        
                        var interpolatedGazeDirection = Vector3.Lerp(ExperimentController.Instance._lastGazeDirection,
                            gazeDirectionCombinedLocal,
                            (smoothMove) * Time.unscaledDeltaTime);
                        var usedDirection = smoothMoveSpeed
                            ? interpolatedGazeDirection.normalized
                            : gazeDirectionCombinedLocal.normalized;
                        
                        ExperimentController.Instance._lastGazeDirection = usedDirection;
                        
                        var screenPos =
                            vrCamera.WorldToScreenPoint(vrCamera.transform.position +
                                                        vrCamera.transform.rotation * usedDirection);
                        
                        float gazeX = screenPos.x / src.width;
                        float gazeY = screenPos.y / src.height;

                        var eyedata = (GetScreenFixationPoint() +","+ GazeFixation() +"," + GetGazeCombinedGazeRayLocal() + "," 
                                       + LeftEyePosition() +","+ RightEyePosition() +","+
                                       LeftEyeRotation() +","+ RightEyeRotation() +","+ LeftEyePupilSize() +","+ RightEyePupilSize() + ","+
                                       LeftEyeOpenAmount() +","+ RightEyeOpenAmount()).Replace("(","").Replace(")","");
                        
                        fileHandler.AppendLine(
                            ExperimentController.Instance.subjectFile.Replace(
                                ExperimentController.Instance.Date_time + ".csv",
                                "_camera_tracker.csv"),
                            ExperimentController.Instance.PrintStepInfo() + "," +
                            ExperimentController.Instance.GetTrialInfo().ToString() +
                            "," + ExperimentController.Instance.GetTrialInfo().augmentation + "," + gameObject.transform.rotation.eulerAngles.x.ToString() + "," +
                            gameObject.transform.rotation.eulerAngles.y.ToString() +
                            "," + gameObject.transform.rotation.eulerAngles.z.ToString() + "," +
                            gameObject.transform.position.x.ToString() + "," +
                            gameObject.transform.position.y.ToString() + "," +
                            gameObject.transform.position.z.ToString() +
                            "," +
                            gazeX + "," + gazeY + "," + usedDirection.x + "," + usedDirection.y + "," +
                            usedDirection.z + "," + eyedata);
                        recordHeadTimer = Time.time;
                    }
                    else
                    {
                        if (!headerWritten)
                        {
                            headerWritten = true;
                            WriteHeaderLearning();
                        }

                        if ((ExperimentController.Instance.phase==1)&((ExperimentController.Instance.currentTrial == 0) ||
                            (ExperimentController.Instance.currentTrial == 3)))
                        {
                            startTime = ExperimentController.Instance.trialStartTime;
                        }

                        fileHandler.AppendLine(
                            ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                                "_learning_camera_tracker.csv"),
                            ExperimentController.Instance.phase + "," + ExperimentController.Instance.currentTrial + "," + ExperimentController.Instance.stepInPhase + "," + (Time.realtimeSinceStartup-startTime) +
                            "," + gameObject.transform.rotation.eulerAngles.x.ToString() + "," +
                            gameObject.transform.rotation.eulerAngles.y.ToString() +
                            "," + gameObject.transform.rotation.eulerAngles.z.ToString() + "," +
                            gameObject.transform.position.x.ToString() + "," +
                            gameObject.transform.position.y.ToString() + "," +
                            gameObject.transform.position.z.ToString());
                        recordHeadTimer = Time.time;
                    }
                }
            }

            Graphics.Blit(src, dest);

        }
        
        public Vector3 GetGazeCombinedGazeRayLocal() {
            return gazeDirectionCombinedLocal; }
        public Vector3 GetGazeCombinedPositionLocal() {
            return gazeDirectionCombinedLocal; }

        public Vector2 GetScreenFixationPoint(){
            var screenPos =
                vrCamera.WorldToScreenPoint( GazeFixation());

            float gazeX = screenPos.x / vrCamera.pixelWidth;
            float gazeY = screenPos.y / vrCamera.pixelHeight; 

            return new Vector2(gazeX, gazeY); }

        public Vector3 GazeFixation()
        {
            var trans = vrCamera.transform; 
            return trans.position + (trans.rotation.eulerAngles.normalized + gazeDirectionCombinedLocal); 
        }
        
        public Vector3 LeftEyePosition() {
            return verboseData.left.gaze_origin_mm; }

        public Vector3 RightEyePosition() {
            return verboseData.right.gaze_origin_mm; }
        
        public Vector3 LeftEyeRotation() {
            return verboseData.left.gaze_direction_normalized; }

        public Vector3 RightEyeRotation() {
            return verboseData.right.gaze_direction_normalized; }

        public float LeftEyeOpenAmount() {
            return verboseData.left.eye_openness; }

        public float RightEyeOpenAmount() {
            return verboseData.right.eye_openness; }

        public float LeftEyePupilSize() {
            return verboseData.left.pupil_diameter_mm; }

        public float RightEyePupilSize() {
            return verboseData.right.pupil_diameter_mm; }
        
        // private void Release()
        // {
        //     if (eye_callback_registered == true)
        //     {
        //         SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
        //         eye_callback_registered = false;
        //     }
        // }

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
