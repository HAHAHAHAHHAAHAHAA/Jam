using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 20f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource shotSound;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;

    private EnemyVision vision;
    private float nextAttackTime;
    private bool isDead;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        vision = GetComponent<EnemyVision>();
    }

    void Update()
    {
        if (isDead) return;

        if (vision != null && vision.IsDetected())
        {
            Chase();

            if (Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                Attack();
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

        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        Shoot();
    }

    private void Shoot()
    {
        muzzleFlash?.Play();
        shotSound?.Play();

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Destroy(bullet, 5f);
    }

    public void OnDetected()
    {
        Debug.Log("Враг обнаружил игрока");
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
        Destroy(gameObject, 2f);
    }
}