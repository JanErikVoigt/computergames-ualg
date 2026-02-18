using UnityEngine;

public class HitSound : MonoBehaviour
{
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }
    
    // OnCollisionEnter is called when the this game object
    // collides with other game object
    void OnCollisionEnter(Collision collision)
    {
        source.Play();
    }
}