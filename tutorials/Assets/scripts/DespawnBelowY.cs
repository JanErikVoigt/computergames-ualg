using UnityEngine;

public class DespawnBelowY : MonoBehaviour
{
    public float threshold = 8f;

    void Update()
    {
        if (transform.position.y < threshold)
        {
            Destroy(gameObject);
        }
    }
}
