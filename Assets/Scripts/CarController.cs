using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeCarController : MonoBehaviour
{
    [Header("Скорости")]
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float brakeDeceleration = 15f;
    [SerializeField] private float coastDeceleration = 3f;
    [SerializeField] private float maxSpeed = 40f;
    [SerializeField] private float reverseSpeed = 15f;

    [Header("Поворот")]
    [SerializeField] private float minTurnSpeed = 20f;
    [SerializeField] private float maxTurnSpeed = 90f;
    [SerializeField] private float fullTurnSpeedThreshold = 20f;
    [SerializeField] private float steeringSmoothTime = 0.1f;

    [Header("Инерция и занос")]
    [SerializeField] private float driftFactor = 0.95f;

    [Header("Визуал колёс")]
    [SerializeField] private Transform frontLeftMesh;
    [SerializeField] private Transform frontRightMesh;
    [SerializeField] private Transform rearLeftMesh;
    [SerializeField] private Transform rearRightMesh;
    [SerializeField] private float maxVisualTurnAngle = 30f;
    [SerializeField] private float wheelRotationSpeed = 360f;

    private float currentSpeed = 0f;
    private float currentSteerAngle = 0f;
    private float steerVelocity = 0f;
    private float wheelRotation = 0f;

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Замораживаем только вращение по осям X и Z, чтобы машина не опрокидывалась
            // Ось Y оставляем свободной для поворотов рулём
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        UpdateWheelVisuals();
    }

    private void HandleMovement()
    {
        // === Управление скоростью ===
        float targetSpeed = 0f;
        if (verticalInput > 0.1f)
            targetSpeed = maxSpeed;
        else if (verticalInput < -0.1f)
            targetSpeed = -reverseSpeed;

        float rate = acceleration;
        if (isBraking)
            rate = brakeDeceleration;
        else if (Mathf.Abs(verticalInput) < 0.1f)
            rate = coastDeceleration;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, rate * Time.deltaTime);
        }

        // === Поворот с динамической скоростью ===
        float turnSpeedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / fullTurnSpeedThreshold);
        float currentTurnSpeed = Mathf.Lerp(minTurnSpeed, maxTurnSpeed, turnSpeedFactor);

        float targetSteer = horizontalInput * currentTurnSpeed;
        currentSteerAngle = Mathf.SmoothDamp(currentSteerAngle, targetSteer, ref steerVelocity, steeringSmoothTime);

        transform.Rotate(0f, currentSteerAngle * Time.deltaTime, 0f);

        // === Перемещение с учётом гравитации ===
        Vector3 forwardVelocity = transform.forward * currentSpeed;

        if (rb != null)
        {
            // Сохраняем текущую вертикальную скорость (гравитация/прыжки)
            float verticalVelocity = rb.linearVelocity.y;

            // Боковая скорость с учётом дрифта
            Vector3 lateralVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);

            // Итоговая скорость
            Vector3 newVelocity = forwardVelocity + lateralVelocity * driftFactor;
            newVelocity.y = verticalVelocity; // <-- важно: гравитация не теряется

            rb.linearVelocity = newVelocity;
        }
        else
        {
            // Без Rigidbody гравитация не работает — только для отладки
            transform.position += forwardVelocity * Time.deltaTime;
        }
    }

    private void UpdateWheelVisuals()
    {
        wheelRotation += currentSpeed * wheelRotationSpeed * Time.deltaTime;

        float visualSteer = horizontalInput * maxVisualTurnAngle;

        RotateWheel(frontLeftMesh, visualSteer);
        RotateWheel(frontRightMesh, visualSteer);
        RotateWheel(rearLeftMesh, 0f);
        RotateWheel(rearRightMesh, 0f);
    }

    private void RotateWheel(Transform wheel, float steerAngle)
    {
        if (wheel == null) return;

        wheel.localRotation = Quaternion.Euler(
            wheelRotation,
            steerAngle,
            0f
        );
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        horizontalInput = input.x;
        verticalInput = input.y;
    }

    public void OnBrake(InputValue value)
    {
        isBraking = value.isPressed;
    }
}