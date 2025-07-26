using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetIfMisplaced : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private bool isBeingHeld = false;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isBeingHeld = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isBeingHeld = false;

        // Check if solved before resetting
        ShapeValidator validator = GetComponent<ShapeValidator>();
        if (validator == null || !validator.isSolved)
        {
            ResetToOriginalPosition();
        }
    }

    private void ResetToOriginalPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
