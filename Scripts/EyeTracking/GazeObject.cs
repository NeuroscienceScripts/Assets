using UnityEngine;
using Tobii.XR;

namespace DefaultNamespace
{
    public class GazeObject : MonoBehaviour
    {
        private FileHandler fileHandler = new FileHandler();
        private float recordFrequency = 0.15f;
        private float lastRecord = 0.0f;
        private bool smoothMove = true;
        private float smoothMoveSpeed = 18.0f;
        private Vector3 lastGazeDirection = new Vector3(0.0f, 0.0f, 0.0f);

        //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
        public void GazeFocusChanged(bool hasFocus)
        {
            //If this object received focus, fade the object's color to highlight color
            if (hasFocus & Time.realtimeSinceStartup - lastRecord > recordFrequency)
            {
                fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "gazeTargets.csv"),
                    ExperimentController.Instance.PrintStepInfo() + "," + gameObject.name);
                lastRecord = Time.realtimeSinceStartup; 
            }
        }
    }

}
    
