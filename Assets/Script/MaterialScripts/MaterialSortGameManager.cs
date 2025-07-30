using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaterialSortGameManager : MonoBehaviour
{
    public static MaterialSortGameManager Instance;

    [Header("Managers")]
    public MaterialSpawnManager spawnManager;
    [Header("Timing and Score Settings")]
    public float totalGameTime = 300f; // Set your total game time here
    public float profitPoints = 10;

    [Header("UI")]
    public GameObject timerPanel;
    public TMP_Text timerText;
    public GameObject scorePanel;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;


    [Header("UV Holder")]
    public GameObject uVHolder;

    private List<MaterialsTracker> activeShapes = new List<MaterialsTracker>();
    private float score = 0;
    private Coroutine timerRoutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("🟢 GameManager started");
        spawnManager.Init();

        // Spawn initial shapes (e.g. 3 to begin)
        for (int i = 0; i < 3; i++)
        {
            SpawnNewShape();
        }

        // Start global game timer
        if (timerRoutine != null)
            StopCoroutine(timerRoutine);
        timerRoutine = StartCoroutine(GlobalGameTimerRoutine());
    }

    void Update()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    IEnumerator GlobalGameTimerRoutine()
    {
        Debug.Log("⏳ Global Game Timer Started");
        float endTime = Time.time + totalGameTime;

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
                    // Score update
                    score += profitPoints;
                    tracker.hasScored = true;

                    // Free spawn point and destroy old shape
                    spawnManager.FreeSpawnPoint(tracker.validator.assignedSpawnPoint);
                    activeShapes.RemoveAt(i);
                    Destroy(tracker.gameObject);

                    // Try to spawn new shape
                    SpawnNewShape();

                    // Check end condition: no shapes left + no active ones
                    if (spawnManager.IsEmpty() && activeShapes.Count == 0)
                    {
                        EndGame();
                        yield break;
                    }
                }
            }

            yield return null;
        }

        Debug.Log("⏱️ Time's up! Ending game.");
        EndGame();
    }

    private void SpawnNewShape()
    {
        GameObject shape = spawnManager.SpawnNextmaterial();
        if (shape != null)
        {
            MaterialsTracker tracker = shape.AddComponent<MaterialsTracker>();
            tracker.validator = shape.GetComponent<MaterialValidator>();
            tracker.spawnTime = Time.time;
            activeShapes.Add(tracker);
        }
    }

    private void EndGame()
    {
        Debug.Log("🏁 All shapes completed or time expired. Game Over!");

        if (timerText != null)
            timerText.text = "Done!";

        timerPanel.SetActive(false);
        uVHolder.SetActive(false);
        foreach (var tracker in activeShapes)
        {
            if (tracker != null)
                tracker.gameObject.SetActive(false);
        }

        gameOverPanel.SetActive(true);
    }
}

public class MaterialsTracker : MonoBehaviour
{
    public MaterialValidator validator;
    public float spawnTime;
    public bool hasScored = false;
}
