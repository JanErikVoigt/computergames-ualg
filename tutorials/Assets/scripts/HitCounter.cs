using TMPro;
using UnityEngine;

public class HitCounter : MonoBehaviour
{
    public TextMeshProUGUI display;
    public string excludeTag = "Ground";

    private int hits = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(excludeTag))
        {
            hits++;
            if (display != null)
                display.text = "Hits: " + hits;
        }
    }
}
