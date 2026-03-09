using UnityEngine;

public class DespawnBelowY : MonoBehaviour
{
    public float threshold = -10f;

    void Update()
    {
        if (transform.position.y < threshold)
        {
            enabled = false;
            Destroy(gameObject);
        }
    }
}
