// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public class DesktopMode : MonoBehaviour
// {
//     /*
//      * Attatch to EventSystem
//      * Always keep CurvedUIInputModule disabled before Play
//      * StandaloneInputModule and CurvedUIInputModule references are on Event System
//     */
//
//     [SerializeField] private StandaloneInputModule input;
//    // [SerializeField] private CurvedUIInputModule curvedUI;
//     
//     void OnEnable()
//     {
//         if (ExperimentController.Instance.desktopMode)
//         {
//             input.enabled = true;
//             //curvedUI.enabled = false;
//         }
//         else
//         {
//             input.enabled = false;
//             //curvedUI.enabled = true;
//         }
//     }
// }
