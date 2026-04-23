using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float defaultMouseSensitivity = 2f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 1f;
    [Header("Footstep Audio")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float crouchStepInterval = 0.7f;

    private float stepTimer = 0f;
    private float currentStepInterval => isCrouching ? crouchStepInterval : (isSprinting ? sprintStepInterval : walkStepInterval);
    private CharacterController controller;
    private Camera playerCamera;
    private float yaw = 0f;
    private float pitch = 0f;
    private float verticalVelocity = 0f;
    private float gravity = -9.81f;
    private bool isCrouching = false;
    private Weapon currentWeapon;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting = false;
    private float mouseSensitivity;
    private bool isInputEnabled = true;
    void Start()
    {
        InitializeComponents();
        SetupPlayerInput();
        LockCursor(true);
        SetCrouchState(false);
        currentWeapon = GetComponentInChildren<Weapon>();

        if (SettingsManager.Instance != null)
        {
            mouseSensitivity = SettingsManager.Instance.GetSensitivity();
            SettingsManager.Instance.OnSettingsChanged += UpdateSensitivity;
        }
        else
        {
            mouseSensitivity = defaultMouseSensitivity;
        }
    }

    void OnDestroy()
    {
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.OnSettingsChanged -= UpdateSensitivity;
        }
    }

    void Update()
    {
        if (!isInputEnabled) return;
        HandleMouseLook();
        HandleMovement();
        HandleFootsteps();
    }

    private void UpdateSensitivity()
    {
        if (SettingsManager.Instance != null)
        {
            mouseSensitivity = SettingsManager.Instance.GetSensitivity();
        }
    }

    private void InitializeComponents()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    private void SetupPlayerInput()
    {
        if (GetComponent<PlayerInput>() == null)
        {
            gameObject.AddComponent<PlayerInput>();
        }
    }
    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
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

    private void SetCrouchState(bool crouching)
    {
        isCrouching = crouching;
        controller.height = isCrouching ? crouchHeight : standHeight;

        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = isCrouching ? 0.3f : 0.6f;
        playerCamera.transform.localPosition = camPos;
    }

    private void HandleMouseLook()
    {
        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float speed = GetCurrentSpeed();

        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * speed * Time.deltaTime);
    }
    private void HandleFootsteps()
    {
        bool isMoving = moveInput.magnitude > 0.1f && controller.isGrounded;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = currentStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        if (clip != null && AudioManager.Instance != null)
        {
            float volume = isCrouching ? 0.5f : (isSprinting ? 1f : 0.75f);
            float pitch = Random.Range(0.9f, 1.1f);
            AudioManager.Instance.PlaySound(clip, transform.position, volume, pitch);
        }
    }
    private float GetCurrentSpeed()
    {
        if (isCrouching) return crouchSpeed;
        if (isSprinting && moveInput.y > 0) return sprintSpeed;
        return walkSpeed;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public void OnAttack(InputValue value)
    {
        if (!isInputEnabled) return;
        if (currentWeapon != null) currentWeapon.Shoot();
    }

    public void OnReload(InputValue value)
    {
        if (!isInputEnabled) return;
        if (currentWeapon != null) currentWeapon.Reload();
    }

    public void OnMove(InputValue value)
    {
        if (!isInputEnabled) return;
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (!isInputEnabled) return;
        lookInput = value.Get<Vector2>();
    }

    public void OnCrouch(InputValue value)
    {
        if (!isInputEnabled) return;
        if (value.isPressed) SetCrouchState(!isCrouching);
    }

    public void OnSprint(InputValue value)
    {
        if (!isInputEnabled) return;
        isSprinting = value.isPressed;
    }
    public void OnToggleSettings(InputValue value)
    {
        SettingsManager.Instance.ToggleSettings();
    }
}