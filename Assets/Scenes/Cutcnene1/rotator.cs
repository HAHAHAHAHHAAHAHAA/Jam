using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Скорость вращения (градусов в секунду)

    void Update()
    {
        // Вращаем объект вокруг оси Y
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}