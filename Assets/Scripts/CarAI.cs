using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float startDelay = 2f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [Header("Distance Check")]
    [SerializeField] private Transform player;
    [SerializeField] private float loseDistance = 30f;
    private float currentSpeed = 0f;
    private int currentWaypoint = 0;
    private Vector3 targetPosition;
    private bool isGameEnded = false;
    private bool isWaitingForVictory = false;
    private float victoryTimer = 0f;
    private float startTimer = 0f;
    private bool hasStarted = false;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (waypoints.Length > 0) targetPosition = waypoints[0].position;
        startTimer = startDelay;
    }

    void Update()
    {
        if (isGameEnded) return;

        if (!hasStarted)
        {
            startTimer -= Time.deltaTime;
            if (startTimer <= 0f)
            {
                hasStarted = true;
            }
            return;
        }

        if (isWaitingForVictory)
        {
            victoryTimer -= Time.deltaTime;
            if (victoryTimer <= 0f)
            {
                OnVictory();
            }
            return;
        }

        CheckDistance();
        FollowWaypoints();
    }

    private void FollowWaypoints()
    {
        if (waypoints.Length == 0) return;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance < 2f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                isWaitingForVictory = true;
                victoryTimer = 2f;
                return;
            }
            targetPosition = waypoints[currentWaypoint].position;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;

        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        transform.position += direction * currentSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void CheckDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > loseDistance)
        {
            OnDefeat();
        }
    }

    private void OnVictory()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        TownManager.Instance?.Win();
    }

    private void OnDefeat()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        Debug.Log("Поражение! Игрок отдалился от машины");
        TownManager.Instance?.Lose();
    }
}