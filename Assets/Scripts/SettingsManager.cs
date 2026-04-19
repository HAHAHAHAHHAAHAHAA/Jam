using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [SerializeField] private float defaultSensitivity = 2f;
    [SerializeField] private float defaultMasterVolume = 1f;
    [SerializeField] private float defaultMusicVolume = 1f;
    [SerializeField] private float defaultSFXVolume = 1f;

    private float mouseSensitivity;
    private float masterVolume;
    private float musicVolume;
    private float sfxVolume;

    public System.Action OnSettingsChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", defaultSFXVolume);
        ApplySettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void ApplySettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(masterVolume);
            AudioManager.Instance.SetMusicVolume(musicVolume);
            AudioManager.Instance.SetSFXVolume(sfxVolume);
        }

        OnSettingsChanged?.Invoke();
    }

    public void SetSensitivity(float value)
    {
        mouseSensitivity = value;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        SaveSettings();
        ApplySettings();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        SaveSettings();
        ApplySettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        SaveSettings();
        ApplySettings();
    }

    public float GetSensitivity() => mouseSensitivity;
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}