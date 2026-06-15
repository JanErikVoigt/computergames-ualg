using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject startScreenPanel; 
    public GameObject endScreenPanel;
    public GameObject hudPanel; 

    [Header("HUD Elements")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

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

    void Start()
    {
        startScreenPanel.SetActive(true);
        hudPanel.SetActive(false);
        endScreenPanel.SetActive(false);
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
        
        UpdateScoreBoard();
        StartRound();
    }

    private void StartRound()
    {
        isGameActive = true;
        roundText.text = "Round " + currentRound + " / 5";

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
    // NEW: Call this function when the timer hits 0 or the Hunter catches the Runner
    public void EndRound(bool didHumanWinRound)
    {
        isGameActive = false;
        
        if (didHumanWinRound)
        {
            humanWins++;
        }
        else
        {
            machineWins++;
        }

        UpdateScoreBoard();

        // Check if the whole game is over
        if (humanWins >= 3 || machineWins >= 3 || currentRound >= 5)
        {
            EndGame();
        }
        else
        {
            // If the game isn't over, set up the next round
            currentRound++;
            
            // This flips the roles: If it was true, it becomes false. If false, it becomes true.
            isHumanHunter = !isHumanHunter; 
            
            // TODO: Reset capsule positions back to their starting corners here
            
            StartRound();
        }
    }

    private void EndGame()
    {
        hudPanel.SetActive(false);
        endScreenPanel.SetActive(true);
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

    void Update()
    {
        if (isGameActive)
        {
            // Timer countdown logic goes here...
        }
    }
}