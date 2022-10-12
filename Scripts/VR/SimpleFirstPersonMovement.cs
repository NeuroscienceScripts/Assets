using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VR
{
    public class SimpleFirstPersonMovement : MonoBehaviour
    {
        Vector2 rotation = Vector2.zero;
        [SerializeField] float mouseSensitivity = 100.0f;
        [SerializeField] float movementSpeed = 5.0f;
        [SerializeField] bool fixedVerticalHeadRotation = true;
        [SerializeField] private bool lockHeight = true;
        [SerializeField] private bool adjustableSensitivity;
        private float lockedHeight; 
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
                var t = transform;
                rotation.y += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                rotation.x -= fixedVerticalHeadRotation
                    ? 0
                    : Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                t.eulerAngles = (Vector2) rotation;

                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                    t.position += t.forward * Time.deltaTime * movementSpeed;

                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                    t.position += t.forward * Time.deltaTime * movementSpeed * -1.0f;

                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    t.position += t.right * Time.deltaTime * movementSpeed * -1.0f;

                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    t.position += t.right * Time.deltaTime * movementSpeed;

                mouseSensitivity += adjustableSensitivity ? 2.5f * Input.mouseScrollDelta.y : 0.0f;

                if (lockHeight)
                    t.position = new Vector3(t.position.x, lockedHeight, t.position.z); 
            }
        }

        public void Start()
        {
            lockedHeight = gameObject.transform.position.y; 
        }
    }
}