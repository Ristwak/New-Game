using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawnManager : MonoBehaviour
{
    [Header("Shape Prefabs")]
    public List<GameObject> shapePrefabs;

    [Header("Spawn Settings")]
    public int initialSpawnCount = 3;
    public float minSpawnDistance = 1.5f;
    public float maxSpawnDistance = 3f;
    public Transform playerTransform;
    

    private List<GameObject> spawnedShapes = new List<GameObject>();
    private List<GameObject> remainingShapes = new List<GameObject>();
    private int solvedCount = 0;

    void Start()
    {
        remainingShapes = new List<GameObject>(shapePrefabs);
        SpawnShapes(initialSpawnCount);
    }

    void Update()
    {
        // Check each currently spawned shape to see if it's solved
        for (int i = spawnedShapes.Count - 1; i >= 0; i--)
        {
            ShapeValidator validator = spawnedShapes[i].GetComponent<ShapeValidator>();
            if (validator != null && validator.isSolved)
            {
                solvedCount++;

                // Destroy(spawnedShapes[i]); // Clean up solved shape
                spawnedShapes.RemoveAt(i);

                // Spawn 2 new shapes immediately
                SpawnShapes(2);

                // Check for game completion
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
            int index = Random.Range(0, remainingShapes.Count);
            GameObject prefab = remainingShapes[index];

            Vector3 spawnPos = RandomPositionInRoom();
            GameObject shape = Instantiate(prefab, spawnPos, Quaternion.identity);
            shape.name = prefab.name;
            spawnedShapes.Add(shape);

            remainingShapes.RemoveAt(index);
        }
    }

    Vector3 RandomPositionInRoom()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 offset = new Vector3(randomCircle.x, 0f, randomCircle.y);
        Vector3 position = playerTransform.position + offset;
        position.y = 0.5f; // Adjust height based on shape prefab
        return position;
    }
}
