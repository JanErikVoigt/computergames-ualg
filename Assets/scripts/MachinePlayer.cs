using UnityEngine;

public class MachinePlayer : MonoBehaviour
{
    [Header("Current Role")]
    public bool isCurrentlyActive = false; // Is the AI controlling this capsule?
    public bool isHunter = false; // True = Hunter, False = Runner

    // TODO: Later, this class will inherit from Unity.MLAgents.Agent 
    // and will contain CollectObservations() and OnActionReceived()

    public void ActivateAI(bool asHunter)
    {
        isCurrentlyActive = true;
        isHunter = asHunter;
        // Logic to turn on the ML-Agent behavior will go here
    }

    public void DeactivateAI()
    {
        isCurrentlyActive = false;
        // Logic to turn off the ML-Agent behavior will go here
    }
}