using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Classes;
using DefaultNamespace;
using TMPro;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.VR;
using Random = UnityEngine.Random;

//TODO: Fix paintings to be on wall, fix footprints to rotate toward next target 
/// <summary>
/// On each frame, ExperimentController checks which phase you are in (learning, retracing, or testing).
/// Within each phase, ExperimentController checks what step of the phase you are on and acts accordingly.
/// For more info on each phase, see function's summary
/// </summary>
public class ExperimentController : MonoBehaviour
{
    public static ExperimentController Instance { get; private set; }

    //[SerializeField] public Canvas introCanvas;
    [SerializeField] public Canvas stressCanvas;
    [SerializeField] private GameObject maze;
    [SerializeField] private GameObject footprints;
    [SerializeField] private GameObject moveForwardArrow;
    [SerializeField] public GameObject stressLevel;
    [SerializeField] public GameObject stressText; 

    //[SerializeField] private GameObject subjectInput;
    //[SerializeField] private GameObject trialInput;
    [SerializeField] private GameObject userText;
    [SerializeField] private RawImage redScreen;

    [SerializeField] public bool desktopMode = false;
    [SerializeField] private bool hidePaintingsDuringTesting = true;
    [SerializeField] private GameObject paintings;
    [SerializeField] private GameObject node;

    [SerializeField] private int learningRounds = 5;
    [SerializeField] private int retraceRounds = 1; 
    private int learningRedoRounds = 0;
    [SerializeField] private float retraceTimeLimit = 10.0f;
    [SerializeField] public float stressTimeLimit = 15.0f;
    [SerializeField] private float nonStressTimeLimit = 30.0f;

    [SerializeField] private int number_practice_trials = 2;
    private int numTrials = 24;
    [SerializeField] private bool stressFirst = false;
    
    #region StressVars
    [SerializeField] private StressFactors stressFactors;
    [SerializeField] private DynamicBlock dynamicBlock;
    #endregion

    public int subjectNumber = 0;

    public int phase = 0;
    public int stepInPhase = 0;
    public int currentTrial;
    public bool confirm = false;
    public float trialStartTime = 0.0f;
    private float redFlashTimer = 0.0f; 
    [SerializeField] private float redFlashTimeLimit = .5f;
    
    
    public FileHandler fileHandler = new FileHandler();
    public string subjectFile;
    public string Date_time;
    public bool recordCameraAndNodes = false;
    public string blockedWall = "";
    

    private Vector3[] arrowPath =
    {
        // x location, z location, rotation (east=0, south=90, west=180, north=270)
        //todo fix arrows
        new Vector3(2.0f, -3.0f, 180.0f),
        new Vector3(0.0f, -3.0f, 180.0f),
        new Vector3(-3.0f, -3.0f, 270.0f),
        new Vector3(-3.0f, -1.0f, 270.0f),
        new Vector3(-3.0f, 1.0f, 0.0f),
        new Vector3(-2.0f, 2.0f, 235.0f),
        new Vector3(-2.5f, 3.0f, 0.0f),
        new Vector3(1.0f, 3.0f, 0.0f),
        new Vector3(3.0f, 3.0f, 90.0f),
        new Vector3(3.0f, 1.0f, 180.0f),
        new Vector3(0.0f, 1.0f, 90.0f),
        new Vector3(0f, -1.0f, 0.0f),
        new Vector3(2.0f, -1.0f, 90.0f),
    };
    
