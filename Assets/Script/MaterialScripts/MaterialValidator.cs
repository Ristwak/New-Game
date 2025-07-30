using UnityEngine;

public class MaterialValidator : MonoBehaviour
{
    [Header("Material Info")]
    public MaterialType typeOfMaterial;  // Set this in the prefab

    [Header("Feedback")]
    public float glowDuration = 0.5f;
    public float popScale = 1.2f;
    public float popDuration = 0.3f;
    public float shakeDuration = 0.4f;
    public float shakeIntensity = 0.1f;
    public bool isSolved = false;

    [Header("Materials")]
    public Material glowMaterial;
    public Material greenMaterial;
    private Material originalMaterial;

    private Renderer rend;
    private Vector3 originalScale;
    private bool isAnimating = false;

    public Transform assignedSpawnPoint;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAnimating || isSolved) return;

        DropBox dropBox = other.GetComponent<DropBox>();
        if (dropBox == null) return;

        bool isCorrect = dropBox.acceptedType == typeOfMaterial;

        if (isCorrect)
        {
            Debug.Log("✅ Correct material placed: " + gameObject.name);
            StartCoroutine(GlowAndPop());
        }
        else
        {
            Debug.Log("❌ Incorrect placement for: " + gameObject.name);
            StartCoroutine(ShakeAndShrink());
        }
    }

    System.Collections.IEnumerator GlowAndPop()
    {
        isAnimating = true;
        rend.material = glowMaterial;

        Vector3 targetScale = originalScale * popScale;
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(1f, popScale, t / popDuration);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        transform.localScale = originalScale;
        rend.material = greenMaterial;
        yield return new WaitForSeconds(glowDuration);

        gameObject.SetActive(false);

        isAnimating = false;
        isSolved = true;
    }

    System.Collections.IEnumerator ShakeAndShrink()
    {
        isAnimating = true;
        Vector3 startPos = transform.position;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float offset = Mathf.Sin(t * 40f) * shakeIntensity;
            transform.position = startPos + new Vector3(offset, 0, 0);
            yield return null;
        }

        transform.position = startPos;

        float shrinkTime = 0.2f;
        t = 0f;
        while (t < shrinkTime)
        {
            t += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0.85f, t / shrinkTime);
            transform.localScale = originalScale * scale;
            yield return null;
        }

        transform.localScale = originalScale;
        isAnimating = false;
    }
}

