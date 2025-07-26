using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawnManager : MonoBehaviour
{
    [Header("Shape Prefabs")]
    public List<GameObject> shapePrefabs;
    // public float spawnObjectTime = 60f;

    [Header("Spawn Settings")]
    public int initialSpawnCount = 3;
    public List<Transform> spawnPoints; // Set these in Inspector manually

    private List<GameObject> spawnedShapes = new List<GameObject>();
    private List<GameObject> remainingShapes = new List<GameObject>();
    private Dictionary<Transform, bool> spawnPointStatus = new Dictionary<Transform, bool>();
    private int solvedCount = 0;

    void Start()
    {
        // Initialize available shapes and spawn point availability
        remainingShapes = new List<GameObject>(shapePrefabs);

        foreach (var point in spawnPoints)
        {
            spawnPointStatus[point] = false; // All spawn points start as free
        }

        SpawnShapes(initialSpawnCount);
    }

    public void Init()
    {
        remainingShapes = new List<GameObject>(shapePrefabs);

        spawnPointStatus.Clear();
        foreach (var point in spawnPoints)
        {
            spawnPointStatus[point] = false;
        }

        spawnedShapes.Clear();
        solvedCount = 0;
    }

    void Update()
    {
        for (int i = spawnedShapes.Count - 1; i >= 0; i--)
        {
            ShapeValidator validator = spawnedShapes[i].GetComponent<ShapeValidator>();
            if (validator != null && validator.isSolved)
            {
                solvedCount++;

                // Mark spawn point as free again
                Transform spawnPoint = validator.assignedSpawnPoint;
                if (spawnPoint != null)
                    spawnPointStatus[spawnPoint] = false;

                spawnedShapes.RemoveAt(i);

                // Spawn 1 new shape only
                SpawnShapes(1);

                if (remainingShapes.Count == 0 && spawnedShapes.Count == 0)
                {
                    Debug.Log("✅ Game Completed!");
                    // TODO: Trigger win UI or scene transition
                }
            }
        }
    }

    void SpawnShapes(int count)
    {
        for (int i = 0; i < count && remainingShapes.Count > 0; i++)
        {
            Transform freeSpot = GetFreeSpawnPoint();
            if (freeSpot == null)
            {
                Debug.LogWarning("❗ No available spawn points.");
                return;
            }

            int index = Random.Range(0, remainingShapes.Count);
            GameObject prefab = remainingShapes[index];
            GameObject shape = Instantiate(prefab, freeSpot.position, prefab.transform.rotation);
            shape.name = prefab.name;

            // Attach reference to assigned spawn point (optional)
            ShapeValidator validator = shape.GetComponent<ShapeValidator>();
            if (validator != null)
                validator.assignedSpawnPoint = freeSpot;

            spawnedShapes.Add(shape);
            remainingShapes.RemoveAt(index);
            spawnPointStatus[freeSpot] = true;
        }
    }

    public bool IsEmpty()
    {
        return remainingShapes.Count == 0 && spawnedShapes.Count == 0;
    }

    public GameObject SpawnNextShape()
    {
        // Pick one from the list and spawn if a spawn point is free
        Transform freeSpot = GetFreeSpawnPoint();
        if (freeSpot == null || remainingShapes.Count == 0)
            return null;

        int index = Random.Range(0, remainingShapes.Count);
        GameObject prefab = remainingShapes[index];
        GameObject shape = Instantiate(prefab, freeSpot.position, prefab.transform.rotation);
        shape.name = prefab.name;

        // ShapeGameManager.Instance.Countdown(spawnObjectTime);

        ShapeValidator validator = shape.GetComponent<ShapeValidator>();
        if (validator != null)
            validator.assignedSpawnPoint = freeSpot;

        spawnedShapes.Add(shape);
        remainingShapes.RemoveAt(index);
        spawnPointStatus[freeSpot] = true;

        return shape;
    }

    Transform GetFreeSpawnPoint()
    {
        List<Transform> freePoints = new List<Transform>();
        foreach (var kvp in spawnPointStatus)
        {
            if (!kvp.Value)
                freePoints.Add(kvp.Key);
        }

        if (freePoints.Count == 0) return null;
        return freePoints[Random.Range(0, freePoints.Count)];
    }
}
