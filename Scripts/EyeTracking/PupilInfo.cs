using UnityEngine;
using ViveSR.anipal.Eye;

namespace DefaultNamespace
{
    public class PupilInfo : MonoBehaviour
    {
         private FileHandler fileHandler = new FileHandler();
        private float recordFrequency = 0.15f;
        private float lastRecord = 0.0f;
        private bool smoothMove = true;
        private float smoothMoveSpeed = 18.0f;
        private Vector3 lastGazeDirection = new Vector3(0.0f, 0.0f, 0.0f);

        private static EyeData eyeData;
        private static VerboseData verboseData;
        private float pupilDiameterLeft, pupilDiameterRight;
        private Vector2 pupilPositionLeft, pupilPositionRight;
        private float eyeOpenLeft, eyeOpenRight;
        
        private void Update()
        {
            SRanipal_Eye.GetVerboseData(out verboseData);    
             
            // pupil positions    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
            // pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;    // eye open
            // eyeOpenLeft = eyeData.verbose_data.left.eye_openness;    eyeOpenRight = eyeData.verbose_data.right.eye_openness;}

            if (Time.realtimeSinceStartup - lastRecord > recordFrequency &
                ExperimentController.Instance.recordCameraAndNodes)
            {

                pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;    
                pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;   
                
                fileHandler.AppendLine(ExperimentController.Instance.subjectFile.Replace(".csv", "_pupilsize.csv"),
                    ExperimentController.Instance.PrintStepInfo() + "," + pupilDiameterLeft + "," + pupilDiameterRight);

            }
        }

    }
}