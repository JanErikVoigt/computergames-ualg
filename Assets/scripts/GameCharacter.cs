using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class GameCharacter : MonoBehaviour
{
    public Rigidbody Rb { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public PlayerInput InputComponent { get; private set; }

    private IPlayerController activeController;

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Agent = GetComponent<NavMeshAgent>();
        InputComponent = GetComponent<PlayerInput>();

        // Set default physics settings
        Rb.constraints = RigidbodyConstraints.FreezeRotation;
        Rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public void SetController(IPlayerController controller, float speed, bool isHunter)
    {
        if (activeController != null)
        {
            activeController.Deactivate();
        }

        activeController = controller;

        if (activeController != null)
        {
            activeController.Initialize(this, speed, isHunter);
        }
    }

    void Update()
    {
        if (activeController != null)
        {
            activeController.UpdateController();
        }
    }

    void FixedUpdate()
    {
        if (activeController != null)
        {
            activeController.FixedUpdateController();
        }
    }

    // Forward message from PlayerInput component
    public void OnMove(InputValue value)
    {
        if (activeController != null)
        {
            activeController.OnMove(value.Get<Vector2>());
        }
    }
}
