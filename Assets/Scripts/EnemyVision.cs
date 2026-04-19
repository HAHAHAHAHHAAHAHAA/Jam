using UnityEngine;
using UnityEngine.UI;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private float viewRange = 15f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private Image detectionBar;
    [SerializeField] private float detectTime = 1.5f;

    private FPSController playerController;
    private Camera playerCamera;
    private float currentDetection = 0f;
    private bool canSeePlayer = false;
    private bool isInDetectionRange = false;

    void Start()
    {
        InitializeReferences();
        SetupDetectionBar();
    }

    void Update()
    {
        CheckVision();
        UpdateDetection();
        UpdateDetectionBarUI();
    }

    private void InitializeReferences()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<FPSController>();
        playerCamera = Camera.main;
    }

    private void SetupDetectionBar()
    {
        if (detectionBar != null)
        {
            detectionBar.fillAmount = 0;
        }
    }

    private void CheckVision()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        isInDetectionRange = distanceToPlayer <= detectionRange;

        if (isInDetectionRange)
        {
            canSeePlayer = true;
            return;
        }

        float effectiveRange = GetEffectiveRange();

        if (!IsWithinRange(distanceToPlayer, effectiveRange))
        {
            canSeePlayer = false;
            return;
        }

        if (!IsWithinViewAngle(directionToPlayer))
        {
            canSeePlayer = false;
            return;
        }

        canSeePlayer = !HasObstacle(directionToPlayer, distanceToPlayer);
    }

    private float GetEffectiveRange()
    {
        if (playerController != null && playerController.IsCrouching())
        {
            return viewRange * 0.5f;
        }
        return viewRange;
    }

    private bool IsWithinRange(float distance, float range)
    {
        return distance <= range;
    }

    private bool IsWithinViewAngle(Vector3 direction)
    {
        float angle = Vector3.Angle(transform.forward, direction);
        return angle <= viewAngle / 2;
    }

    private bool HasObstacle(Vector3 direction, float distance)
    {
        return Physics.Raycast(transform.position, direction, distance, obstacleMask);
    }

    private void UpdateDetection()
    {
        if (canSeePlayer)
        {
            if (isInDetectionRange)
            {
                if (currentDetection < 1f)
                {
                    currentDetection = 1f;
                    OnFullDetection();
                }
            }
            else
            {
                currentDetection += Time.deltaTime / detectTime;
                if (currentDetection >= 1f)
                {
                    currentDetection = 1f;
                    OnFullDetection();
                }
            }
        }
        else if (currentDetection < 1f)
        {
            currentDetection -= Time.deltaTime / detectTime;
            if (currentDetection <= 0f)
            {
                currentDetection = 0f;
            }
        }

        currentDetection = Mathf.Clamp01(currentDetection);
    }

    private void UpdateDetectionBarUI()
    {
        if (detectionBar == null) return;

        detectionBar.transform.LookAt(playerCamera.transform);
        detectionBar.fillAmount = currentDetection;
    }

    private void OnFullDetection()
    {
        GetComponent<Enemy>()?.OnDetected();
    }

    public bool IsDetected()
    {
        return currentDetection >= 1f;
    }
}