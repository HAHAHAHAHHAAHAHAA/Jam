using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private float defaultSensitivity = 2f;
    [SerializeField] private float defaultMasterVolume = 1f;
    [SerializeField] private float defaultMusicVolume = 1f;
    [SerializeField] private float defaultSFXVolume = 1f;
    [SerializeField] private GameObject settingsPanel;
    private bool isSettingsOpen = false;
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
    void Start()
    {
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = mouseSensitivity;
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
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

    public void ToggleSettings()
    {
        isSettingsOpen = !isSettingsOpen;

        if (settingsPanel != null) settingsPanel.SetActive(isSettingsOpen);

        Cursor.lockState = isSettingsOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isSettingsOpen;
        Time.timeScale = isSettingsOpen ? 0f : 1f;

        FPSController fps = FindObjectOfType<FPSController>();
        if (fps != null) fps.SetInputEnabled(!isSettingsOpen);
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

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
    public float GetSensitivity() => mouseSensitivity;
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}