using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 20f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float chaseMemoryTime = 5f;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem customMuzzleFlash;
    [SerializeField] private AudioClip shotClip;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;

    private EnemyVision vision;
    private float nextAttackTime;
    private bool isDead;
    private float chaseMemoryTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        vision = GetComponent<EnemyVision>();
    }

    void Update()
    {
        if (isDead) return;
        HandleBehavior();
    }

    private void HandleBehavior()
    {
        bool isDetected = vision != null && vision.IsDetected();

        if (isDetected)
        {
            chaseMemoryTimer = chaseMemoryTime;

            if (!isChasing)
            {
                isChasing = true;
            }
            ChaseAndAttack();
        }
        else if (chaseMemoryTimer > 0)
        {
            chaseMemoryTimer -= Time.deltaTime;
            ChaseAndAttack();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
            }
        }
    }

    private void ChaseAndAttack()
    {
        Chase();

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Attack();
        }
    }

    private void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        Vector3 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
    }

    private void Attack()
    {
        agent.isStopped = true;

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        Shoot();
    }

    private void Shoot()
    {
        if (customMuzzleFlash != null)
        {
            ParticleManager.Instance?.PlayParticle(customMuzzleFlash, firePoint.position, firePoint.rotation, 0.5f);
        }
        else
        {
            ParticleManager.Instance?.PlayMuzzleFlash(firePoint.position, firePoint.rotation);
        }

        if (shotClip != null)
        {
            AudioManager.Instance?.PlaySound(shotClip, firePoint.position, 0.5f);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Destroy(bullet, 5f);
    }
    public void OnDetected()
    {

    }
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        ParticleManager.Instance?.PlayExplosion(transform.position);
        Destroy(gameObject, 2f);
    }
}