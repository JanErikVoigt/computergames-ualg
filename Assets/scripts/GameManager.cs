using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

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

    [Header("Characters & Camera")]
    public CameraFollow cameraController;
    public Transform hunterTransform;
    public Transform runnerTransform;

    [Header("Game Characters")]
    public GameCharacter hunterCharacter;
    public GameCharacter runnerCharacter;

    [Header("Speed Settings")]
    public float hunterSpeed = 12f;
    public float runnerSpeed = 9f;
    public float catchDistance = 1.6f;

    private bool isGameActive = false;

    // Tracking variables
    private int currentRound = 1;
    private int humanWins = 0;
    private int machineWins = 0;

    // Tracks whose turn it is to be the Hunter
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
        // Auto-assign the GameCharacter references from the GameObjects
        if (hunterTransform != null)
        {
            hunterCharacter = hunterTransform.GetComponentInChildren<GameCharacter>();
            hunterStartPos = hunterTransform.position;
            hunterStartRot = hunterTransform.rotation;
        }
        if (runnerTransform != null)
        {
            runnerCharacter = runnerTransform.GetComponentInChildren<GameCharacter>();
            runnerStartPos = runnerTransform.position;
            runnerStartRot = runnerTransform.rotation;
        }

        Debug.Log($"[GameManager] Auto-configured components. Hunter: {hunterTransform?.name}, Runner: {runnerTransform?.name}");

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

        ResetCharacters();
        UpdateScoreBoard();
        StartRound();
    }

    private void StartRound()
    {
        isGameActive = true;
        timer = 30f; // Reset timer to 30s for the new round
        UpdateTimerText();
        roundText.text = "Round " + currentRound + " / 5";

        if (isHumanHunter)
        {
            // Round 1/3/5: Human is Hunter (on hunterCharacter), Machine is Runner (on runnerCharacter)
            if (hunterCharacter != null)
            {
                hunterCharacter.SetController(new HumanPlayerController(), hunterSpeed, true);
            }
            if (runnerCharacter != null)
            {
                runnerCharacter.SetController(new MachinePlayerController(), runnerSpeed, false);
            }
        }
        else
        {
            // Round 2/4: Human is Runner (on runnerCharacter), Machine is Hunter (on hunterCharacter)
            if (runnerCharacter != null)
            {
                runnerCharacter.SetController(new HumanPlayerController(), runnerSpeed, false);
            }
            if (hunterCharacter != null)
            {
                hunterCharacter.SetController(new MachinePlayerController(), hunterSpeed, true);
            }
        }

        // Update Camera target dynamically
        if (cameraController != null)
        {
            cameraController.target = isHumanHunter ? hunterTransform : runnerTransform;
        }

        Debug.Log($"[GameManager] StartRound {currentRound}. isHumanHunter: {isHumanHunter}. Hunter: {hunterTransform?.name}, Runner: {runnerTransform?.name}");
    }

    private int lastTransitionFrame = -1;

    // Call this function when the timer hits 0 or the Hunter catches the Runner
    public void EndRound(bool didHumanWinRound)
    {
        // Guard against same-frame double-triggers (e.g., proximity check + physics collision)
        if (Time.frameCount == lastTransitionFrame) return;
        lastTransitionFrame = Time.frameCount;

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
        // 1. Deactivate active controllers to clean up components (disable NavMeshAgents, reset kinematics)
        if (hunterCharacter != null)
        {
            hunterCharacter.SetController(null, 0f, false);
        }
        if (runnerCharacter != null)
        {
            runnerCharacter.SetController(null, 0f, false);
        }

        // 2. Reset Hunter capsule position and rotation
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

        // 3. Reset Runner capsule position and rotation
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

            // Proximity-based catch detection as a robust fallback/fail-safe
            if (hunterTransform != null && runnerTransform != null)
            {
                float dist = Vector3.Distance(hunterTransform.position, runnerTransform.position);

                if (dist <= catchDistance)
                {
                    Debug.Log($"[GameManager] Proximity catch triggered! Distance: {dist:F2} <= {catchDistance:F2}");
                    OnHunterCaughtRunner();
                    return;
                }
            }

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