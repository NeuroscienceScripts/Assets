using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackv3 : MonoBehaviour
{
    public static trackv3 Instance { get; private set; }
    public Camera playerCamera;  // Assign the player's camera in the Inspector
    public List<GameObject> objectsToTrack, objectsInView;  // List of objects to track for visibility
    private Plane[] frustumPlanes;
    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Initialize the list of objects to track (you can populate this via the Inspector)
        objectsToTrack = new List<GameObject>(GameObject.FindGameObjectsWithTag("Trackable"));  // Assumes objects have a "Trackable" tag
        objectsInView = new List<GameObject>();
    }

    public void trackSecObjects()
    {
        // objectsInView.Clear();
        // Update the view frustum planes based on the camera's current position and orientation
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        // Check the visibility of each object
        foreach (GameObject obj in objectsToTrack)
        {
            if (IsObjectInView(obj) && !IsObjectOccluded(obj))
            {
                // Object is in view and not occluded
                if (!objectsInView.Contains(obj))
                {
                    objectsInView.Add(obj);
                }
            }
        }
    }

    bool IsObjectInView(GameObject obj)
    {
        // Get the object's renderer bounds
        Bounds bounds = obj.GetComponent<Renderer>().bounds;

        // Check if the bounds of the object are within the view frustum
        return GeometryUtility.TestPlanesAABB(frustumPlanes, bounds);
    }

    bool IsObjectOccluded(GameObject obj)
    {
        // Cast a ray from the camera to the object
        Vector3 campos = playerCamera.transform.position;
        Vector3 objpos = obj.transform.position;
        Vector3 directionToObj = objpos - campos;
        float distance = Vector3.Distance(campos, objpos);
        if (distance>2.6f)
        {
            return true;
        }
        
        // Ray ray = new Ray(campos, directionToObj);
        // Debug.DrawRay(ray.origin, ray.direction*10, Color.yellow);
        RaycastHit hit;
        
        // Perform the raycast
        if (Physics.Raycast(campos, directionToObj*10, out hit))
        {
            // // Get the GameObject that was hit
            // GameObject hitObject = hit.collider.gameObject;
            // Debug.Log("Hit GameObject: " + hitObject.name);
            // If the ray hits the object first, it's not occluded
            if (hit.transform == obj.transform)
            {
                return false;
            }
            // Otherwise, it's occluded
            else
            {
                return true;
            }
        }

        // Default to occluded
        return true;
    }
}
