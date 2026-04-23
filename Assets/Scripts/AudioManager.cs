using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private GameObject sfxPrefab;

    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

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

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null || sfxPrefab == null) return;

        GameObject sfxObject = Instantiate(sfxPrefab, position, Quaternion.identity);
        AudioSource source = sfxObject.GetComponent<AudioSource>();

        if (source != null)
        {
            source.clip = clip;
            source.volume = volume * sfxVolume * masterVolume;
            source.pitch = pitch;
            source.Play();
            Destroy(sfxObject, clip.length);
        }
    }
    public void PlaySound2D(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxPrefab == null) return;

        GameObject sfxObject = Instantiate(sfxPrefab, Vector3.zero, Quaternion.identity);
        AudioSource source = sfxObject.GetComponent<AudioSource>();

        if (source != null)
        {
            source.spatialBlend = 0f;
            source.clip = clip;
            source.volume = volume * sfxVolume * masterVolume;
            source.Play();
            Destroy(sfxObject, clip.length);
        }
    }
    public void PlaySoundOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxPrefab == null) return;

        GameObject sfxObject = Instantiate(sfxPrefab, Vector3.zero, Quaternion.identity);
        AudioSource source = sfxObject.GetComponent<AudioSource>();

        if (source != null)
        {
            source.volume = volume * sfxVolume * masterVolume;
            source.PlayOneShot(clip);
            Destroy(sfxObject, clip.length);
        }
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = musicVolume * masterVolume;
    }

    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}