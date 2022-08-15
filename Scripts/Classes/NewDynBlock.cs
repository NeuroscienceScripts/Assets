// using System;
// using System.Linq;
// using UnityEngine;
//
// namespace Classes
// {
//     public class NewDynBlock : MonoBehaviour
//     {
//         private ExperimentController EH; 
//         public void alreadyBlocked = false; 
//         private void Update(){ if (ExperimentController.Instance.GetTrialInfo().isWallTrial 
//            && CheckStart() && GetWall()) 
//             GetWall().SetActive(true); }
//
//         bool CheckStart()
//         {
//             switch (ExperimentController.Instance.GetTrialInfo().start.GetString())
//             {
//                 case "A1": return new[] {"B2", "A3"}.Contains(name); 
//                 // case ... 
//             }
//
//             return false; 
//         }
//         
//         GameObject GetWall()
//         {
//             alreadyBlocked = true; 
//             if (EH.)
//             //returns wall to turn on 
//         }
//
//        
//         private void Awake()
//         {
//             EH = ExperimentController.Instance;
//         }
//     }
// }