    private Trial[] trialList =
    {
        // Practice trials
        new Trial(new GridLocation("A", 1), new GridLocation("A", 6), false),
        new Trial(new GridLocation("G", 2), new GridLocation("B", 2), false),

        // Stress trials
        // blockedList = {7,6,5,11,3,9};
        // IsWallTrial = true --> odd id participants will have these trials blocked, even will be opposite
        new Trial(new GridLocation("A", 1), new GridLocation("F", 6), true, true),
        new Trial(new GridLocation("E", 6), new GridLocation("A", 1), true, true),
        new Trial(new GridLocation("D", 1), new GridLocation("F", 6), true, true),
        new Trial(new GridLocation("B", 2), new GridLocation("C", 6), true, true),
        new Trial(new GridLocation("D", 4), new GridLocation("G", 2), true, true),
        new Trial(new GridLocation("G", 5), new GridLocation("C", 7), true, true),
        new Trial(new GridLocation("A", 6), new GridLocation("G", 2), true),
        new Trial(new GridLocation("G", 2), new GridLocation("C", 7), true),
        new Trial(new GridLocation("F", 1), new GridLocation("E", 6), true),
        new Trial(new GridLocation("C", 7), new GridLocation("B", 2), true),
        new Trial(new GridLocation("A", 1), new GridLocation("D", 4), true),
        new Trial(new GridLocation("F", 6), new GridLocation("A", 6), true),

        // Non-stress trials 
        new Trial(new GridLocation("F", 6), new GridLocation("B", 2), false),
        new Trial(new GridLocation("B", 2), new GridLocation("G", 5), false), 
        new Trial(new GridLocation("F", 1), new GridLocation("C", 7), false),
        new Trial(new GridLocation("C", 6), new GridLocation("F", 1), false),
        new Trial(new GridLocation("C", 6), new GridLocation("A", 1), false),
        new Trial(new GridLocation("G", 2), new GridLocation("C", 6), false),
        new Trial(new GridLocation("E", 6), new GridLocation("D", 1), false),
        new Trial(new GridLocation("C", 7), new GridLocation("D", 1), false),
        new Trial(new GridLocation("D", 1), new GridLocation("D", 4), false),
        new Trial(new GridLocation("D", 4), new GridLocation("F", 1), false),
        new Trial(new GridLocation("A", 6), new GridLocation("E", 6), false),
        new Trial(new GridLocation("G", 5), new GridLocation("A", 6), false),
        
        // new Trial(new GridLocation("A", 1), new GridLocation("F", 6), true),
        // new Trial(new GridLocation("A", 6), new GridLocation("E", 6), true),
        // new Trial(new GridLocation("B", 2), new GridLocation("G", 5), true),
        // new Trial(new GridLocation("F", 1), new GridLocation("E", 6), true, true),
        // new Trial(new GridLocation("C", 6), new GridLocation("F", 1), true),
        // new Trial(new GridLocation("C", 7), new GridLocation("D", 1), true, true),
        // new Trial(new GridLocation("D", 1), new GridLocation("D", 4), true, true),
        // new Trial(new GridLocation("D", 4), new GridLocation("G", 2), true, true),
        // new Trial(new GridLocation("E", 6), new GridLocation("A", 1), true),
        // new Trial(new GridLocation("F", 6), new GridLocation("B", 2), true, true),
        // new Trial(new GridLocation("G", 2), new GridLocation("C", 6), true),
        // new Trial(new GridLocation("G", 5), new GridLocation("C", 7), true, true),
        //
        // // Non-stress trials 
        // new Trial(new GridLocation("A", 1), new GridLocation("D", 4), false),
        // new Trial(new GridLocation("A", 6), new GridLocation("G", 2), false),
        // new Trial(new GridLocation("B", 2), new GridLocation("C", 6), false),
        // new Trial(new GridLocation("F", 1), new GridLocation("C", 7), false),
        // new Trial(new GridLocation("C", 6), new GridLocation("A", 1), false),
        // new Trial(new GridLocation("C", 7), new GridLocation("B", 2), false),
        // new Trial(new GridLocation("D", 1), new GridLocation("F", 6), false),
        // new Trial(new GridLocation("D", 4), new GridLocation("F", 1), false),
        // new Trial(new GridLocation("E", 6), new GridLocation("D", 1), false),
        // new Trial(new GridLocation("F", 6), new GridLocation("A", 6), false),
        // new Trial(new GridLocation("G", 2), new GridLocation("C", 7), false),
        // new Trial(new GridLocation("G", 5), new GridLocation("A", 6), false),
    };

