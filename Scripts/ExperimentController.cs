using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Classes;
using DefaultNamespace;
using VR; 
using DynamicBlocking;

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
    [SerializeField] public bool isTraining = false;
    [SerializeField] public Canvas introCanvas;
    [SerializeField] public Canvas stressCanvas;
    [SerializeField] private GameObject maze;
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject footprints;
    [SerializeField] private GameObject moveForwardArrow;
    [SerializeField] public GameObject stressLevel;
    [SerializeField] public GameObject stressText;
    [SerializeField] public GameObject player;
    [SerializeField] private GameObject playerCam;
    //[SerializeField] private DynamicBlock dynamicBlock;
    [SerializeField] private TMP_InputField subjectNum, trialNum; 
    
    //[SerializeField] private GameObject subjectInput;
    //[SerializeField] private GameObject trialInput;
    [SerializeField] private GameObject Panel;
    [SerializeField] private Canvas debugCanvas; 
    [SerializeField] private GameObject userText;
    [SerializeField] private RawImage redScreen;
    [SerializeField] private Image blankScreen;
    private bool resettingWall = false;
    
    [SerializeField] private GameObject paintings;
    [SerializeField] private GameObject node;

    [SerializeField] private int learningRounds = 6;
    [SerializeField] public bool stressLearning = false;
    [SerializeField] private int retraceRounds = 1;
    private int learningRedoRounds = 0;
    [SerializeField] private float retraceTimeLimit = 10.0f;
    [SerializeField] public float stressTimeLimit = 15.0f;
    [SerializeField] private float nonStressTimeLimit = 30.0f;

    [SerializeField] private int number_practice_trials = 2;
    private int numTrials = 24;
    [SerializeField] private bool stressFirst = false;

    #region StressVars
    #endregion

    public int subjectNumber = 0;

    public Vector3 _lastGazeDirection = new Vector3(.5f, .5f, 1f);
    public int phase = 0;
    public int stepInPhase = 0;
    public int currentTrial;
    public float trialStartTime = 0.0f;
    private float redFlashTimer = 0.0f;
    [SerializeField] private float redFlashTimeLimit = .5f;


    public FileHandler fileHandler = new FileHandler();
    public string subjectFile;
    public string Date_time;
    public bool recordCameraAndNodes = false;
    public bool confirm = false;
    public string blockedWall = "";
    private bool replayMode = false;

    private Queue<string> testWalls;

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
        new Vector3(-3.0f, 3.0f, 0.0f),
        new Vector3(1.0f, 3.0f, 0.0f),
        new Vector3(3.0f, 3.0f, 90.0f),
        new Vector3(3.0f, 1.0f, 180.0f),
        new Vector3(0.0f, 1.0f, 90.0f),
        new Vector3(0f, -1.0f, 0.0f),
        new Vector3(2.0f, -1.0f, 90.0f),
        new Vector3(2.0f, -2.0f, 90.0f),
        new Vector3(2.0f, -3.0f, 180.0f)
    };
    
    public Trial[] trialList =
    {
        // Practice trials
        new Trial(new GridLocation("A", 1), new GridLocation("A", 6), false, false, false),
        new Trial(new GridLocation("G", 2), new GridLocation("B", 2), false, false, false),
        
        // // 8 Non-stress trials (with & without audio) 
        new Trial(new GridLocation("F", 6), new GridLocation("B", 2), false, false, false),
        new Trial(new GridLocation("B", 2), new GridLocation("G", 5), false, false, false),
        new Trial(new GridLocation("F", 1), new GridLocation("C", 7), false, false, false),
        new Trial(new GridLocation("C", 6), new GridLocation("F", 1), false, false, false),
        new Trial(new GridLocation("C", 6), new GridLocation("A", 1), false, true, false),
        new Trial(new GridLocation("G", 2), new GridLocation("C", 6), false, true, false),
        new Trial(new GridLocation("E", 6), new GridLocation("D", 1), false, true, false),
        new Trial(new GridLocation("C", 7), new GridLocation("D", 1), false, true, false),
        
        // new Trial(new GridLocation("D", 1), new GridLocation("D", 4), false, false),
        // new Trial(new GridLocation("D", 4), new GridLocation("F", 1), false, false),
        // new Trial(new GridLocation("A", 6), new GridLocation("E", 6), false, false),
        // new Trial(new GridLocation("G", 5), new GridLocation("A", 6), false, false),
        
        // 8 Stress trials (with & without audio) 
        // blockedList = {7,6,5,11,3,9};
        new Trial(new GridLocation("A", 6), new GridLocation("G", 2), true, false, true),
        new Trial(new GridLocation("G", 2), new GridLocation("C", 7), true, false, false),
        new Trial(new GridLocation("F", 1), new GridLocation("E", 6), true, false, true),
        new Trial(new GridLocation("C", 7), new GridLocation("B", 2), true, false, true),
        // new Trial(new GridLocation("A", 1), new GridLocation("D", 4), true, true),
        // new Trial(new GridLocation("F", 6), new GridLocation("A", 6), true, true),
        new Trial(new GridLocation("A", 1), new GridLocation("F", 6), true, true, false),
        new Trial(new GridLocation("E", 6), new GridLocation("A", 1), true, true, true),
        new Trial(new GridLocation("D", 1), new GridLocation("F", 6), true, true, false),
        new Trial(new GridLocation("B", 2), new GridLocation("C", 6), true, true, true),
        // new Trial(new GridLocation("D", 4), new GridLocation("G", 2), true, false, true),
        // new Trial(new GridLocation("G", 5), new GridLocation("C", 7), true, false, true),
        // new Trial(new GridLocation("G", 5), new GridLocation("C", 7), true, false, true),
    };
    
    private string[] obstaclesList = { "B1", "B3", "B5", "B6", "D2", "D3", "D5", "D6", "F2", "F4", "F5", "F7" };

    [SerializeField] private int[] trialOrder = { 0 }; //Randomized at start


    /* Debug */
    [SerializeField] bool debugActive = true;
    [SerializeField] private GameObject phaseDisplay;
    [SerializeField] private GameObject stepDisplay;
    [SerializeField] private GameObject trialDisplay;
    [SerializeField] private GameObject gazeTrackingDisplay;
    [SerializeField] private GameObject pause;

    // private void OnEnable()
    // {
    //     dynamicBlock.onWallActivated += ActivateNextWall;
    // }
    //
    // private void OnDisable()
    // {
    //     dynamicBlock.onWallActivated -= ActivateNextWall;
    // }

    /// <summary>
    /// Called every frame. Checks which phase of the experiment to run then calls the correct function
    /// </summary>
    void Update()
    {
        // if (phase !=0) {   //Wait for start canvas to complete.
            if (!replayMode) {
                if (XRSettings.enabled && SteamVR.calibrating) {
                    pause.SetActive(true);
                    Time.timeScale = 0f; }
                else {
                    if (Time.timeScale == 0f) {
                        pause.SetActive(false);
                        Time.timeScale = 1f; } }

                if (Time.realtimeSinceStartup - redFlashTimer > redFlashTimeLimit)
                    redScreen.enabled = false;
                DisplayDebugInfo();
                // Different phases of experiment
                switch (phase) {
                    case 0:
                        Panel.SetActive(true);
                        maze.SetActive(false);
                        stressCanvas.enabled = false; 
                        userText.GetComponent<TextMeshProUGUI>().text = "Input subject/trial number and select phase";
                        break;
                    case 1:
                        RunLearning();
                        break;
                    case 2:
                        RunRetrace();
                        break;
                    case 3:
                        // Debug.Log(_lastGazeDirection);
                        // DisplayDebugInfo();
                        RunTesting();
                        break;
                    default:
                        FinishExperiment();
                        StartCoroutine(WaitCoroutine());
                        Application.Quit();
                        break; }

                if (recordCameraAndNodes) {
                    RecordNodes(); } 

            // else {
            //     /* "Trial_ID,TrialTime,Phase,TrialNumber,StepInPhase,Start,End," +
            //          "CamRotX,CamRotY,CamRotZ,CamPosX,CamPosY,CamPosZ,ScreenGazeX,ScreenGazeY,WorldGazeX,WorldGazeY,WorldGazeZ" 
            //     split_lines = new string[System.IO.File.ReadAllLines(StartData.instance.replayFile).Length][];
            //     int count = 0;
            //     sr = new StreamReader(StartData.instance.replayFile);
            //     
            //     while (!sr.EndOfStream) {
            //         // Should input every line split into the array
            //         split_lines[count] = sr.ReadLine().Split(',');} */
            //     // stressCanvas.enabled = false; 
            //     // userText.GetComponent<TextMeshProUGUI>().text = "Input subject/trial number and select phase";
            // } 
        } 
    } 

    
    private GridLocation lastLoc;
   void RecordNodes() {
           if (NodeExtension.CurrentNode(player.transform.position) != lastLoc)
               fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv",
                   "_nodePath.csv"), NodeExtension.CurrentNode(player.transform.position).GetString()); }


    private StreamReader sr;
    private string[][] split_lines;
    
    private int currentFrame = 0;
    [SerializeField] private GameObject camera;
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (replayMode)
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
        debugCanvas.enabled = XRSettings.enabled; 
        if (debugActive)
        {
            phaseDisplay.GetComponent<TextMeshProUGUI>().text = "Phase: " + phase.ToString();
            stepDisplay.GetComponent<TextMeshProUGUI>().text = "Step: " + stepInPhase.ToString();
            trialDisplay.GetComponent<TextMeshProUGUI>().text = "Trial: " + currentTrial;
            gazeTrackingDisplay.GetComponent<TextMeshProUGUI>().text = "Gaze: " + _lastGazeDirection.ToString();
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
        // Takes user input 
        subjectNumber = Int32.Parse((string) subjectNum.text.ToString());
        currentTrial = Int32.Parse((string) trialNum.text.ToString()); 
        Date_time = "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "-" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;
        subjectFile = Application.dataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + subjectNumber + Date_time + ".csv";
        Debug.Log(subjectFile);
        // Append data to file
        fileHandler.AppendLine(
            ExperimentController.Instance.subjectFile.Replace(ExperimentController.Instance.Date_time + ".csv",
                "_camera_tracker.csv"), "Trial_ID,TrialTime,Phase,TrialNumber,StepInPhase,Start,End," +
                                        "CamRotX,CamRotY,CamRotZ,CamPosX,CamPosY,CamPosZ,ScreenGazeX,ScreenGazeY,WorldGazeX,WorldGazeY,WorldGazeZ");
        // TAKE OUT: blocked Wall, ADD hasAudio
        fileHandler.AppendLine(subjectFile, "trialID,timeInTrial,phase,trialNumber,stepInPhase,start,goal,selected,isStressTrial,hasAudio");
        fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_nodePath.csv"),
            DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Now.Hour + ":" + DateTime.Now.Minute);
        
        fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_stress.csv"),
            "start,end,stress_level");

        // Blocking
        trialOrder = new int[trialList.Length];
        for (int i = 0; i < number_practice_trials; i++)
        {
            trialOrder[i] = i;
            Debug.Log(trialOrder[i]);
        }

        List<int> stress_b = new List<int>();
        List<int> stress_nb = new List<int>();
        List<int> fixedstress = new List<int>();
        List<int> nonStress = new List<int>();
        for (int i = number_practice_trials; i < trialList.Length; i++) {
            if (trialList[i].stressTrial)
                if((subjectNumber % 2 != 0 && trialList[i].isWallTrial) || (subjectNumber % 2 == 0 && !trialList[i].isWallTrial))
                    stress_nb.Add(i);
                else
                    stress_b.Add(i);
            else
                nonStress.Add(i); }

        Random.InitState(subjectNumber * 10);

        int x = Random.Range(0,stress_b.Count);
        fixedstress.Add(stress_b[x]);
        List<int> stress = new List<int>();
        for (int i = stress_b[0]; i < (stress_b[0] + stress_b.Count); i++)
        {
            if (i == (stress_b[0] + x))
                continue;
            stress.Add(i);
        }
        stress = stress.Concat(stress_nb).ToList();
        
        stress = stress.ToArray().OrderBy(x => Random.Range(0, stress.Count)).ToList();
        nonStress = nonStress.ToArray().OrderBy(x => Random.Range(0, nonStress.Count)).ToList();
        stress = fixedstress.Concat(stress).ToList();
        
        
        if (stressFirst){
            for (int i = 0; i < stress.Count; i++)
                trialOrder[number_practice_trials + i] = stress[i];
            for (int i = 0; i < nonStress.Count; i++)
                trialOrder[number_practice_trials + stress.Count + i] = nonStress[i]; }
        else {
            for (int i = 0; i < nonStress.Count; i++)
                trialOrder[number_practice_trials + i] = nonStress[i];
            for (int i = 0; i < stress.Count; i++)
                trialOrder[number_practice_trials + nonStress.Count + i] = stress[i];
        }

        // Create a web of invisible node colliders to track position
        string[] letters = { "A", "B", "C", "D", "E", "F", "G" };
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7 };
        GameObject[] nodes = new GameObject[49];
        for (int letter = 0; letter < letters.Length; letter++){
            for (int number = 1; number <= 7; number++) {
                int currentPos = number - 1 + letter * 7;
                nodes[currentPos] = GameObject.Instantiate(node);
                nodes[currentPos].name = letters[letter] + number;

                if (!obstaclesList.Contains(nodes[currentPos].name)) {
                    GridLocation nodeLocation = new GridLocation(letters[letter], number);
                    nodes[currentPos].transform.position =
                        new Vector3(nodeLocation.GetX(), node.transform.position.y, nodeLocation.GetY()); }
                else
                    nodes[currentPos].SetActive(false); // To avoid erroneous recordings from inside walls
            } }
        introCanvas.enabled = false;
        Panel.SetActive(false);
        maze.SetActive(true);
        phase = phaseNumberStart; 
    }


    /// <summary>
    /// Requires participant to walk to starting point of learning phase and hit the trigger.
    /// Once started, the arrow moves along the path specified (at the start of this file) each
    /// time the participant collides with it.  This repeats for how ever many learningRounds are
    /// specified. 
    /// </summary>
    [SerializeField] private GameObject stressTimer;
    void RunLearning()
    {
        float arrowHeight = moveForwardArrow.transform.position.y;
        Debug.Log(arrowHeight);

        if (currentTrial >= learningRounds)
        {
            Debug.Log("Move to retracing phase");
            moveForwardArrow.SetActive(false);
            currentTrial = 0;
            fileHandler.AppendLine(
                subjectFile.Replace(Date_time + ".csv", "_desktop_Parameter.csv"),player.GetComponent<SimpleFirstPersonMovement>().mouseSensitivity.ToString());
            Debug.Log("file");
            phase++;
        }
        else
        {
            // if (stepInPhase == 0)
            // {
            //     userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the" + (XRSettings.enabled ? " trigger " : " space " ) + "button";
            //     maze.SetActive(false);
            //     moveForwardArrow.SetActive(false);
            //     footprints.SetActive(true);
            //     footprints.transform.position =
            //         new Vector3(arrowPath[0].x, footprints.transform.position.y, arrowPath[0].y);
            //     if (GetTrigger(false) & NodeExtension.SameNode(player, footprints))
            //     {
            //         recordCameraAndNodes = true;
            //         fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_nodePath.csv"), "Learning Phase");
            //         stepInPhase++;
            //     }
            // }
            // If have not gone through all arrows in learning phase yet
            if (stepInPhase < arrowPath.Length)
            {
                if (stepInPhase == 0)
                {
                    blankScreen.color = new Color(blankScreen.color.r, blankScreen.color.g, blankScreen.color.b, 1);
                    userText.GetComponent<TextMeshProUGUI>().text = "Left-Click to Start";
                    if (Input.GetMouseButtonDown(0))
                    {
                        trialStartTime = Time.realtimeSinceStartup;
                        stepInPhase++;
                        player.GetComponent<SimpleFirstPersonMovement>().active = false;
                        playerCam.transform.position = new Vector3(arrowPath[0].x,arrowHeight,arrowPath[0].y);
                        // moveForwardArrow.transform.position = new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
                        // moveForwardArrow.transform.rotation = Quaternion.Euler(moveForwardArrow.transform.rotation.eulerAngles.x, arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);
                        StartCoroutine(FadeScreen());
                        startTimer = true;
                        playerCam.transform.rotation = Quaternion.Euler(0,-90,0);
                        recordCameraAndNodes = true;
                        player.GetComponent<SimpleFirstPersonMovement>().active = true;
                        userText.GetComponent<TextMeshProUGUI>().text = "Learn the path by following the arrow";
                        fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_nodePath.csv"), "Learning Phase");
                        // if (currentTrial >= 3 & stressLearning)
                        // {
                        //     testWalls = new();
                        //     switch (currentTrial)
                        //     {
                        //         case 3:
                        //             //dynamicBlock.testWall = "E1";
                        //             testWalls.Enqueue("C5");
                        //             break;
                        //         case 4:
                        //             //dynamicBlock.testWall = "G3";
                        //             testWalls.Enqueue("A3");
                        //             testWalls.Enqueue("E5");
                        //             break;
                        //         case 5:
                        //             //dynamicBlock.testWall = "B7";
                        //             testWalls.Enqueue("E5");
                        //             break;
                        //         default:
                        //             //dynamicBlock.testWall = "";
                        //             break;
                        //     }
                           // dynamicBlock.enabled = true;
                            resettingWall = false;
                        
                    }
                }

                // if (dynamicBlock.enabled == false && currentTrial >= 3 && !resettingWall && stressLearning)
                // {
                //     StartCoroutine(ResetWall(2f));
                // }

                maze.SetActive(true);
                footprints.SetActive(false);
                moveForwardArrow.SetActive(true);
                moveForwardArrow.transform.position =
                    new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
                moveForwardArrow.transform.rotation = Quaternion.Euler(
                    moveForwardArrow.transform.rotation.eulerAngles.x,
                    arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);

                if (NodeExtension.SameNode(player, moveForwardArrow) & stepInPhase!=0)
                {
                    stepInPhase++;
                    //recordCameraAndNodes = false;
                }
            }

            if (stepInPhase > 1)
            {
                userText.GetComponent<TextMeshProUGUI>().text = "";
                
            }

            if (stepInPhase >= arrowPath.Length-1)
            {
                Debug.Log("Increment current trial");
                stressTimer.SetActive(false);
                recordCameraAndNodes = false;
                currentTrial++;
                startTimer = false;
                stepInPhase = 0;
                player.GetComponent<SimpleFirstPersonMovement>().active = false;
                player.GetComponent<SimpleFirstPersonMovement>().rotation = Vector2.zero;
                if (learningRedoRounds>0)
                {
                    Debug.Log("Move to retracing phase");
                    moveForwardArrow.SetActive(false);
                    currentTrial = 0;
                    fileHandler.AppendLine(
                        subjectFile.Replace(Date_time + ".csv", "_desktop_Parameter.csv"),player.GetComponent<SimpleFirstPersonMovement>().mouseSensitivity.ToString());
                    phase++;
                }
                // playerCam.transform.rotation = Quaternion.Euler(0,-90,0);
                // player.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

        }

    }

    private IEnumerator ResetWall(float duration)
    {
        resettingWall = true;
        yield return new WaitForSecondsRealtime(duration);
        //dynamicBlock.DisableWalls();
        if (testWalls.Count > 0)
        {
            string testWall = testWalls.Dequeue();
            //dynamicBlock.testWall = testWall;
            //dynamicBlock.enabled = true;
        }
        
    }

    // private void ActivateNextWall()
    // {
    //     if (phase == 1)
    //     {
    //         StartCoroutine(ResetWall(2f));
    //     }
    // }

    private IEnumerator FadeScreen()
    {
        float t = 0;
        float duration = 0.5f;
        Color start = blankScreen.color;
        Color end = new Color(blankScreen.color.r, blankScreen.color.g, blankScreen.color.b, 0);
        while (t <= duration)
        {
            t += Time.deltaTime;
            blankScreen.color = Color.Lerp(start, end, t / duration);
            yield return null;
        }

        // blankScreen.color = end;
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
                // userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the" + (XRSettings.enabled ? " trigger " : " space " ) + "button";
                // maze.SetActive(false);
                // moveForwardArrow.SetActive(false);
                // footprints.SetActive(true);
                // footprints.transform.position = new Vector3(arrowPath[0].x, footprints.transform.position.y, arrowPath[0].y);
                //
                // if (GetTrigger(false) & NodeExtension.SameNode(player, footprints))
                // {
                //     stepInPhase++;
                //     recordCameraAndNodes = true;
                //     retraceNodes = 0;
                //     footprints.SetActive(false);
                //     Debug.Log("Start retracing phase");
                //     fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_nodePath.csv"), "Start_retrace");
                //     recordCameraAndNodes = true;
                //     retraceTimer = Time.realtimeSinceStartup;
                // }
                blankScreen.color = new Color(blankScreen.color.r, blankScreen.color.g, blankScreen.color.b, 1);
                userText.GetComponent<TextMeshProUGUI>().text = "Left-Click to Start";
                footprints.SetActive(false);
                if (Input.GetMouseButtonDown(0))
                {
                    trialStartTime = Time.realtimeSinceStartup;
                    stepInPhase++;
                    // player.GetComponent<SimpleFirstPersonMovement>().active = false;
                    playerCam.transform.position = new Vector3(arrowPath[0].x,moveForwardArrow.transform.position.y,arrowPath[0].y);
                    // moveForwardArrow.transfor  m.position = new Vector3(arrowPath[stepInPhase].x, arrowHeight, arrowPath[stepInPhase].y);
                    // moveForwardArrow.transform.rotation = Quaternion.Euler(moveForwardArrow.transform.rotation.eulerAngles.x, arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z);
                    StartCoroutine(FadeScreen());
                    retraceNodes = 0;
                    retraceTimer = Time.realtimeSinceStartup;
                    playerCam.transform.rotation = Quaternion.Euler(0,-90,0);
                    recordCameraAndNodes = true;
                    player.GetComponent<SimpleFirstPersonMovement>().active = true;
                    fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_nodePath.csv"), "Start_retrace");
                    
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

                if(stepInPhase < arrowPath.Length-1)
                    moveForwardArrow.transform.position =
                        new Vector3(arrowPath[stepInPhase].x, moveForwardArrow.transform.position.y, arrowPath[stepInPhase].y);
                if (NodeExtension.SameNode(player, moveForwardArrow))
                {
                    stepInPhase++;
                    retraceNodes = 0;
                    retraceTimer = Time.realtimeSinceStartup;
                }
                if (Time.realtimeSinceStartup - retraceTimer > retraceTimeLimit) //if (retraceNodes > 4)
                {
                    stepInPhase = 0;
                    phase--; // Need to relearn
                    learningRedoRounds++;
                    currentTrial = 0;
                    moveForwardArrow.GetComponent<MeshRenderer>().enabled = true;
                    foreach (var arrowPart in moveForwardArrow.GetComponentsInChildren<MeshRenderer>())
                    {
                        arrowPart.enabled = true;
                    }
                    player.GetComponent<SimpleFirstPersonMovement>().active = false;
                    // playerCam.transform.position = new Vector3(arrowPath[0].x,moveForwardArrow.transform.position.y,arrowPath[0].y);
                    player.GetComponent<SimpleFirstPersonMovement>().rotation = Vector2.zero;
                    // playerCam.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                currentTrial++;
                recordCameraAndNodes = false;
                stepInPhase = 0;
                player.GetComponent<SimpleFirstPersonMovement>().active = false;
                player.GetComponent<SimpleFirstPersonMovement>().rotation = Vector2.zero;
                // playerCam.transform.rotation = Quaternion.Euler(0, 0, 0);
            }


        }
        if (phase == 2 & currentTrial >= retraceRounds)
        {
            fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_numLearning.csv"), learningRedoRounds.ToString());
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
        playerCam.transform.position = Vector3.zero;
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
                    footprints.GetComponent<MeshRenderer>().enabled = false;

                    userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the " + (XRSettings.enabled ? "trigger" : "space" ) + " button";
                    if (NodeExtension.SameNode(player, footprints) & GetTrigger(false))
                    {
                        float nextX = GetTrialInfo().start.GetX();
                        float nextY = GetTrialInfo().start.GetY();
                        Vector3 rot = new Vector3(0, GetFootprintDir() - 90, 0);

                        footprints.transform.position = new Vector3(nextX, footprints.transform.position.y, nextY);
                        footprints.transform.eulerAngles = rot;
                        stepInPhase++;
                    }

                    break;

                case 1: // Go to next start
                    footprints.GetComponent<MeshRenderer>().enabled = true;
                    if (NodeExtension.SameNode(player, footprints) & GetTrigger(false))
                    {

                        footprints.SetActive(false);
                        maze.SetActive(true);
                        stepInPhase++;
                        fileHandler.AppendLine(
                            (subjectFile).Replace(Date_time + ".csv", "_nodePath.csv"),
                            PrintStepInfo() + "," + GetTrialInfo().start.GetString() + "," + GetTrialInfo().end.GetString());

                    }
                    break;

                case 2: // Wait for them to touch painting
                    Debug.Log("Touch painting"); 
                    userText.GetComponent<TextMeshProUGUI>().text =
                        "Touch the painting and press " + (XRSettings.enabled ? "trigger" : "space" ) + " to start trial";
                    if (GetTrigger(true) )
                    {
                        //dynamicBlock.enabled = true;
                        stepInPhase++;
                        trialStartTime = Time.realtimeSinceStartup;
                        fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_nodePath.csv"), "Start_Testing");
                        recordCameraAndNodes = true;
                    }

                    break;

                case 3: // Walk to end
                    recordCameraAndNodes = true;
                    userText.GetComponent<TextMeshProUGUI>().text =
                        "Target Object: " + GetTrialInfo().end.GetTarget();
                    //Debug.Log("Walk to end");
                    if (GetTrigger(true)  ||
                        (GetTrialInfo().stressTrial & Time.realtimeSinceStartup - trialStartTime >= stressTimeLimit) ||
                        Time.realtimeSinceStartup - trialStartTime >= nonStressTimeLimit)
                    {
                        if (!GetTrialInfo().stressTrial || (subjectNumber % 2 != 0 && GetTrialInfo().isWallTrial) || (subjectNumber % 2 == 0 && !GetTrialInfo().isWallTrial))
                            blockedWall = "N/A";
                        
                        fileHandler.AppendLine(subjectFile,
                            PrintStepInfo() + "," + GetTrialInfo() + "," + NodeExtension.CurrentNode(player.transform.position).GetString()  + "," + GetTrialInfo().stressTrial + "," + GetTrialInfo().hasAudio + "," + GetTrialInfo().hasExplosion);
                        maze.SetActive(false);
                        stressLevel.GetComponent<TextMeshProUGUI>().text = "4";
                        //dynamicBlock.enabled = false;
                        stepInPhase++;
                    }

                    break;

                case 4: // Rate stress
                    floor.SetActive(false);
                    stressCanvas.enabled = true;
                    // Set explosion to false so can be reactivated for next trial 
                    Explosion.explosionActivated = false;
                    //StressFactors.spatialAudio.Stop();
                    if (XRSettings.enabled && SteamVR_Actions._default.SnapTurnLeft.GetStateDown(SteamVR_Input_Sources.Any) ||
                         Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        stressLevel.GetComponent<TextMeshProUGUI>().text =
                            (int.Parse(stressLevel.GetComponent<TextMeshProUGUI>().text) - 1).ToString();
                    }

                    if (XRSettings.enabled && SteamVR_Actions._default.SnapTurnRight.GetStateDown(SteamVR_Input_Sources.Any)||
                        Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        stressLevel.GetComponent<TextMeshProUGUI>().text =
                            (int.Parse(stressLevel.GetComponent<TextMeshProUGUI>().text) + 1).ToString();
                    }

                    stressLevel.GetComponent<TextMeshProUGUI>().text =
                        Math.Clamp(int.Parse(stressLevel.GetComponent<TextMeshProUGUI>().text), 1, 7).ToString();

                    //dynamicBlock.DisableWalls();
                    userText.GetComponent<TextMeshProUGUI>().text = "";

                    recordCameraAndNodes = false;
                    recordCameraAndNodes = false;

                    // select stress, once selected disable stress UI and move phase forward
                    if (GetTrigger(false))
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
                        floor.SetActive(true);
                        stepInPhase = 0;
                        footprints.transform.position = new Vector3(Random.Range(-3, 3), footprints.transform.position.y,
                            Random.Range(-3, 3));


                        fileHandler.AppendLine(subjectFile.Replace(Date_time + ".csv", "_stress.csv"), GetTrialInfo() + "," + stressLevel.GetComponent<TextMeshProUGUI>().text);

                        currentTrial++;
                        Debug.Log("Current trial: " + currentTrial);
                    }
                    else if (XRSettings.enabled && SteamVR_Actions._default.GrabGrip.GetStateDown(SteamVR_Input_Sources.Any))
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


    private float lastTrigger; 
    private float triggerTimer = 1.5f;
    public bool startTimer = false;


    /// <summary>
    /// Checks if the trigger (or spacebar) is pressed, has a half second delay before it will read a subsequent
    /// trigger press.
    /// </summary>
    /// <returns> if(trigger & >.5 seconds since last press){return true};</returns>
    private bool GetTrigger(bool forPainting)
    {
        if (Time.realtimeSinceStartup-lastTrigger > triggerTimer && ((XRSettings.enabled && SteamControllerVR.Instance.TriggerPressed) || Input.GetKeyDown(KeyCode.Space) ))
        {
            Debug.Log(Time.realtimeSinceStartup);
            Debug.Log(lastTrigger);
            redScreen.enabled = true;
            redFlashTimer = Time.realtimeSinceStartup;
            lastTrigger = Time.realtimeSinceStartup;
            if (forPainting)
                return NodeExtension.SameNode(player, GetTrialInfo().end) && stepInPhase == 3 ||
                       NodeExtension.SameNode(player, GetTrialInfo().start) && stepInPhase == 2;

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
        return trialOrder[currentTrial] + "," + (Time.realtimeSinceStartup - trialStartTime) + "," + phase + "," + currentTrial + "," + stepInPhase; }

    public void ChangeBlockingOrder()
    {
        stressFirst = !stressFirst;
    }
    
    public void ChangeLearning()
    {
        stressLearning = !stressLearning;
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
    
}