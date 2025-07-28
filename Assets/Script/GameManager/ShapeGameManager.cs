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

    private List<ShapeTracker> activeShapes = new List<ShapeTracker>();
    private int score = 0;
    private Coroutine timerRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("🟢 GameManager started");
        spawnManager.Init(); // Make sure this method exists
        SpawnShapeBatch(); // Start the first batch
    }

    void Update()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void SpawnShapeBatch()
    {
        Debug.Log("🚀 Spawning new batch");

        // Disable previous shapes
        foreach (var tracker in activeShapes)
        {
            if (tracker != null && tracker.validator != null)
            {
                Transform spawnPoint = tracker.validator.assignedSpawnPoint;
                spawnManager.FreeSpawnPoint(spawnPoint); // ✅ We'll add this method
                Destroy(tracker.gameObject); // ✅ Fully remove shape from scene
            }
        }
        activeShapes.Clear();


        for (int i = 0; i < 3; i++)
        {
            GameObject shape = spawnManager.SpawnNextShape();
            if (shape == null)
            {
                Debug.LogWarning("❌ Shape spawn returned null");
                continue;
            }

            ShapeTracker tracker = shape.AddComponent<ShapeTracker>();
            tracker.validator = shape.GetComponent<ShapeValidator>();
            tracker.spawnTime = Time.time;
            activeShapes.Add(tracker);
            Debug.Log("✅ Spawned: " + shape.name);
        }

        if (activeShapes.Count == 0)
        {
            Debug.Log("🏁 No more shapes left. Game Over.");
            if (timerText) timerText.text = "00:00";
            return;
        }

        if (timerRoutine != null)
            StopCoroutine(timerRoutine);

        timerRoutine = StartCoroutine(TimerAndScoreRoutine());
    }

    IEnumerator TimerAndScoreRoutine()
    {
        Debug.Log("⏳ Timer started for batch");
        float endTime = Time.time + timePerShape;

        while (Time.time < endTime)
        {
            float remaining = endTime - Time.time;
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);

            if (timerText != null)
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            for (int i = activeShapes.Count - 1; i >= 0; i--)
            {
                var tracker = activeShapes[i];
                if (tracker != null && tracker.validator != null && tracker.validator.isSolved && !tracker.hasScored)
                {
                    // Update score
                    score += 10;
                    tracker.hasScored = true;

                    // Free old spawn point
                    spawnManager.FreeSpawnPoint(tracker.validator.assignedSpawnPoint);

                    // Store the spawn point before destroying
                    Transform oldPoint = tracker.validator.assignedSpawnPoint;

                    // Remove and destroy old shape
                    activeShapes.RemoveAt(i);
                    Destroy(tracker.gameObject);

                    // Spawn new shape
                    GameObject newShape = spawnManager.SpawnNextShape();
                    if (newShape != null)
                    {
                        ShapeTracker newTracker = newShape.AddComponent<ShapeTracker>();
                        newTracker.validator = newShape.GetComponent<ShapeValidator>();
                        newTracker.spawnTime = Time.time;
                        activeShapes.Add(newTracker);
                    }
                }
            }

            // ✅ All shapes solved before timer ends
            if (activeShapes.Count > 0 && AllShapesSolved())
            {
                Debug.Log("🎉 All shapes solved early! Spawning next batch...");
                yield return new WaitForSeconds(0.5f); // Small delay for feedback
                SpawnShapeBatch();
                yield break; // Stop current coroutine
            }

            yield return null;
        }

        Debug.Log("⏱️ Time's up for batch, spawning next batch");
        SpawnShapeBatch();
    }

    private bool AllShapesSolved()
    {
        foreach (var tracker in activeShapes)
        {
            if (tracker != null && tracker.validator != null && !tracker.validator.isSolved)
                return false;
        }
        return true;
    }
}

public class ShapeTracker : MonoBehaviour
{
    public ShapeValidator validator;
    public float spawnTime;
    public bool hasScored = false;
}
