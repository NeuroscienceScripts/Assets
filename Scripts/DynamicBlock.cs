using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Classes;
using System;
using DefaultNamespace;

public class DynamicBlock : MonoBehaviour
{
    private string prevRow;
    private string[] obstaclesList = { "B1", "B3", "B5", "B6", "D2", "D3", "D5", "D6", "F2", "F4", "F5", "F7" };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ExperimentController.Instance.GetTrialInfo().stressTrial)
        {
            //Currently this will just compare where the start and end are relative to each other and determine what the row right
            //before the end is. TODO: Determine an effective method for when start and end are on the same row, as for now it defaults
            //to the row after it (this can be an issue if start is mailbox and end is chair for example as it goes out of bounds to Row H)
            if (ExperimentController.Instance.GetTrialInfo().start.y.CompareTo(ExperimentController.Instance.GetTrialInfo().end.y) < 0)
            {
                prevRow = (char)(ExperimentController.Instance.GetTrialInfo().end.y[0] - 1) + "";
            }
            else
            {
                prevRow = (char)(ExperimentController.Instance.GetTrialInfo().end.y[0] + 1) + "";
            }

            //if end is to your left it will try to block your left
            //if end is to your right it will try to block to your right
            //if end is ahead, it will try blocking top and bottom
            //if left/right were unsuccessful, it will default to top/bottom
            if (ControllerCollider.Instance.currentNode.y.Equals(prevRow))
            {
                //left case which reverts into top/bot
                if(ExperimentController.Instance.GetTrialInfo().end.x < ControllerCollider.Instance.currentNode.x)
                {
                    if (Array.IndexOf(obstaclesList, prevRow + (ControllerCollider.Instance.currentNode.x - 1 + "")) == -1)
                        ExperimentController.Instance.SetTrialBlockedLocation(ControllerCollider.Instance.currentNode.y, ControllerCollider.Instance.currentNode.x - 1);
                    else
                        ExperimentController.Instance.SetTrialBlockedLocation(ExperimentController.Instance.GetTrialInfo().end.y, ExperimentController.Instance.GetTrialInfo().end.x);
                } else if (ExperimentController.Instance.GetTrialInfo().end.x > ControllerCollider.Instance.currentNode.x) //right case
                {
                    if (Array.IndexOf(obstaclesList, prevRow + (ControllerCollider.Instance.currentNode.x + 1 + "")) == -1)
                        ExperimentController.Instance.SetTrialBlockedLocation(ControllerCollider.Instance.currentNode.y, ControllerCollider.Instance.currentNode.x + 1);
                    else
                        ExperimentController.Instance.SetTrialBlockedLocation(ExperimentController.Instance.GetTrialInfo().end.y, ExperimentController.Instance.GetTrialInfo().end.x);
                } else //case that you are right in front of the end
                {
                    ExperimentController.Instance.SetTrialBlockedLocation(ExperimentController.Instance.GetTrialInfo().end.y, ExperimentController.Instance.GetTrialInfo().end.x);
                }
            }
        }
        else
        {
            this.gameObject.transform.position = new Vector3(12, 12, 12);
        }
    }
}
