using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifeTime = 5f;

    private Vector3 lastPosition;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 newPosition = transform.position + transform.forward * speed * Time.deltaTime;
        Vector3 direction = newPosition - lastPosition;
        float distance = direction.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(lastPosition, direction.normalized, out hit, distance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerStats playerStats = hit.collider.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                }
            }

            if (!hit.collider.CompareTag("Enemy"))
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.position = newPosition;
        lastPosition = newPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}