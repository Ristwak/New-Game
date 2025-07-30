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
    private Transform lastDropBoxTransform;

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
            lastDropBoxTransform = dropBox.transform; // Save drop target
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

        // Wait for glow duration
        yield return new WaitForSeconds(glowDuration);

        // Disable interaction and physics
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable>();
        if (grab != null)
            grab.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Snap the material into the drop box
        if (lastDropBoxTransform != null)
        {
            transform.SetParent(lastDropBoxTransform); // So it moves with the box if needed
            transform.localPosition = GetNextAvailableSpot(lastDropBoxTransform);
        }

        isSolved = true;
        isAnimating = false;
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
        ResetIfMisplaced.instance.ResetToOriginalPosition();
        isAnimating = false;
    }

    private Vector3 GetNextAvailableSpot(Transform box)
    {
        int count = box.childCount;
        float offsetY = 0.1f; // Adjust spacing
        return new Vector3(0, offsetY * count, 0);
    }
}

