using UnityEngine;

public class MonologTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip startSound;
    private bool hasStarted = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasStarted && other.CompareTag("Player") && startSound != null)
        {
            hasStarted = true;
            AudioManager.Instance?.PlaySound2D(startSound, 1f);
        }
    }
}