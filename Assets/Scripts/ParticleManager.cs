using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    [SerializeField] private ParticleSystem defaultBloodEffect;
    [SerializeField] private ParticleSystem defaultMuzzleFlash;
    [SerializeField] private ParticleSystem defaultExplosion;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayParticle(ParticleSystem particlePrefab, Vector3 position, Quaternion rotation, float duration = 2f)
    {
        if (particlePrefab == null) return;

        ParticleSystem particle = Instantiate(particlePrefab, position, rotation);
        particle.Play();
        Destroy(particle.gameObject, duration);
    }

    public void PlayBlood(Vector3 position)
    {
        PlayParticle(defaultBloodEffect, position, Quaternion.identity);
    }

    public void PlayMuzzleFlash(Vector3 position, Quaternion rotation)
    {
        PlayParticle(defaultMuzzleFlash, position, rotation, 0.5f);
    }

    public void PlayExplosion(Vector3 position)
    {
        PlayParticle(defaultExplosion, position, Quaternion.identity);
    }
}