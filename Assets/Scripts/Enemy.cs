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

    [Header("Animations")]
    [SerializeField] private Animator animator;

    private EnemyVision vision;
    private float nextAttackTime;
    private bool isDead;
    private float chaseMemoryTimer = 0f;
    private bool isChasing = false;
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        vision = GetComponent<EnemyVision>();

        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;

        HandleBehavior();
        UpdateAnimations();
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

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                isAttacking = true;
                isChasing = false;
                Attack();
            }
            else
            {
                isAttacking = false;
                isChasing = true;
                Chase();
            }
        }
        else if (chaseMemoryTimer > 0)
        {
            chaseMemoryTimer -= Time.deltaTime;
            isAttacking = false;
            isChasing = true;
            Chase();
        }
        else
        {
            if (isChasing || isAttacking)
            {
                isChasing = false;
                isAttacking = false;
                StopMoving();
            }
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

        Vector3 direction = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10f);

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        Shoot();
    }
    private void StopMoving()
    {
        agent.isStopped = true;
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

    private void UpdateAnimations()
    {
        if (animator == null) return;

        if (isDead)
        {
            animator.Play("Dead");
            return;
        }

        bool isMoving = agent.velocity.magnitude > 0.5f;

        if (isMoving)
        {
            animator.SetBool("Running", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            animator.SetBool("Running", false);
            animator.SetBool("Idle", true);
        }
    }
    public void OnDetected()
    {

    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (vision != null)
        {
            vision.ForceDetect();
        }

        if (health <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        isChasing = false;
        isAttacking = false;
        agent.isStopped = true;
        animator.Play("Dead");
        ParticleManager.Instance?.PlayExplosion(transform.position);
        Destroy(gameObject, 2f);
    }
}