using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VR
{
    public class SimpleFirstPersonMovement : MonoBehaviour
    {
        public Vector2 rotation = Vector2.zero;
        [SerializeField] public float mouseSensitivity = 100.0f;
        [SerializeField] float movementSpeed = 5.0f;
        [SerializeField] bool fixedVerticalHeadRotation = true;
        [SerializeField] private bool lockHeight = true;
        [SerializeField] private bool adjustableSensitivity;
        [SerializeField] private Rigidbody rb;
        private float lockedHeight;
        private Vector3 dir;
        public bool active;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                active = !active;
                Debug.Log(active
                    ? "Escape button pressed, activated movement"
                    : "Escape button pressed, de-activated movement");
            }

            if (active)
            {
                dir = Vector3.zero;
                var t = transform;
                t.localPosition = Vector3.zero;
                // rotation.y += (Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime);
                // rotation.x -= fixedVerticalHeadRotation
                //     ? 0
                //     : Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                // t.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);

                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetMouseButton(0))
                    //t.parent.position += t.forward * Time.deltaTime * movementSpeed;
                    dir += t.forward;
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetMouseButton(1))
                    //t.parent.position += t.forward * Time.deltaTime * movementSpeed * -1.0f;
                    dir += -t.forward;
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    //t.parent.position += t.right * Time.deltaTime * movementSpeed * -1.0f;
                    dir += -t.right;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    //t.parent.position += t.right * Time.deltaTime * movementSpeed;
                    dir += t.right;
                
                mouseSensitivity += adjustableSensitivity ? 2.5f * Input.mouseScrollDelta.y : 0.0f;

                if (lockHeight)
                    t.parent.position = new Vector3(t.parent.position.x, lockedHeight, t.parent.position.z); 
            }
        }

        private void LateUpdate()
        {
            if (active)
            {
                var t = transform;
                rotation.y += (Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime);
                rotation.x -= fixedVerticalHeadRotation
                    ? 0
                    : Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                t.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0);
                rb.velocity = dir.normalized * movementSpeed;
            }
        }

        public void Start()
        {
            lockedHeight = gameObject.transform.position.y; 
        }
    }
}