using System;
using System.Collections;
using System.Collections.Generic;
using Tobii.XR;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeTrackingTestHandler : MonoBehaviour
{
    private FileHandler fileHandler = new FileHandler(); 
    private float recordHeadTimer = 0.0f;
    public float timeInterval = .15f; //how often to record head position
    
    public Material pointShader;
    public Camera vrCamera;
    public float smoothMove = 80; 
    private Vector3 _lastGazeDirection;
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var provider = TobiiXR.Internal.Provider;
        var eyeTrackingData = new TobiiXR_EyeTrackingData();
        provider.GetEyeTrackingDataLocal(eyeTrackingData);
        var smoothMoveSpeed = true; 
        
        var interpolatedGazeDirection = Vector3.Lerp(_lastGazeDirection, eyeTrackingData.GazeRay.Direction, 
            (smoothMove) * Time.unscaledDeltaTime);
        var usedDirection = smoothMoveSpeed ? interpolatedGazeDirection.normalized : eyeTrackingData.GazeRay.Direction.normalized;
        _lastGazeDirection = usedDirection; 
        
        var screenPos = vrCamera.WorldToScreenPoint(vrCamera.transform.position + vrCamera.transform.rotation * usedDirection);
        Debug.Log(screenPos);

        float gazeX = screenPos.x / src.width;
        float gazeY = screenPos.y / src.height;
       
        pointShader.SetFloat("aspectRatio", (float) src.height / src.width);
        pointShader.SetFloat("scotomaSize", .05f);
        pointShader.SetFloat("gazeX", gazeX); 
        pointShader.SetFloat("gazeY", gazeY); 
        
        if (Time.time - recordHeadTimer > timeInterval & ExperimentController.Instance.recordCameraAndNodes)
        {
            fileHandler.AppendLine( ExperimentController.Instance.subjectFile.Replace(".csv", "_camera_Rot.csv"),
                ExperimentController.Instance.PrintStepInfo() + "," + 
                ExperimentController.Instance.GetTrialInfo().ToString()+ ","+ gameObject.transform.rotation.eulerAngles.x.ToString() +"," +
                gameObject.transform.rotation.eulerAngles.y.ToString() + "," + gameObject.transform.rotation.eulerAngles.z.ToString() );
            fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "_cameraPos.csv"),
                ExperimentController.Instance.PrintStepInfo() + "," + ExperimentController.Instance.GetTrialInfo().ToString()+ ","+
                gameObject.transform.position.x.ToString() +"," + gameObject.transform.position.y.ToString() + "," + gameObject.transform.position.z.ToString() );
            fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "_gazeInfo.csv"), gazeX + "," + gazeY);
            recordHeadTimer = Time.time; 
        }

        RenderTexture tmp = new RenderTexture(src); 
        Graphics.Blit(src, tmp, pointShader);
        Graphics.Blit(tmp, dest);
        RenderTexture.ReleaseTemporary(tmp); 
    }
}
