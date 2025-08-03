using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MaterialValidator : MonoBehaviour
{
    [Header("Material Info")]
    public MaterialType typeOfMaterial;  // Set this in the prefab
    public string materialName;

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

        // Clean name if not already set
        if (string.IsNullOrEmpty(materialName))
        {
            materialName = CleanName(gameObject.name);
        }
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

    IEnumerator GlowAndPop()
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

        // Set to green to indicate correct
        rend.material = greenMaterial;

        // Wait briefly
        yield return new WaitForSeconds(glowDuration);

        // Disable grabbing interaction
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.interactionLayers = 0;
            grab.enabled = false;
        }

        // Lock movement/physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Snap and parent inside the drop box
        if (lastDropBoxTransform != null)
        {
            transform.SetParent(lastDropBoxTransform);
            transform.localPosition = GetNextAvailableSpot(lastDropBoxTransform);
        }

        isSolved = true;
        isAnimating = false;
    }

    IEnumerator ShakeAndShrink()
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
        float offsetY = 0.1f; // Vertical stacking
        return new Vector3(0, offsetY * count, 0);
    }

    private string CleanName(string rawName)
    {
        // Removes " (Clone)" or " (1)" etc. from name
        int index = rawName.IndexOf('(');
        if (index > 0)
            rawName = rawName.Substring(0, index);

        return rawName.Trim();
    }
}
