using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] GameObject Camera; 

    // Update is called once per frame
    void Update()
    {
         transform.position = Camera.transform.position; 
    }
}