    private string[] obstaclesList = { "B1", "B3", "B5", "B6", "D2", "D3", "D5", "D6", "F2", "F4", "F5", "F7" };

    [SerializeField] private int[] trialOrder = {0}; //Randomized at start


    /* Debug */
    [SerializeField] bool debugActive = true;
    [SerializeField] private GameObject phaseDisplay;
    [SerializeField] private GameObject stepDisplay;
    [SerializeField] private GameObject trialDisplay;
    [SerializeField] private GameObject pause;

    /// <summary>
    /// Called every frame. Checks which phase of the experiment to run then calls the correct function
    /// </summary>
    void Update()
    {
        if (!StartData.instance.replayMode)
        {
            if (SteamVR.calibrating)
            {
                pause.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                if (Time.timeScale == 0f)
                {
                    pause.SetActive(false);
                    Time.timeScale = 1f;
                }
            }

            if (Time.time - redFlashTimer > redFlashTimeLimit)
                redScreen.enabled = false;
            DisplayDebugInfo();

            switch (phase)
            {
                case 0:
                    // stressCanvas.enabled = false;
                    // userText.GetComponent<TextMeshProUGUI>().text = "Input subject/trial number and select phase";
                    break;
                case 1:
                    RunLearning();
                    break;
                case 2:
                    RunRetrace();
                    break;
                case 3:
                    RunTesting();
                    break;
                default:
                    FinishExperiment();
                    StartCoroutine(WaitCoroutine());
                    Application.Quit();
                    break;
            }
        }
    }

    private StreamReader sr;
    private string[][] split_lines; 

    private void Start()
    {
        if (StartData.instance.replayMode)
        {
           /* "Trial_ID,TrialTime,Phase,TrialNumber,StepInPhase,Start,End," +
                "CamRotX,CamRotY,CamRotZ,CamPosX,CamPosY,CamPosZ,ScreenGazeX,ScreenGazeY,WorldGazeX,WorldGazeY,WorldGazeZ" */
            split_lines = new string[System.IO.File.ReadAllLines(StartData.instance.replayFile).Length][];
            int count = 0; 
            sr = new StreamReader(StartData.instance.replayFile);
            
            while (!sr.EndOfStream)
            {
                // Should input every line split into the array
                split_lines[count] = sr.ReadLine().Split(',');
            }
        }
    }

