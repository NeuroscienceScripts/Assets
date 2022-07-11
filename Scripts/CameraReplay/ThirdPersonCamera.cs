using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera thirdPersonCam;
    [SerializeField] private Transform target;
    [SerializeField] private Transform cameraPivot;
    
    
    [Header("Variables")]
    [SerializeField] private float camSpeed = 0.2f;
    [SerializeField] private float horizSpeed;
    [SerializeField] private float vertSpeed;
    [SerializeField] private float minVertAngle = -35;
    [SerializeField] private float maxVertAngle = 35;
    [SerializeField] private float zoomSpeed;

    private Vector3 camVelocity;
    private Vector3 zoomVelocity;
    private float vertAngle;
    private float horizAngle;
    private float cameraOffsetX;
    private float cameraOffsetY;
    private bool lockCam;

    private void Awake()
    {
        camVelocity = Vector3.zero;
        cameraOffsetX = thirdPersonCam.transform.position.z;
        cameraOffsetY = cameraPivot.transform.position.y;
        lockCam = false;
    }

    private void CameraFollow()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref camVelocity, camSpeed);
    }

    private void CameraRotate()
    {
        horizAngle += (Input.GetAxis("Mouse X") * horizSpeed);
        vertAngle -= (Input.GetAxis("Mouse Y") * vertSpeed);
        vertAngle = Mathf.Clamp(vertAngle, minVertAngle, maxVertAngle);

        transform.rotation = Quaternion.Euler(new Vector3(0, horizAngle, 0));

        cameraPivot.localRotation = Quaternion.Euler(new Vector3(vertAngle, 0, 0));

    }

    private void CameraZoom()
    {
        thirdPersonCam.transform.localPosition = Vector3.SmoothDamp(thirdPersonCam.transform.localPosition, new Vector3(thirdPersonCam.transform.localPosition.x, thirdPersonCam.transform.localPosition.y, thirdPersonCam.transform.localPosition.z + (Input.mouseScrollDelta.y * zoomSpeed)), ref zoomVelocity, 0.2f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lockCam = !lockCam;
            Cursor.lockState = lockCam ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = lockCam;
        }
    }


    private void LateUpdate()
    {
        
        if (!thirdPersonCam.enabled) {
            transform.rotation = Quaternion.identity;
            cameraPivot.localRotation = Quaternion.identity;
            transform.position = target.position - (target.forward * cameraOffsetX) - (target.up * cameraOffsetY);
            thirdPersonCam.transform.localPosition = new Vector3(0, 0, cameraOffsetX);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        CameraFollow();
        if(!lockCam) CameraRotate();
        CameraZoom();
    }
}
