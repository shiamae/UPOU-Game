using System.Collections.Generic;
using UnityEngine;

public class WasteSpawner : MonoBehaviour
{
    [Header("Waste Types")]
    public WasteSpawnData[] wasteItems;

    [Header("Spawn Area")]
    public Vector3 spawnArea = new Vector3(20f, 0f, 20f);

    [Header("Ground Detection")]
    public float raycastHeight = 50f;
    public LayerMask groundLayer;

    [Header("Spacing")]
    public float minimumDistance = 1f;
    public int maxSpawnAttempts = 30;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnWaste();
    }

    void SpawnWaste()
    {
        spawnedPositions.Clear();

        foreach (WasteSpawnData waste in wasteItems)
        {
            if (waste.wastePrefab == null)
                continue;

            for (int i = 0; i < waste.quantity; i++)
            {
                bool spawned = false;

                for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
                {
                    // Random XZ position
                    float randomX = Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
                    float randomZ = Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f);

                    Vector3 rayOrigin = transform.position +
                                        new Vector3(randomX, raycastHeight, randomZ);

                    // Shoot ray downward
                    if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, groundLayer))
                    {
                        Vector3 spawnPosition = hit.point;

                        if (!IsPositionValid(spawnPosition))
                            continue;

                        // Small offset so objects don't clip into the ground
                        spawnPosition += Vector3.up * 0.05f;

                        Quaternion rotation =
                            waste.wastePrefab.transform.rotation *
                            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                        Instantiate(
                            waste.wastePrefab,
                            spawnPosition,
                            rotation
                        );

                        spawnedPositions.Add(spawnPosition);
                        spawned = true;
                        break;
                    }
                }

                if (!spawned)
                {
                    Debug.LogWarning($"Couldn't spawn {waste.wastePrefab.name}");
                }
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minimumDistance)
                return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(spawnArea.x, 0.1f, spawnArea.z)
        );
    }
}