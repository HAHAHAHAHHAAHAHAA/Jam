using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTime = 2f;

    private NavMeshAgent agent;
    private int currentWaypoint;
    private float waitTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
    }

    void Update()
    {
        HandlePatrol();
    }

    private void HandlePatrol()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer = waitTime;
            GoToNextWaypoint();
        }
    }

    private void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    public void EnablePatrol(bool enable)
    {
        this.enabled = enable;
        if (enable)
        {
            GoToNextWaypoint();
        }
        else
        {
            agent.isStopped = true;
        }
    }
}