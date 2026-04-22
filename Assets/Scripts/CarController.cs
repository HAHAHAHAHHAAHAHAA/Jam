using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float motorTorque = 600f;
    [SerializeField] private float brakeTorque = 3000f;
    [SerializeField] private float maxSteeringAngle = 28f;

    [Header("Steering Smoothing")]
    [SerializeField] private float steeringSpeed = 3f;
    [SerializeField] private float brakeForce = 4000f;

    [Header("Engine Sound")]
    [SerializeField] private AudioSource engineSource;
    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float volumeSpeed = 2f;

    [Header("Колёса")]
    [SerializeField] private WheelCollider frontLeftWheel;
    [SerializeField] private WheelCollider frontRightWheel;
    [SerializeField] private WheelCollider rearLeftWheel;
    [SerializeField] private WheelCollider rearRightWheel;

    [Header("Визуал")]
    [SerializeField] private Transform frontLeftMesh;
    [SerializeField] private Transform frontRightMesh;
    [SerializeField] private Transform rearLeftMesh;
    [SerializeField] private Transform rearRightMesh;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteeringAngle;
    private float targetSteeringAngle;
    private bool isBraking;
    private float currentVolume = 0.1f;

    void Start()
    {
        LockCursor(true);

        if (engineSource != null)
        {
            engineSource.volume = minVolume;
            engineSource.Play();
        }
    }

    void Update()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheelMeshes();
        HandleEngineSound();
    }
    private void FixedUpdate()
    {
        StabilizeCar();
    }
    private void HandleMotor()
    {
        rearLeftWheel.motorTorque = verticalInput * motorTorque;
        rearRightWheel.motorTorque = verticalInput * motorTorque;

        if (isBraking)
        {
            frontLeftWheel.brakeTorque = brakeTorque;
            frontRightWheel.brakeTorque = brakeTorque;
            rearLeftWheel.brakeTorque = brakeForce;
            rearRightWheel.brakeTorque = brakeForce;
        }
        else
        {
            frontLeftWheel.brakeTorque = 0;
            frontRightWheel.brakeTorque = 0;
            rearLeftWheel.brakeTorque = 0;
            rearRightWheel.brakeTorque = 0;
        }
    }

    private void HandleSteering()
    {
        targetSteeringAngle = maxSteeringAngle * horizontalInput;
        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, targetSteeringAngle, Time.deltaTime * steeringSpeed);

        frontLeftWheel.steerAngle = currentSteeringAngle;
        frontRightWheel.steerAngle = currentSteeringAngle;
    }

    private void HandleEngineSound()
    {
        if (engineSource == null) return;

        float targetVolume = Mathf.Abs(verticalInput);
        targetVolume = Mathf.Clamp(targetVolume, minVolume, maxVolume);

        currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * volumeSpeed);

        if (AudioManager.Instance != null)
        {
            engineSource.volume = currentVolume * AudioManager.Instance.GetSFXVolume() * AudioManager.Instance.GetMasterVolume();
        }
        else
        {
            engineSource.volume = currentVolume;
        }
    }

    private void UpdateWheelMeshes()
    {
        UpdateWheelMesh(frontLeftWheel, frontLeftMesh);
        UpdateWheelMesh(frontRightWheel, frontRightMesh);
        UpdateWheelMesh(rearLeftWheel, rearLeftMesh);
        UpdateWheelMesh(rearRightWheel, rearRightMesh);
    }

    private void UpdateWheelMesh(WheelCollider collider, Transform mesh)
    {
        if (mesh == null) return;

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        mesh.position = position;
        mesh.rotation = rotation;
    }
    private void StabilizeCar()
    {
        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * 0.1f);
    }
    private void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
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