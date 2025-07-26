using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeGameManager : MonoBehaviour
{
    [Header("Managers")]
    public ShapeSpawnManager spawnManager;

    [Header("Timing Settings")]
    public float timePerShape = 60f;

    [Header("UI")]
    public Text timerText;
    public Text scoreText;

    private Queue<ShapeTracker> activeShapes = new Queue<ShapeTracker>();
    private int score = 0;

    void Start()
    {
        StartCoroutine(SpawnAndTrack());
    }

    void Update()
    {
        // Update score UI
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    IEnumerator SpawnAndTrack()
    {
        while (spawnManager != null)
        {
            // Wait for shape to be spawned
            GameObject newShape = spawnManager.SpawnNextShape();
            if (newShape == null) yield break;

            ShapeTracker tracker = newShape.AddComponent<ShapeTracker>();
            tracker.validator = newShape.GetComponent<ShapeValidator>();
            tracker.spawnTime = Time.time;
            activeShapes.Enqueue(tracker);

            StartCoroutine(StartShapeTimer(tracker));

            yield return new WaitForSeconds(3f); // Wait before allowing next spawn (optional pacing)
        }
    }

    IEnumerator StartShapeTimer(ShapeTracker tracker)
    {
        float startTime = Time.time;

        while (Time.time - startTime < timePerShape)
        {
            // Show remaining time only for the first shape
            if (activeShapes.Count > 0 && activeShapes.Peek() == tracker)
            {
                float remaining = timePerShape - (Time.time - startTime);
                if (timerText != null)
                    timerText.text = "Time Left: " + Mathf.CeilToInt(remaining) + "s";
            }

            if (tracker.validator != null && tracker.validator.isSolved)
            {
                score += 10; // Add score for correct
                RemoveAndSpawnNext(tracker);
                yield break;
            }

            yield return null;
        }

        // Time expired — if still unsolved
        if (tracker.validator != null && !tracker.validator.isSolved)
        {
            Debug.Log("⏱️ Time up for: " + tracker.gameObject.name);
            Destroy(tracker.gameObject);
            RemoveAndSpawnNext(tracker);
        }
    }

    void RemoveAndSpawnNext(ShapeTracker tracker)
    {
        if (activeShapes.Contains(tracker))
        {
            activeShapes.Dequeue(); // Remove the oldest tracked shape
            StartCoroutine(SpawnAndTrack()); // Trigger next spawn
        }
    }
}

public class ShapeTracker : MonoBehaviour
{
    public ShapeValidator validator;
    public float spawnTime;
}
