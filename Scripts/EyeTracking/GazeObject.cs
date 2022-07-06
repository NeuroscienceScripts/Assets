using UnityEngine;
using Tobii.G2OM;

namespace DefaultNamespace
{
    public class GazeObject : MonoBehaviour, IGazeFocusable
    {
        private FileHandler fileHandler = new FileHandler();
        
        //The method of the "IGazeFocusable" interface, which will be called when this object receives or loses focus
        public void GazeFocusChanged(bool hasFocus)
        {
            //Debug.Log(gameObject.name);
            if(ExperimentController.Instance.recordCameraAndNodes)
               fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time+".csv","_gazeTargets.csv"),
                ExperimentController.Instance.PrintStepInfo() + gameObject.name);
        }
    }

}