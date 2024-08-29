using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectTracker : MonoBehaviour
{
    public Camera mainCamera;
    public float maxFogDensityThreshold = 1.0f; 

    private List<GameObject> objectsInFrame = new List<GameObject>();

    private void Update()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not assigned!");
            return;
        }
        
        foreach (GameObject obj in ExperimentController.Instance.selecedSecondaryObjects)
        {
            if (IsInLineOfSight(obj))
            {
                if (!objectsInFrame.Contains(obj))
                {
                    objectsInFrame.Add(obj);
                    Debug.Log(obj.name + " entered the camera frame.");
                }
            }
            else
            {
                if (objectsInFrame.Contains(obj))
                {
                    objectsInFrame.Remove(obj);
                    Debug.Log(obj.name + " left the camera frame.");
                }
            }
        }
    }

    private bool IsInLineOfSight(GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - mainCamera.transform.position;
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.transform.position, directionToTarget, out hit))
        {
            if (hit.collider.gameObject == target)
            {
                float fogDensityAtHitPoint = GetFogDensityAtPosition(hit.point); 

                if (fogDensityAtHitPoint < maxFogDensityThreshold)
                {
                    return true; 
                }
            }
        }

        return false;
    }

    private float GetFogDensityAtPosition(Vector3 position)
    {
        float distance = Vector3.Distance(mainCamera.transform.position, position);
        float fogDensity = Mathf.Clamp01((distance - 1f) / 1.5f);
        // Debug.Log(fogDensity);
        return fogDensity;
    }
}
