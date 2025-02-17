using UnityEngine;

public class GoblinWave : MonoBehaviour
{
    public GameObject goblinPrefab;
    public Transform[] spawnPoints; 
    public int goblinsPerWave = 5; 
    public float timeBetweenWaves = 10.0f; 

    private int currentWave = 0;
    private float waveTimer;

    void Start()
    {
        waveTimer = timeBetweenWaves;
    }

    void Update()
    {
        waveTimer -= Time.deltaTime;

        if (waveTimer <= 0)
        {
            SpawnWave();
            waveTimer = timeBetweenWaves;
        }
    }

    void SpawnWave()
    {
        currentWave++;
        Debug.Log("Spawning Wave " + currentWave);

        for (int i = 0; i < goblinsPerWave; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(goblinPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}