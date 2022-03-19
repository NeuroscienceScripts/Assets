using System;
using System.IO;
using System.Linq;
using Classes;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// On each frame, ExperimentController checks which phase you are in (learning, retracing, or testing).
/// Within each phase, ExperimentController checks what step of the phase you are on and acts accordingly.
/// For more info on each phase, see function's summary
/// </summary>
public class ExperimentController : MonoBehaviour
{
    public static ExperimentController  Instance { get; private set; }

    [SerializeField] public Canvas introCanvas;
    [SerializeField] public Canvas stressCanvas;
    [SerializeField] private GameObject maze;
    [SerializeField] private GameObject footprints;
    [SerializeField] private GameObject moveForwardArrow;
    [SerializeField] public GameObject stressLevel;

    [SerializeField] private GameObject subjectInput;
    [SerializeField] private GameObject trialInput;
    [SerializeField] private GameObject userText;

    [SerializeField] private bool hidePaintingsDuringTesting = true; 
    [SerializeField] private GameObject paintings;
    [SerializeField] private GameObject node; 
    
    [SerializeField] private int learningRounds = 2;
    [SerializeField] private int retraceRounds = 2;
    private int learningRedoRounds = 0; 
    
    [SerializeField] private int number_practice_trials = 3;
   
    private int subjectNumber = 0;

    public int phase = 0;
    public int stepInPhase = 0; 
    public int currentTrial;
    public bool confirm = false;

    private FileHandler fileHandler = new FileHandler();
    public string subjectFile;
    public bool recordCameraAndNodes = false; 

    private Vector3[] arrowPath =
    {
        // x location, z location, rotation (east=0, south=90, west=180, north=270)
        //todo fix arrows
        new Vector3(2.0f, -3.0f, 180.0f),
        new Vector3(-3.0f, -3.0f, 270.0f),
        new Vector3(-3.0f, 1.0f, 0.0f),
        new Vector3(-2.0f, 1.0f, 270.0f),
        new Vector3(-2.0f, 1.0f, 270.0f),
        new Vector3(-2.0f, 2.0f, 180.0f),
        new Vector3(-3.0f, 3.0f, 0.0f),
        new Vector3(1.0f, 3.0f, 180.0f),
        new Vector3(3.0f, 1.0f, 90.0f),
        new Vector3(0.0f, 1.0f, 90.0f),
        new Vector3(0f, -1.0f, 0.0f),
        new Vector3(2.0f, -1.0f, 90.0f),
    }; 

    //todo Add trials to this list
    private Trial[] trialList =
    {
        new Trial(new GridLocation("A", 1), new GridLocation("C", 1)),
        new Trial(new GridLocation("A", 1), new GridLocation("D", 1))
    };

    private string[] obstaclesList = {"B1", "B3", "B5", "B6", "D2", "D3", "D5", "D6", "F2", "F4", "F5", "F7"};

    private int[] trialOrder; //Randomized at start

    /* Debug */
    [SerializeField] bool debugActive = true; 
    [SerializeField] private GameObject phaseDisplay;
    [SerializeField] private GameObject stepDisplay; 
    
