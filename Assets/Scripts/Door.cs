using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    public void Interact()
    {
        if (isMoving) return;

        if (isOpen)
        {
            StartCoroutine(RotateDoor(closedRotation));
            isOpen = false;
        }
        else
        {
            StartCoroutine(RotateDoor(openRotation));
            isOpen = true;
        }
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        isMoving = true;
        Quaternion startRotation = transform.rotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        isMoving = false;
    }
}