using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Menu Panels")]
    public GameObject startScreenPanel; 
    public GameObject endScreenPanel;
    public GameObject hudPanel; 

    [Header("HUD Elements")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    [Header("End Screen Elements")]
    public TextMeshProUGUI victorText;

    [Header("Player Scripts")]
    public HumanPlayer humanHunterScript;
    public MachinePlayer machineHunterScript;
    
    public HumanPlayer humanRunnerScript;
    public MachinePlayer machineRunnerScript;

    [Header("Characters & Camera")]
    public CameraFollow cameraController; 
    public Transform hunterTransform;     
    public Transform runnerTransform;     

    private bool isGameActive = false; 
    
    // Tracking variables
    private int currentRound = 1;
    private int humanWins = 0;
    private int machineWins = 0;
    
    // NEW: Tracks whose turn it is to be the Hunter
    private bool isHumanHunter = true; 

    // Character starting states for resetting
    private Vector3 hunterStartPos;
    private Quaternion hunterStartRot;
    private Vector3 runnerStartPos;
    private Quaternion runnerStartRot;

    // Timer state
    private float timer = 30f; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Record initial states...
        if (hunterTransform != null) { hunterStartPos = hunterTransform.position; hunterStartRot = hunterTransform.rotation; }
        if (runnerTransform != null) { runnerStartPos = runnerTransform.position; runnerStartRot = runnerTransform.rotation; }

        if (isTrainingMode)
        {
            // Hide menus completely and jump straight into the loop
            if (startScreenPanel != null) startScreenPanel.SetActive(false);
            if (hudPanel != null) hudPanel.SetActive(true);
            if (endScreenPanel != null) endScreenPanel.SetActive(false);
            
            StartRound();
        }
        else
        {
            // Normal human play mode setup
            startScreenPanel.SetActive(true);
            hudPanel.SetActive(false);
            endScreenPanel.SetActive(false);
        }
    }

    public void StartGame()
    {
        startScreenPanel.SetActive(false); 
        hudPanel.SetActive(true); 
        
        // Reset everything for a fresh game
        currentRound = 1;
        humanWins = 0;
        machineWins = 0;
        isHumanHunter = true; // Human starts as Hunter in Round 1
        
        ResetCharacters();
        UpdateScoreBoard();
        StartRound();
    }

    [Header("Game Modes")]
    public bool isTrainingMode = false;
    private void StartRound()
    {
        isGameActive = true;
        timer = 30f; // Reset timer to 30s for the new round
        UpdateTimerText();
        roundText.text = "Round " + currentRound + " / 5";
        if (isTrainingMode)
        {
            // Activate both machines, deactivate both humans
            humanHunterScript.isCurrentlyActive = false;
            humanRunnerScript.isCurrentlyActive = false;

            machineHunterScript.ActivateAI(true);
            machineRunnerScript.ActivateAI(false);

            // Optional: Let the camera watch the action from a fixed point
            // or just follow the Hunter during training
            cameraController.target = hunterTransform;
        }
        else
        {
            if (isHumanHunter)
            {
                // Human is the Hunter
                cameraController.target = hunterTransform;
                
                humanHunterScript.isCurrentlyActive = true;
                humanHunterScript.isHunter = true;
                machineHunterScript.DeactivateAI(); // Turn AI off on Hunter

                // Machine is the Runner
                humanRunnerScript.isCurrentlyActive = false; // Turn human off on Runner
                machineRunnerScript.ActivateAI(false);       // Turn AI on as Runner
            }
            else
            {
                // Human is the Runner
                cameraController.target = runnerTransform;
                
                humanRunnerScript.isCurrentlyActive = true;
                humanRunnerScript.isHunter = false;
                machineRunnerScript.DeactivateAI(); // Turn AI off on Runner

                // Machine is the Hunter
                humanHunterScript.isCurrentlyActive = false; // Turn human off on Hunter
                machineHunterScript.ActivateAI(true);        // Turn AI on as Hunter
            }
        }
    }
    // NEW: Call this function when the timer hits 0 or the Hunter catches the Runner
    public void EndRound(bool didHumanWinRound)
    {
        isGameActive = false;
        
        if (didHumanWinRound)
        {
            humanWins++;
            // The machine failed its objective
            machineHunterScript.AddReward(-1f);
            machineRunnerScript.AddReward(-1f);
        }
        else
        {
            machineWins++;
            machineHunterScript.AddReward(1f);
            machineRunnerScript.AddReward(1f);
        }

        machineHunterScript.EndEpisode();
        machineRunnerScript.EndEpisode();

        UpdateScoreBoard();

        // Check if the whole game is over
        if (humanWins >= 3 || machineWins >= 3 || currentRound >= 5)
        {
            if (isTrainingMode)
            {
                // Instead of stopping at the End Game screen, reset values and keep looping forever!
                currentRound = 1;
                humanWins = 0;
                machineWins = 0;
                isHumanHunter = true;
                ResetCharacters();
                StartRound();
            }
            else
            {
                EndGame();
            }
        }
        else
        {
            // If the game isn't over, set up the next round
            currentRound++;
            
            // This flips the roles: If it was true, it becomes false. If false, it becomes true.
            isHumanHunter = !isHumanHunter; 
            
            // Reset capsule positions back to their starting corners
            ResetCharacters();
            
            StartRound();
        }
    }

    private void EndGame()
    {
        hudPanel.SetActive(false);
        endScreenPanel.SetActive(true);

        if (victorText != null)
        {
            if (humanWins > machineWins)
            {
                victorText.text = "Human Wins the Game!";
            }
            else if (machineWins > humanWins)
            {
                victorText.text = "Machine Wins the Game!";
            }
            else
            {
                victorText.text = "It's a Tie!";
            }
        }
    }

    public void RestartGame()
    {
        // Hide the end screen and go back to the start menu
        endScreenPanel.SetActive(false);
        startScreenPanel.SetActive(true);
    }

    private void UpdateScoreBoard()
    {
        scoreText.text = "Human: " + humanWins + " | Machine: " + machineWins;
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + timer.ToString("F1") + "s";
        }
    }

    // Call this from HunterCollision when Hunter catches Runner
    public void OnHunterCaughtRunner()
    {
        if (!isGameActive) return;

        // Hunter caught Runner, so Hunter wins round.
        // If human is Hunter, human wins round. Otherwise machine wins.
        EndRound(isHumanHunter);
    }

    private void ResetCharacters()
    {
        // Reset Hunter capsule
        if (hunterTransform != null)
        {
            hunterTransform.position = hunterStartPos;
            hunterTransform.rotation = hunterStartRot;
            if (hunterTransform.TryGetComponent<Rigidbody>(out var hunterRb))
            {
                hunterRb.linearVelocity = Vector3.zero;
                hunterRb.angularVelocity = Vector3.zero;
            }
        }

        // Reset Runner capsule
        if (runnerTransform != null)
        {
            runnerTransform.position = runnerStartPos;
            runnerTransform.rotation = runnerStartRot;
            if (runnerTransform.TryGetComponent<Rigidbody>(out var runnerRb))
            {
                runnerRb.linearVelocity = Vector3.zero;
                runnerRb.angularVelocity = Vector3.zero;
            }
        }
    }

    void Update()
    {
        if (isGameActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 0f;
                UpdateTimerText();
                // Runner wins round (Time out).
                // If human is Runner (not isHumanHunter), human wins. Otherwise machine wins.
                EndRound(!isHumanHunter);
            }
            else
            {
                UpdateTimerText();
            }
        }
    }
}