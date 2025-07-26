using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShapeGameManager : MonoBehaviour
{
    public static ShapeGameManager Instance;

    [Header("Managers")]
    public ShapeSpawnManager spawnManager;

    [Header("Timing Settings")]
    public float timePerShape = 60f;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;

    public Queue<ShapeTracker> activeShapes = new Queue<ShapeTracker>();
    private int score = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        spawnManager.Init();

        // Spawn initial 3 shapes
        for (int i = 0; i < 3; i++)
        {
            GameObject shape = spawnManager.SpawnNextShape();
            if (shape == null) break;

            ShapeTracker tracker = shape.AddComponent<ShapeTracker>();
            tracker.validator = shape.GetComponent<ShapeValidator>();
            tracker.spawnTime = Time.time;
            activeShapes.Enqueue(tracker);
        }

        StartCoroutine(ProcessQueue());
    }

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    IEnumerator ProcessQueue()
    {
        while (true)
        {
            // Wait until there is at least one active shape
            while (activeShapes.Count == 0)
                yield return null;

            ShapeTracker current = activeShapes.Peek();
            float endTime = Time.time + timePerShape;

            bool shapeSolvedOrExpired = false;

            while (!shapeSolvedOrExpired)
            {
                float remaining = endTime - Time.time;
                remaining = Mathf.Clamp(remaining, 0f, timePerShape);

                int minutes = Mathf.FloorToInt(remaining / 60f);
                int seconds = Mathf.FloorToInt(remaining % 60f);

                if (timerText != null)
                    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                if (current.validator != null && current.validator.isSolved)
                {
                    score += 10;
                    activeShapes.Dequeue();
                    yield return SpawnNewShape();
                    shapeSolvedOrExpired = true;
                    break;
                }

                if (Time.time >= endTime)
                {
                    Debug.Log("⏱️ Time up: " + current.name);
                    Destroy(current.gameObject);
                    activeShapes.Dequeue();
                    yield return SpawnNewShape();
                    shapeSolvedOrExpired = true;
                    break;
                }

                yield return null;
            }

            // Exit condition: game ends when there are no more shapes to track or spawn
            if (activeShapes.Count == 0 && spawnManager.IsEmpty())
            {
                timerText.text = "00:00";
                Debug.Log("✅ All shapes completed!");
                break;
            }
        }
    }

    IEnumerator SpawnNewShape()
    {
        GameObject newShape = spawnManager.SpawnNextShape();
        if (newShape != null)
        {
            ShapeTracker newTracker = newShape.AddComponent<ShapeTracker>();
            newTracker.validator = newShape.GetComponent<ShapeValidator>();
            newTracker.spawnTime = Time.time;
            activeShapes.Enqueue(newTracker);
        }

        yield return null;
    }
}

public class ShapeTracker : MonoBehaviour
{
    public ShapeValidator validator;
    public float spawnTime;
}
