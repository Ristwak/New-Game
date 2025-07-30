using UnityEngine;
using System.Collections.Generic;

public class MaterialSpawnManager : MonoBehaviour
{
    [Header("Shape Prefabs")]
    public List<GameObject> shapePrefabs;

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints; // Assign in Inspector

    public List<GameObject> remainingShapes = new List<GameObject>();
    private Dictionary<Transform, bool> spawnPointStatus = new Dictionary<Transform, bool>();

    public void Init()
    {
        // Reset remaining shapes
        remainingShapes = new List<GameObject>(shapePrefabs);

        // Mark all spawn points as free
        spawnPointStatus.Clear();
        foreach (var point in spawnPoints)
        {
            spawnPointStatus[point] = false;
        }
    }

    public GameObject SpawnNextmaterial()
    {
        Transform freeSpot = GetFreeSpawnPoint();
        if (freeSpot == null || remainingShapes.Count == 0)
        {
            Debug.LogWarning("❌ No free spot or no shapes left to spawn");
            return null;
        }

        // Pick and remove a random shape from remaining pool
        int index = Random.Range(0, remainingShapes.Count);
        GameObject prefab = remainingShapes[index];
        remainingShapes.RemoveAt(index);

        GameObject shape = Instantiate(prefab, freeSpot.position, prefab.transform.rotation);
        shape.name = prefab.name;

        ShapeValidator validator = shape.GetComponent<ShapeValidator>();
        if (validator != null)
            validator.assignedSpawnPoint = freeSpot;

        spawnPointStatus[freeSpot] = true;

        Debug.Log("🟩 Spawned: " + shape.name + " at " + freeSpot.name);
        return shape;
    }

    private Transform GetFreeSpawnPoint()
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

    public void FreeSpawnPoint(Transform point)
    {
        if (spawnPointStatus.ContainsKey(point))
            spawnPointStatus[point] = false;
    }

    public bool IsEmpty()
    {
        return remainingShapes.Count == 0;
    }
}
