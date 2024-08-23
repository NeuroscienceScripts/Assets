using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPointer : MonoBehaviour
{
    public Transform targetObject; // Assign your target object in the inspector
    public Transform vrCamera; // Assign your VR camera in the inspector
    public GameObject Arrow; // Assign your existing 3D arrow in the inspector
    public float offset = 2f;
    private Vector3 targetArrowPosition;
    private Quaternion targetArrowRotation;
    public float smoothTime = 0.1f; // Adjust this value to control the smoothness
    public bool smoothmove=false;

    private void Update()
    {
        targetObject = ExperimentController.Instance.GetTrialInfo().end.GetTargetObject().transform;
        if (targetObject != null && vrCamera != null && Arrow != null)
        {
            // // Calculate direction from camera to target
            // Vector3 targetDirection = targetObject.position - vrCamera.position;
            //
            // // Position the arrow in front of the camera
            // Arrow.transform.position = vrCamera.position + vrCamera.forward * offset; // Adjust distance as needed
            //
            // // Rotate the arrow to point towards the target
            // Arrow.transform.rotation = Quaternion.LookRotation(targetDirection);
            
            // Calculate target position and rotation
            targetArrowPosition = vrCamera.position + vrCamera.forward * offset;
            targetArrowRotation = Quaternion.LookRotation(targetObject.position - vrCamera.position);

            // Smoothly interpolate towards the target position and rotation
            // Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, targetArrowPosition, smoothTime * Time.deltaTime);
            if (smoothmove)
            {
                // Smooth only the Y component of the arrow's position
                Vector3 smoothedPosition = targetArrowPosition;
                smoothedPosition.y = Mathf.Lerp(Arrow.transform.position.y, targetArrowPosition.y, smoothTime * Time.deltaTime);

                // Apply the smoothed position and the target rotation
                Arrow.transform.position = smoothedPosition;
                // Arrow.transform.position = Vector3.Lerp(Arrow.transform.position, targetArrowPosition, smoothTime * Time.deltaTime);
                // Arrow.transform.rotation = Quaternion.Slerp(Arrow.transform.rotation, targetArrowRotation, smoothTime * Time.deltaTime);
                Arrow.transform.rotation = Quaternion.LookRotation(targetObject.position - vrCamera.position);
            }
            else
            {
                Arrow.transform.position = targetArrowPosition; // Adjust distance as needed
                Arrow.transform.rotation = Quaternion.LookRotation(targetObject.position - vrCamera.position);
            }
        }
    }
}
