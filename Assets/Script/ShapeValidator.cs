using UnityEngine;

public class ShapeValidator : MonoBehaviour
{
    [Header("Validation")]
    public string expectedTag = "CorrectNet";
    public Transform assignedSpawnPoint;

    [Header("Feedback")]
    public float glowDuration = 0.5f;
    public float popScale = 1.2f;
    public float popDuration = 0.3f;
    public float shakeDuration = 0.4f;
    public float shakeIntensity = 0.1f;
    public bool isSolved = false;

    [Header("Material")]
    public Material glowMaterial;
    public Material greenMaterial;
    private Material originalMaterial;

    private Renderer rend;
    private Vector3 originalScale;
    private bool isAnimating = false;


    void Start()
    {
        rend = GetComponent<Renderer>();
        originalMaterial = rend.material;
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAnimating) return;
        Debug.Log(other.gameObject.layer);

        bool isCorrect = other.CompareTag(expectedTag);

        if (isCorrect)
        {
            Destroy(other.gameObject);
            // isSolved = true;
            StartCoroutine(GlowAndPop());
        }
        else
        {
            StartCoroutine(ShakeAndShrink());
        }
    }

    System.Collections.IEnumerator GlowAndPop()
    {
        isAnimating = true;
        rend.material = glowMaterial;

        // Pop animation
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
        // Set to permanent green
        rend.material = greenMaterial;

        // Wait for glow to finish
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

        // Shrink
        float shrinkTime = 0.2f;
        t = 0;
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
