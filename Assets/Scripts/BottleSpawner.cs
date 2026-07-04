using UnityEngine;

public class BottleSpawner : MonoBehaviour
{
    [Header("Bottle Prefab")]
    public GameObject bottlePrefab;

    [Header("Spawn Settings")]
    public int numberOfBottles = 50;

    public Vector3 spawnArea = new Vector3(20f, 0f, 20f);

    public float spawnHeight = 1f;

    void Start()
    {
        SpawnBottles();
    }

    void SpawnBottles()
    {
        for (int i = 0; i < numberOfBottles; i++)
        {
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
                spawnHeight,
                Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f)
            );

            Instantiate(
                bottlePrefab,
                randomPosition,
                Quaternion.Euler(90f, Random.Range(0f, 360f), 0f)
            );
        }
    }
}