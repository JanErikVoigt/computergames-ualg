using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

[System.Serializable]
public class ArenaSetup
{
    public GameObject arenaObject;
    public Transform hunterSpawnPoint;
    public Transform runnerSpawnPoint;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Menu Panels")]
    public GameObject startScreenPanel;
    public GameObject instructionsScreenPanel; 
    public GameObject endScreenPanel;
    public GameObject hudPanel; 

    [Header("HUD Elements")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    [Header("End Screen Elements")]
    public TextMeshProUGUI victorText;
    public TextMeshProUGUI statsText; 

    [Header("Arena Progression")]
    public ArenaSetup[] arenas; // Will hold Arena1, Arena2, Arena3

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
    private int currentRound = 1;
    private int humanWins = 0;
    private int machineWins = 0;
    private bool isHumanHunter = true; 

    // Stats tracking
    private float totalTimePlayed = 0f;
    private int roundsCompleted = 0;
    private float timer = 30f; 

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        startScreenPanel.SetActive(true);
        instructionsScreenPanel.SetActive(false);
        hudPanel.SetActive(false);
        endScreenPanel.SetActive(false);
    }

    public void ShowInstructionsScreen()
    {
        startScreenPanel.SetActive(false);
        instructionsScreenPanel.SetActive(true);
    }

    public void StartGame()
    {
        startScreenPanel.SetActive(false);
        instructionsScreenPanel.SetActive(false); 
        hudPanel.SetActive(true); 
        
        currentRound = 1;
        humanWins = 0;
        machineWins = 0;
        totalTimePlayed = 0f;
        roundsCompleted = 0;
        isHumanHunter = true; 
        
        UpdateScoreBoard();
        StartRound();
    }

    private void StartRound()
    {
        isGameActive = true;
        timer = 30f; 
        UpdateTimerText();
        roundText.text = "Round " + currentRound + " / 5";

        ConfigureActiveArena();
        ResetCharactersToCurrentArena();

        if (isHumanHunter)
        {
            cameraController.target = hunterTransform;
            humanHunterScript.isCurrentlyActive = true;
            humanHunterScript.isHunter = true;
            machineHunterScript.DeactivateAI(); 

            humanRunnerScript.isCurrentlyActive = false; 
            machineRunnerScript.ActivateAI(false);       
        }
        else
        {
            cameraController.target = runnerTransform;
            humanRunnerScript.isCurrentlyActive = true;
            humanRunnerScript.isHunter = false;
            machineRunnerScript.DeactivateAI(); 

            humanHunterScript.isCurrentlyActive = false; 
            machineHunterScript.ActivateAI(true);        
        }
    }

    private void ConfigureActiveArena()
    {
        // Determine which arena should be active
        int activeIndex = 0;
        if (currentRound == 3 || currentRound == 4) activeIndex = 1;
        if (currentRound == 5) activeIndex = 2;

        // Toggle the correct arena on and the others off
        for (int i = 0; i < arenas.Length; i++)
        {
            if (arenas[i] != null && arenas[i].arenaObject != null)
            {
                arenas[i].arenaObject.SetActive(i == activeIndex);
            }
        }
    }

    public void EndRound(bool didHumanWinRound)
    {
        isGameActive = false;
        
        // Record stats
        float timeSpentThisRound = 30f - timer;
        totalTimePlayed += timeSpentThisRound;
        roundsCompleted++;

        if (didHumanWinRound) humanWins++;
        else machineWins++;

        UpdateScoreBoard();

        if (humanWins >= 3 || machineWins >= 3 || currentRound >= 5)
        {
            EndGame();
        }
        else
        {
            currentRound++;
            isHumanHunter = !isHumanHunter; 
            StartRound();
        }
    }

    private void EndGame()
    {
        hudPanel.SetActive(false);
        endScreenPanel.SetActive(true);

        if (victorText != null)
        {
            if (humanWins > machineWins) victorText.text = "Human Wins the Game!";
            else if (machineWins > humanWins) victorText.text = "Machine Wins the Game!";
            else victorText.text = "It's a Tie!";
        }

        if (statsText != null)
        {
            float avgTime = roundsCompleted > 0 ? totalTimePlayed / roundsCompleted : 0f;
            statsText.text = $"Rounds Played: {roundsCompleted}\nAverage Round Time: {avgTime:F1}s";
        }
    }

    private void ResetCharactersToCurrentArena()
    {
        int activeIndex = 0;
        if (currentRound == 3 || currentRound == 4) activeIndex = 1;
        if (currentRound == 5) activeIndex = 2;

        ArenaSetup currentSetup = arenas[activeIndex];

        // Reset Hunter capsule
        if (hunterTransform != null && currentSetup.hunterSpawnPoint != null)
        {
            hunterTransform.position = currentSetup.hunterSpawnPoint.position;
            hunterTransform.rotation = currentSetup.hunterSpawnPoint.rotation;
            if (hunterTransform.TryGetComponent<Rigidbody>(out var hunterRb))
            {
                hunterRb.linearVelocity = Vector3.zero;
                hunterRb.angularVelocity = Vector3.zero;
            }
        }

        // Reset Runner capsule
        if (runnerTransform != null && currentSetup.runnerSpawnPoint != null)
        {
            runnerTransform.position = currentSetup.runnerSpawnPoint.position;
            runnerTransform.rotation = currentSetup.runnerSpawnPoint.rotation;
            if (runnerTransform.TryGetComponent<Rigidbody>(out var runnerRb))
            {
                runnerRb.linearVelocity = Vector3.zero;
                runnerRb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void OnHunterCaughtRunner()
    {
        if (!isGameActive) return;
        EndRound(isHumanHunter);
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
                EndRound(!isHumanHunter);
            }
            else
            {
                UpdateTimerText();
            }
        }
    }

    private void UpdateScoreBoard()
    {
        if (scoreText != null)
        {
            scoreText.text = "Human: " + humanWins + " | Machine: " + machineWins;
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + timer.ToString("F1") + "s";
        }
    }
}