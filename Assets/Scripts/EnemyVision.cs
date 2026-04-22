using UnityEngine;
using UnityEngine.UI;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] protected float viewRange = 15f;
    [SerializeField] protected float viewAngle = 60f;
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] protected LayerMask obstacleMask;
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private Image detectionBar;
    [SerializeField] private float detectTime = 1.5f;

    private FPSController playerController;
    private Camera playerCamera;
    private float currentDetection = 0f;
    private bool canSeePlayer = false;
    private bool isInDetectionRange = false;

    private void Awake()
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
    public void ForceDetect()
    {
        currentDetection = 1f;
        OnFullDetection();
    }

    public void NotifyShot(Vector3 shotPosition)
    {
        float distanceToShot = Vector3.Distance(transform.position, shotPosition);

        if (distanceToShot <= viewRange)
        {
            currentDetection += 0.9f;
            if (currentDetection >= 1f)
            {
                currentDetection = 1f;
                OnFullDetection();
            }
        }
    }
    private void UpdateDetectionBarUI()
    {
        if (detectionBar == null) return;

        detectionBar.transform.LookAt(playerCamera.transform);
        detectionBar.fillAmount = currentDetection;
    }

    protected virtual void OnFullDetection()
    {
        GetComponent<Enemy>()?.OnDetected();
    }

    public bool IsDetected()
    {
        return currentDetection >= 1f;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewRange;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewRange;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}