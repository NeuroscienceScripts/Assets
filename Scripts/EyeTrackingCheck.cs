using UnityEngine;
using UnityEngine.UI;
using VR; 

public class EyeTrackingCheck : MonoBehaviour {
    [SerializeField] private int maxFramesWithoutUpdate = 100;
    [SerializeField] private RawImage eyeError;
    private CameraTracker cameraTracker; 
    public int framesWithoutGazeUpdate;
    private Vector3 preveye = new Vector3(0.0f, 0.0f, 0.0f);

    void Update()
    {
        ExperimentController ec = ExperimentController.Instance;
        // if (ec.recordCameraAndNodes && (ec._lastGazeDirection.x == 0.0f &&
        //                                 ec._lastGazeDirection.y == 0.0f &&
        //                                 ec._lastGazeDirection.z == 1.0f)) {
        if (ec.recordCameraAndNodes & Vector3.Distance(ec._lastGazeDirection, preveye) < 0.1 & ExperimentController.Instance.phase==3)
        {
            framesWithoutGazeUpdate++;
            eyeError.enabled = framesWithoutGazeUpdate > maxFramesWithoutUpdate;
        }
        else
        {
            framesWithoutGazeUpdate = ec.recordCameraAndNodes ? 0 : framesWithoutGazeUpdate;
            eyeError.enabled = false;
        }

        preveye = ec._lastGazeDirection;
    }

    void Start()
    {
        cameraTracker = gameObject.GetComponent<CameraTracker>(); 
    }
}
