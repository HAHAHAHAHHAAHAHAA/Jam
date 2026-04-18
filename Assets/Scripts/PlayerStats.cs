using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"Игрок получил {amount} урона. Осталось {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Игрок вылечил {amount}. Теперь {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        Debug.Log("Игрок умер");
        Time.timeScale = 0f;
    }

    public float GetHealth() => currentHealth;
}