    /// <summary>
    /// Called every frame. Checks which phase of the experiment to run then calls the correct function
    /// </summary>
    void Update()
    {
        DisplayDebugInfo(); 

        switch (phase)
        {
            case 0:
                userText.GetComponent<TextMeshProUGUI>().text = "Input subject/trial number and select phase"; 
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
                break;
        }
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
        }
        else
        {
            phaseDisplay.SetActive(false);
            stepDisplay.SetActive(false);
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
        subjectNumber = int.Parse(subjectInput.GetComponent<TMP_InputField>().text);
        subjectFile = Application.dataPath + Path.DirectorySeparatorChar + "Data"+ Path.DirectorySeparatorChar + subjectNumber + ".csv";
        fileHandler.AppendLine(subjectFile,
            "Start,End,Selected," + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Now.Hour + ":" +
            DateTime.Now.Minute); 
        fileHandler.AppendLine(subjectFile.Replace(".csv", "_nodePath.csv"),
            DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Now.Hour + ":" + DateTime.Now.Minute); 

        currentTrial = int.Parse(trialInput.GetComponent<TMP_InputField>().text);
        phase = phaseNumberStart;
        introCanvas.enabled = false; 
        
        // ** Randomize trial order **
        Random.InitState(subjectNumber * 10); // Insures same path randomizations every run for same subject (in case the experiment needs restarted)
        trialOrder = new int[trialList.Length];
        for(int i=0; i<trialOrder.Length; i++)
            trialOrder[i] = i;
        for(int t=number_practice_trials; t<trialOrder.Length; t++)
        {
            int tmp = trialOrder[t];
            int r = Random.Range(t, trialOrder.Length);
            trialOrder[t] = trialOrder[r];
            trialOrder[r] = tmp;
        }

        // Create a web of invisible node colliders to track position
        string[] letters = {"A", "B", "C", "D", "E", "F", "G"};
        int[] numbers = {1, 2, 3, 4, 5, 6, 7};
        GameObject[] nodes = new GameObject[49]; 
        for(int letter=0; letter<letters.Length; letter++)
        {
            for (int number=1; number<=7; number++)
            {
                int currentPos = number-1  + letter * 7; 
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
        introCanvas.enabled = false; 
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

        if (stepInPhase == 0)
        {
            userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the trigger button"; 
            maze.SetActive(false);
            moveForwardArrow.SetActive(false);
            footprints.SetActive(true);
            footprints.transform.position = new Vector3(arrowPath[0].x, footprints.transform.position.y, arrowPath[0].y);
            if (GetTrigger() & ControllerCollider.Instance.controllerSelection.Contains(footprints.name))
            {
                recordCameraAndNodes = true; 
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
            moveForwardArrow.transform.rotation = Quaternion.Euler(moveForwardArrow.transform.rotation.eulerAngles.x,
                arrowPath[stepInPhase].z, moveForwardArrow.transform.rotation.eulerAngles.z); 
            
            if (ControllerCollider.Instance.controllerSelection.Contains(moveForwardArrow.name))
            {
                stepInPhase++;
                ControllerCollider.Instance.controllerSelection = "Not selected";
                recordCameraAndNodes = false; 
            }
        }

        if (currentTrial < learningRounds)
        {
            if (stepInPhase >= arrowPath.Length)
            {
                currentTrial++;
                stepInPhase = 0; 
            }
        }
        else
        {
            moveForwardArrow.SetActive(false);
            currentTrial = 0; 
            phase++; 
        }

    }

    public int retraceNodes = 0; 
    /// <summary>
    /// Has participants retrace their learned route.  The arrow is invisible but still detecting collisions and moving
    /// like in RunLearning(). If the participant travels too many nodes before reaching a checkpoint, they fail and
    /// must redo the learning phase. 
    /// </summary>
    void RunRetrace()
    {
        if (currentTrial < retraceRounds)
        {
            if (stepInPhase == 0)
            {
                userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the trigger button"; 
                maze.SetActive(false);
                moveForwardArrow.SetActive(false);
                footprints.SetActive(true);
                footprints.transform.position = new Vector3(arrowPath[0].x, footprints.transform.position.y, arrowPath[0].y);
                if (GetTrigger() & ControllerCollider.Instance.controllerSelection.Contains(footprints.name))
                {
                    stepInPhase++;
                    recordCameraAndNodes = true; 
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
                }
            }
            else
            {
                currentTrial++;
                recordCameraAndNodes = false; 
                retraceNodes = 0;
                stepInPhase = 0; 
            }

            if (retraceNodes > 6)
            {
               
                stepInPhase = 0; 
                phase--; // Need to relearn
                retraceNodes = 0;
                learningRedoRounds++; 
            }
            //todo Count learning trials
        }
        else
        {
            fileHandler.AppendLine(subjectFile.Replace(".csv", "_numLearning.csv"), learningRedoRounds.ToString());
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
        foreach (var  painting in paintings.GetComponentsInChildren<MeshRenderer>())
            if(hidePaintingsDuringTesting & !painting.name.Contains("frame"))
                painting.enabled = false;  // Hides all text/pictures in the paintings
        if (currentTrial < trialList.Length)
        {
            switch (stepInPhase)
            {
                case 0: // Reorient
                    maze.SetActive(false);
                    footprints.SetActive(true);

                    userText.GetComponent<TextMeshProUGUI>().text = "Walk to the target and hit the trigger button"; 
                    if (ControllerCollider.Instance.controllerSelection.Contains(footprints.name) & GetTrigger())
                    {
                        float nextX = GetTrialInfo().start.GetX();
                        float nextY = GetTrialInfo().start.GetY();
                        footprints.transform.position = new Vector3(nextX, footprints.transform.position.y, nextY);
                        stepInPhase++;
                    }

                    break;

                case 1: // Go to next start
                    if (ControllerCollider.Instance.controllerSelection.Contains(footprints.name) & GetTrigger())
                    {
                        footprints.SetActive(false);
                        maze.SetActive(true);
                        stepInPhase++;
                        fileHandler.AppendLine(
                            (ExperimentController.Instance.subjectFile).Replace(".csv", "_nodePath.csv"),
                            PrintStepInfo() + "," + GetTrialInfo().start.GetString() + "," + GetTrialInfo().end.GetString());
                    }
                    break;

                case 2: // Walk to end
                    recordCameraAndNodes = true; 
                    recordCameraAndNodes = true; 
                    userText.GetComponent<TextMeshProUGUI>().text =
                        "Target Object: " + GetTrialInfo().end.GetTarget();
                    if (GetTrigger())
                    {
                        if (ControllerCollider.Instance.controllerSelection.Contains("targ"))
                        {
                            Debug.Log("Selected " + ControllerCollider.Instance.controllerSelection);
                            fileHandler.AppendLine(subjectFile,
                                PrintStepInfo() + "," + GetTrialInfo() + "," + ControllerCollider.Instance.controllerSelection.Remove(2));
                            maze.SetActive(false);
                            
                            stepInPhase++; 
                        }
                    }

                    break;

                case 3: // Rate stress
                    //todo Add Apurv's code
                    stressCanvas.enabled = true;
                    
                    recordCameraAndNodes = false; 
                    recordCameraAndNodes = false; 
                    // select stress, once selected disable stress UI and move phase forward
                    if (confirm)
                    {
                        //move forward
                    }
                    stepInPhase = 0;
                    footprints.transform.position = new Vector3(Random.Range(-4, 4), footprints.transform.position.y,
                        Random.Range(-4, 4));
                    currentTrial++;
                    Debug.Log("Current trial: " + currentTrial);
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
        stressCanvas.enabled = false;
    }

    /// <summary>
    /// Informs the participant (and observer) that the experiment is over through text on the GUI
    /// </summary>
    void FinishExperiment()
    {
        userText.GetComponent<TextMeshProUGUI>().text = "THE END\nThanks for participating!!";
        //TODO update for VR (have a screenSpace canvas and worldSpace canvas)
    }

        
    private float triggerTimer = 0;
    /// <summary>
    /// Checks if the trigger (or spacebar) is pressed, has a half second delay before it will read a subsequent
    /// trigger press.
    /// </summary>
    /// <returns> if(trigger & >.5 seconds since last press){return true};</returns>
    private bool GetTrigger()
    {
        if ((Input.GetAxis("Submit") > 0.25 || Input.GetKeyDown(KeyCode.Space)) &
            Time.realtimeSinceStartup - triggerTimer > .5)
        {
            triggerTimer = Time.realtimeSinceStartup; 
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

    public string PrintStepInfo()
    {
        return Time.realtimeSinceStartup.ToString() + "," + phase + "," + currentTrial + "," + stepInPhase; 
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
