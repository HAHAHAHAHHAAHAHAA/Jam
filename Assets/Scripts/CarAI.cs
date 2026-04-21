using UnityEngine;

public class CarAI : MonoBehaviour
{
    [Header("Navigation")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float speed = 5f;

    [Header("Distance Check")]
    [SerializeField] private Transform player;
    [SerializeField] private float loseDistance = 30f;

    private int currentWaypoint = 0;
    private Vector3 targetPosition;
    private bool isGameEnded = false;
    private bool isWaitingForVictory = false;
    private float victoryTimer = 0f;

    void Start()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
        if (waypoints.Length > 0) targetPosition = waypoints[0].position;
    }

    void Update()
    {
        if (isGameEnded) return;

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
        transform.position += direction * speed * Time.deltaTime;

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
        ComicManager.Instance?.Victory();
    }

    private void OnDefeat()
    {
        if (isGameEnded) return;
        isGameEnded = true;
        Debug.Log("Поражение! Игрок отдалился от машины");
        ComicManager.Instance?.GameOver();
    }
}