using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attaches to camera to record camera's position/location at the specified time interval.
/// </summary>
public class CameraTracker : MonoBehaviour
{
    private FileHandler fileHandler = new FileHandler(); 
    private float recordHeadTimer = 0.0f;
    public float timeInterval = .15f; //how often to record head position

    // Update is called once per frame
    void Update()
    {
        if (Time.time - recordHeadTimer > timeInterval & ExperimentController.Instance.recordCameraAndNodes)
        {
            fileHandler.AppendLine( ExperimentController.Instance.subjectFile.Replace(".csv", "_cameraRot.csv"),
                       ExperimentController.Instance.PrintStepInfo() + "," + 
                       ExperimentController.Instance.GetTrialInfo().ToString()+ ","+ gameObject.transform.rotation.eulerAngles.x.ToString() +"," +
                       gameObject.transform.rotation.eulerAngles.y.ToString() + "," + gameObject.transform.rotation.eulerAngles.z.ToString() );
            fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "_cameraPos.csv"),
                ExperimentController.Instance.PrintStepInfo() + "," + ExperimentController.Instance.GetTrialInfo().ToString()+ ","+
                gameObject.transform.position.x.ToString() +"," + gameObject.transform.position.y.ToString() + "," + gameObject.transform.position.z.ToString() );
            recordHeadTimer = Time.time; 
        }
    }

    void Start()
    {
        
    }
}