using UnityEngine;

public class HunterCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Get the GameManager instance
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) return;

        // Check if we collided with the Runner capsule
        if (collision.gameObject.CompareTag("Runner") || 
            (gameManager.runnerTransform != null && collision.transform == gameManager.runnerTransform))
        {
            gameManager.OnHunterCaughtRunner();
        }
    }
}
