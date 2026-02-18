using UnityEngine;

public class BallSpawner : MonoBehaviour
{

    public GameObject whatToSpawn;
    public float delay;
    public int initial_balls=5;

    public Vector3 spawnArea = new Vector3(10,0,10);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this.whatToSpawn == null) {return;}

        for (int i = 0; i < this.initial_balls; i++)
        {
            this.spawnBall();
        }

        InvokeRepeating("spawnBall", this.delay, this.delay);
    }

    void spawnBall()
    {
        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnArea.x/2f,spawnArea.x/2f), 
            Random.Range(-spawnArea.y/2f,spawnArea.y/2f), 
            Random.Range(-spawnArea.z/2f,spawnArea.z/2f));
        
        Vector3 spawnPos = transform.position + spawnOffset;// - this.spawnArea / 2f;
        Instantiate(this.whatToSpawn, spawnPos, Quaternion.identity);
    }
}