    private int currentFrame = 0;
    [SerializeField] private GameObject camera;
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (StartData.instance.replayMode)
        {
            camera.transform.position = new Vector3(float.Parse(split_lines[currentFrame][10]),
                float.Parse(split_lines[currentFrame][11]), float.Parse(split_lines[currentFrame][12]));
            
            Graphics.Blit(src, dest);
        }
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(5);
    }

    /// <summary>
    /// Displays the Phase/Step in the specified debugGUI (if debugActive is set to true)
    /// </summary>
    void DisplayDebugInfo()
    {
        if (debugActive)
        {
            phaseDisplay.GetComponent<TextMeshProUGUI>().text = "Phase: " + phase.ToString();
            stepDisplay.GetComponent<TextMeshProUGUI>().text = "Step: " + stepInPhase.ToString();
            trialDisplay.GetComponent<TextMeshProUGUI>().text = "Trial: " + currentTrial;
        }
        else
        {
            phaseDisplay.SetActive(false);
            stepDisplay.SetActive(false);
            trialDisplay.SetActive(false);
        }
    }

    /// <summary>
    /// Gets starting point/subject number and starts the experiment
    /// Which phase you're starting on is inputted by the buttons in the startup GUI, then trial# and session# are
    /// loaded in as your starting point. 
    /// </summary>
    /// <param name="phaseNumberStart"> int value attached to GUI button which calls RunStartup()</param> 
    public void RunStartup(int phaseNumberStart)
    {
        //subjectNumber = int.Parse(subjectInput.GetComponent<TMP_InputField>().text);
        Date_time = "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        subjectFile = Application.dataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + subjectNumber + Date_time +".csv";
        Debug.Log(subjectFile);
        fileHandler.AppendLine(subjectFile, "trialID,timeInTrial,phase,trialNumber,stepInPhase,start,goal,selected,blockedWall,isStressTrial");
        fileHandler.AppendLine(subjectFile.Replace(Date_time +".csv", "_nodePath.csv"),
            DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
        // fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_cameraRot.csv"),
        //     "trialID,timeInTrial,phase,trialNumber,stepInPhase,start,end,xRot,yRot,zRot");
        // fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_cameraPos.csv"),
        //     "trialID,timeInTrial,phase,trialNumber,stepInPhase,start,end,xPos,yPos,zPos");
        fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_stress.csv"),
            "start,end,stress_level");

        //currentTrial = int.Parse(trialInput.GetComponent<TMP_InputField>().text);
        //phase = phaseNumberStart;
        //introCanvas.enabled = false;

        // Blocking
        trialOrder = new int[trialList.Length];
        for (int i = 0; i < number_practice_trials; i++)
        {
            trialOrder[i] = i;
        }

        List<int> stress = new();
        List<int> nonStress = new();
        for (int i = number_practice_trials; i < trialList.Length; i++)
        {
            if (trialList[i].stressTrial)
            {
                stress.Add(i);
            }
            else
            {
                nonStress.Add(i);
            }
        }

        Random.InitState(subjectNumber * 10);
        stress = stress.ToArray().OrderBy(x => Random.Range(0,stress.Count)).ToList();
        
        nonStress = nonStress.ToArray().OrderBy(x => Random.Range(0,nonStress.Count)).ToList();

        
        
        if (stressFirst)
        {
            for (int i = 0; i < stress.Count; i++)
            {
                trialOrder[number_practice_trials + i] = stress[i];
            }
            for (int i = 0; i < nonStress.Count; i++)
            {
                trialOrder[number_practice_trials + stress.Count + i] = nonStress[i];
            }
        }
        else
        {
            for (int i = 0; i < nonStress.Count; i++)
            {
                trialOrder[number_practice_trials + i] = nonStress[i];
            }
            for (int i = 0; i < stress.Count; i++)
            {
                trialOrder[number_practice_trials + nonStress.Count + i] = stress[i];
            }
        }

        // Create a web of invisible node colliders to track position
        string[] letters = { "A", "B", "C", "D", "E", "F", "G" };
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };
        GameObject[] nodes = new GameObject[49];
        for (int letter = 0; letter < letters.Length; letter++)
        {
            for (int number = 1; number <= 7; number++)
            {
                int currentPos = number - 1 + letter * 7;
                nodes[currentPos] = GameObject.Instantiate(node);
                nodes[currentPos].name = letters[letter] + number;

                if (!obstaclesList.Contains(nodes[currentPos].name))
                {
                    GridLocation nodeLocation = new GridLocation(letters[letter], number);
                    nodes[currentPos].transform.position =
                        new Vector3(nodeLocation.GetX(), node.transform.position.y, nodeLocation.GetY());
                }
                else
                {
                    nodes[currentPos].SetActive(false); // To avoid erroneous recordings from inside walls
                }

            }
        }
        //introCanvas.enabled = false;
    }


    /// <summary>
    /// Requires participant to walk to starting point of learning phase and hit the trigger.
    /// Once started, the arrow moves along the path specified (at the start of this file) each
    /// time the participant collides with it.  This repeats for how ever many learningRounds are
    /// specified. 
    /// </summary>
    void RunLearning()
    {
        float arrowHeight = moveForwardArrow.transform.position.y;

        if ( currentTrial >= learningRounds)
        {
            Debug.Log("Move to retracing phase"); 
            moveForwardArrow.SetActive(false);
            currentTrial = 0;
            phase++;
        }
        else
        {
            if (stepInPhase == 0)
            {
                userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the trigger button";
                maze.SetActive(false);
                moveForwardArrow.SetActive(false);
                footprints.SetActive(true);
                footprints.transform.position =
                    new Vector3(arrowPath[0].x, footprints.transform.position.y, arrowPath[0].y);
                if (GetTrigger(false) & ControllerCollider.Instance.controllerSelection.Contains(footprints.name))
                {
                    recordCameraAndNodes = true;
                    fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_nodePath.csv"), "Learning Phase"); 
                    stepInPhase++;
                }
            }
            else if (stepInPhase < arrowPath.Length)
            {
                userText.GetComponent<TextMeshProUGUI>().text = "Learn the path by following the arrow";
                maze.SetActive(true);
                footprints.SetActive(false);
                moveForwardArrow.SetActive(true);
                moveForwardArrow.transform.position =
                    new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
                moveForwardArrow.transform.rotation = Quaternion.Euler(
                    moveForwardArrow.transform.rotation.eulerAngles.x,
                    arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);

                if (ControllerCollider.Instance.controllerSelection.Contains(moveForwardArrow.name))
                {
                    stepInPhase++;
                    ControllerCollider.Instance.controllerSelection = "Not selected";
                    recordCameraAndNodes = false;
                }
            }

            if (stepInPhase > 1)
            {
                userText.GetComponent<TextMeshProUGUI>().text = "";
            }

            if (stepInPhase >= arrowPath.Length)
            {
                Debug.Log("Increment current trial");
                currentTrial++;
                stepInPhase = 0;
            }
            
        }

    }

    public int retraceNodes = 0;
    public float retraceTimer = 0; 
    /// <summary>
    /// Has participants retrace their learned route.  The arrow is invisible but still detecting collisions and moving
    /// like in RunLearning(). If the participant travels too many nodes before reaching a checkpoint, they fail and
    /// must redo the learning phase. 
    /// </summary>
    void RunRetrace()
    {
        if (stepInPhase > 1)
        {
            userText.GetComponent<TextMeshProUGUI>().text = ""; 
        }

        if (currentTrial < retraceRounds)
        {
            if (stepInPhase == 0)
            {
                userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the trigger button";
                maze.SetActive(false);
                moveForwardArrow.SetActive(false);
                footprints.SetActive(true);
                footprints.transform.position = new Vector3(arrowPath[0].x, footprints.transform.position.y, arrowPath[0].y);
                
                if (GetTrigger(false) & ControllerCollider.Instance.controllerSelection.Contains(footprints.name))
                {
                    stepInPhase++;
                    recordCameraAndNodes = true;
                    retraceNodes = 0;
                    footprints.SetActive(false);
                    Debug.Log("Start retracing phase"); 
                    fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_nodePath.csv"), "Start_retrace");
                    recordCameraAndNodes = true;
                    retraceTimer = Time.time;
                }
            }
            else if (stepInPhase < arrowPath.Length)
            {
               
                    
                maze.SetActive(true);
                userText.GetComponent<TextMeshProUGUI>().text = "Retrace your learned route.";
                moveForwardArrow.SetActive(true);
                foreach (var arrowChild in moveForwardArrow.GetComponentsInChildren<MeshRenderer>())
                {
                    arrowChild.enabled = false;
                }

                moveForwardArrow.transform.position =
                    new Vector3(arrowPath[stepInPhase].x, moveForwardArrow.transform.position.y, arrowPath[stepInPhase].y);
                if (ControllerCollider.Instance.controllerSelection.Contains(moveForwardArrow.name))
                {
                    stepInPhase++;
                    ControllerCollider.Instance.controllerSelection = "Not selected";
                    retraceNodes = 0;
                    retraceTimer = Time.time; 
                }
                if(Time.time - retraceTimer > retraceTimeLimit) //if (retraceNodes > 4)
                {
                    stepInPhase = 0;
                    phase--; // Need to relearn
                    learningRedoRounds++;
                    currentTrial = learningRounds-1;
                    moveForwardArrow.GetComponent<MeshRenderer>().enabled = true;
                    foreach (var arrowPart in moveForwardArrow.GetComponentsInChildren<MeshRenderer>())
                    {
                        arrowPart.enabled = true; 
                    }
                }
            }
            else
            {
                currentTrial++;
                recordCameraAndNodes = false;
                stepInPhase = 0;
            }

           
        }
        if(phase == 2 & currentTrial >= retraceRounds)
        {
            fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_numLearning.csv"), learningRedoRounds.ToString());
            currentTrial = 0;
            phase++;
            stepInPhase = 0;
            moveForwardArrow.SetActive(false);
        }
    }

    /// <summary>
    /// Runs the testing phase.
    /// stepInPhase = 0: Reorienting phase, user walks to random location to disorient
    /// stepInPhase = 1: Reorienting phase, user walks to the starting point of the trial and hits the trigger
    /// stepInPhase = 2: User walks to the painting they believe is correct and hits the trigger
    /// stepInPhase = 3: 
    /// </summary>
    void RunTesting()
    {
        foreach (var textMesh in paintings.GetComponentsInChildren<TextMeshPro>())
        {
            textMesh.enabled = false; 
        }
       
        if (currentTrial < trialList.Length)
        {
            // foreach (var painting in paintings.GetComponentsInChildren<MeshRenderer>())
            //     if (hidePaintingsDuringTesting & !painting.name.Contains("frame"))
            //         if (stepInPhase > 2)
            //             painting.enabled = false;  // Hides all text/pictures in the paintings
            //         else if (!painting.name.Contains(GetTrialInfo().start.GetTarget()))
            //             painting.enabled = false;
            switch (stepInPhase)
            {
                case 0: // Reorient
                    maze.SetActive(false);
                    footprints.SetActive(true);
                    footprints.GetComponent<MeshRenderer>().enabled =false;
                    
                    userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the trigger button";
                    if (ControllerCollider.Instance.controllerSelection.Contains(footprints.name) & GetTrigger(false))
                    {
                        float nextX = GetTrialInfo().start.GetX();
                        float nextY = GetTrialInfo().start.GetY();
                        Vector3 rot = new Vector3(0, GetFootprintDir()-90, 0);

                        footprints.transform.position = new Vector3(nextX, footprints.transform.position.y, nextY);
                        footprints.transform.eulerAngles = rot;
                        stepInPhase++;
                    }

                    break;

                case 1: // Go to next start
                    footprints.GetComponent<MeshRenderer>().enabled = true; 
                    if (ControllerCollider.Instance.controllerSelection.Contains(footprints.name) & GetTrigger(false))
                    {
                        
                        footprints.SetActive(false);
                        maze.SetActive(true);
                        stepInPhase++;
                        fileHandler.AppendLine(
                            (ExperimentController.Instance.subjectFile).Replace(Date_time+".csv", "_nodePath.csv"),
                            PrintStepInfo() + "," + GetTrialInfo().start.GetString() + "," + GetTrialInfo().end.GetString());
                       
                    }
                    break;

                case 2: // Wait for them to touch painting
                    userText.GetComponent<TextMeshProUGUI>().text =
                        "Touch the painting and pull the trigger to start trial";
                    if (GetTrigger(true) & ControllerCollider.Instance.controllerSelection.Contains(GetTrialInfo().start.GetString()))
                    {
                        dynamicBlock.enabled = true;
                        stepInPhase++;
                        trialStartTime = Time.time;
                        fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_nodePath.csv"), "Start_Testing");
                        recordCameraAndNodes = true; 
                    }
                        
                    break;

                case 3: // Walk to end
                    recordCameraAndNodes = true;
                    userText.GetComponent<TextMeshProUGUI>().text =
                        "Target Object: " + GetTrialInfo().end.GetTarget();
                    //Debug.Log("Walk to end");
                    if ((GetTrigger(true) & ControllerCollider.Instance.controllerSelection.Contains("targ")) || 
                        (GetTrialInfo().stressTrial & Time.time - trialStartTime >= stressTimeLimit) ||
                        Time.time - trialStartTime >= nonStressTimeLimit)
                    {
                        if (!GetTrialInfo().stressTrial || (subjectNumber%2 != 0 && GetTrialInfo().isWallTrial) || (subjectNumber%2 == 0 && !GetTrialInfo().isWallTrial))
                            blockedWall = "N/A";
                        Debug.Log("Selected " + ControllerCollider.Instance.controllerSelection);
                        fileHandler.AppendLine(subjectFile,
                            PrintStepInfo() + "," + GetTrialInfo() + "," + ControllerCollider.Instance.controllerSelection.Remove(ControllerCollider.Instance.controllerSelection.Length > 2 ? 2 : 0) + "," + blockedWall + "," + GetTrialInfo().stressTrial);
                        maze.SetActive(false);
                        stressLevel.GetComponent<TextMeshProUGUI>().text = "4";
                        dynamicBlock.enabled = false;
                        stepInPhase++;
                    }

                    break;

                case 4: // Rate stress
                    
                    if (SteamVR_Actions._default.SnapTurnLeft.GetStateDown(SteamVR_Input_Sources.Any))
                    {
                        stressLevel.GetComponent<TextMeshProUGUI>().text =
                            (int.Parse(stressLevel.GetComponent<TextMeshProUGUI>().text) - 1).ToString();
                    }

                    if (SteamVR_Actions._default.SnapTurnRight.GetStateDown(SteamVR_Input_Sources.Any))
                    {
                        stressLevel.GetComponent<TextMeshProUGUI>().text =
                            (int.Parse(stressLevel.GetComponent<TextMeshProUGUI>().text) + 1).ToString();
                    }

                    stressLevel.GetComponent<TextMeshProUGUI>().text =
                        Math.Clamp(int.Parse(stressLevel.GetComponent<TextMeshProUGUI>().text), 1, 7).ToString(); 

                    dynamicBlock.DisableWalls();
                    userText.GetComponent<TextMeshProUGUI>().text = "";
                    stressCanvas.enabled = true;

                    recordCameraAndNodes = false;
                    recordCameraAndNodes = false;

                    // select stress, once selected disable stress UI and move phase forward
                    if (GetTrigger(false) )
                    {
                        stressText.GetComponent<TextMeshProUGUI>().text = "Confirm?"; 
                        stepInPhase++; 
                    }
                    break;
                case 5:
                    if (GetTrigger(false))
                    {
                        //move forward
                        stressText.GetComponent<TextMeshProUGUI>().text = "Rate your Stress Level"; 
                        stressCanvas.enabled = false;
                        stepInPhase = 0;
                        footprints.transform.position = new Vector3(Random.Range(-3, 3), footprints.transform.position.y,
                            Random.Range(-3, 3));
                        
                        
                        fileHandler.AppendLine(subjectFile.Replace(Date_time+".csv", "_stress.csv"), GetTrialInfo() + "," + stressLevel.GetComponent<TextMeshProUGUI>().text );

                        currentTrial++;
                        Debug.Log("Current trial: " + currentTrial);
                    }
                    else if (SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
                    {
                        stepInPhase--; 
                        stressText.GetComponent<TextMeshProUGUI>().text = "Rate your Stress Level"; 
                    }

                    break; 


                default:
                    Debug.Log("Unexpected stepInPhase during RunTesting()");
                    break;
            }
        }
        else
        {
            phase++;
        }
    }

    public void UpdateStressOnClick(string stressValue)
    {
        stressLevel.GetComponent<TextMeshProUGUI>().text = stressValue;
    }

    public void ConfirmStressOnClick()
    {
        if (stressLevel.GetComponent<TextMeshProUGUI>().text == "N/A") return;
        confirm = true;
        stressLevel.GetComponent<TextMeshProUGUI>().text = "N/A";
    }

    /// <summary>
    /// Informs the participant (and observer) that the experiment is over through text on the GUI
    /// </summary>
    void FinishExperiment()
    {
        userText.GetComponent<TextMeshProUGUI>().text = "THE END\nThanks for participating!!";
        //TODO update for VR (have a screenSpace canvas and worldSpace canvas)
    }
    
    float GetFootprintDir()
    {
        string loc = GetTrialInfo().start.GetString();
        switch (loc[0])
        {
            case 'A':
                if (loc[1] == '1')
                    return 180f;
                else if (loc[1] == '6')
                    return 90f;
                break;
            case 'B':
                if (loc[1] == '2')
                    return 0f;
                break;
            case 'C':
                if (loc[1] == '6')
                    return 90f;
                else if (loc[1] == '7')
                    return 0f;
                break;
            case 'D':
                if (loc[1] == '1')
                    return 0f;
                else if (loc[1] == '4')
                    return 180f;
                break;
            case 'E':
                if (loc[1] == '6')
                    return 270f;
                break;
            case 'F':
                if (loc[1] == '1')
                    return 180f;
                else if (loc[1] == '6')
                    return 0f;
                break;
            case 'G':
                if (loc[1] == '2')
                    return 270f;
                else if (loc[1] == '5')
                    return 270f;
                break;
            default:
                Debug.Log(">>Invalid Painting Loc");
                break;
        }
        return 43f; //garbage value, should be obvious if it fails
    }


    private float triggerTimer = 0;
    /// <summary>
    /// Checks if the trigger (or spacebar) is pressed, has a half second delay before it will read a subsequent
    /// trigger press.
    /// </summary>
    /// <returns> if(trigger & >.5 seconds since last press){return true};</returns>
    private bool GetTrigger(bool forPainting)
    {
        if ((SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.Any) || Input.GetKeyDown(KeyCode.Space)) &
            Time.time - triggerTimer > 1)
        {
            redScreen.enabled = true;
            redFlashTimer = Time.time; 
            triggerTimer = Time.time;
            if (forPainting) return ControllerCollider.Instance.PaintingCheck();
            return true;
        }
        

        return false;
    }

    /// <summary>
    /// Gets the current Trial info
    /// </summary>
    public Trial GetTrialInfo()
    {
        return trialList[trialOrder[currentTrial]];
    }
    

    /// <summary>
    /// Returns trial ID, time in trial, phase, trial number, step in phase
    /// </summary>
    /// <returns></returns>
    public string PrintStepInfo()
    {
        return trialOrder[currentTrial] + "," + (Time.time - trialStartTime) + "," + phase + "," + currentTrial + "," + stepInPhase;
    }

    public void ChangeBlockingOrder()
    {
        stressFirst = !stressFirst;

    }

    void RandomizeTrialOrder()
    {
        // ** Randomize trial order **
        Random.InitState(subjectNumber * 10); // Insures same path randomizations every run for same subject (in case the experiment needs restarted)
        trialOrder = new int[trialList.Length];
        for (int i = 0; i < trialOrder.Length; i++)
        {
            trialOrder[i] = i;
            Debug.Log(trialOrder[i]); 
        }

        for (int t = number_practice_trials; t < trialOrder.Length; t++)
        {
            int tmp = trialOrder[t];
            int r = Random.Range(t, trialOrder.Length);
            trialOrder[t] = trialOrder[r];
            trialOrder[r] = tmp;
            Debug.Log(trialOrder[t]); 
        }
    }

    // The following code will make instance of ExperimentController persist between scenes and destroy subsequent instances
    void Awake()
    {
        QualitySettings.vSyncCount = 2;
        Application.targetFrameRate = 45;
        Debug.Log(Application.targetFrameRate); 
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        StartData startData = GameObject.FindGameObjectWithTag("StartData").GetComponent<StartData>();
        stressFirst = startData.stressFirst;
        subjectNumber = startData.subjNum;
        currentTrial = startData.trialNum;
        phase = (int) startData.phase;
        stressCanvas.enabled = false;
        RunStartup(phase);
    }
}
