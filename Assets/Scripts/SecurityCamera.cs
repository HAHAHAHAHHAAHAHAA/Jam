using UnityEngine;

public class SecurityCamera : EnemyVision
{
    [Header("Camera Rotation")]
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float leftAngle = -45f;
    [SerializeField] private float rightAngle = 45f;

    private float currentAngle = 0f;
    private float direction = 1f;

    void FixedUpdate()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        float step = direction * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, step, 0);

        currentAngle += step;

        if (currentAngle >= rightAngle)
        {
            currentAngle = rightAngle;
            direction = -1f;
        }
        else if (currentAngle <= leftAngle)
        {
            currentAngle = leftAngle;
            direction = 1f;
        }
    }

    protected override void OnFullDetection()
    {
        Debug.Log("Камера обнаружила игрока!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}