using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ComicManager : MonoBehaviour
{
    public static ComicManager Instance { get; private set; }

    [Header("Comics")]
    [SerializeField] private List<Sprite> comicPages;
    [SerializeField] private Image comicImage;
    [SerializeField] private GameObject comicPanel;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName;

    private int currentPage = 0;
    private bool isActive = false;

    void Awake()
    {
        Instance = this;
    }
    public void Victory()
    {
        Debug.Log("Победа!");
        StartComics();
    }

    public void GameOver()
    {
        Debug.Log("Поражение!");
        StartCoroutine(GameOverSequence());
    }

    private void StartComics()
    {
        if (comicPages.Count == 0)
        {
            LoadNextScene();
            return;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AudioManager.Instance?.SetSFXVolume(0f);

       // CarController car = FindObjectOfType<CarController>();
       // if (car != null) car.enabled = false;

        CarAI carAI = FindObjectOfType<CarAI>();
        if (carAI != null) carAI.enabled = false;

        FPSController fps = FindObjectOfType<FPSController>();
        if (fps != null) fps.enabled = false;

        currentPage = 0;
        isActive = true;
        comicPanel.SetActive(true);
        ShowPage(currentPage);
    }

    private void NextPage()
    {
        currentPage++;

        if (currentPage >= comicPages.Count)
        {
            EndComics();
        }
        else
        {
            ShowPage(currentPage);
        }
    }
    public void OnNext()
    {
        Debug.Log("pressnext");
        if (isActive)
        NextPage();
    }
    private void ShowPage(int index)
    {
        if (comicImage != null && comicPages[index] != null)
        {
            comicImage.sprite = comicPages[index];
        }
    }

    private void EndComics()
    {
        isActive = false;
        comicPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        AudioManager.Instance?.SetSFXVolume(1f);

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator GameOverSequence()
    {
        Time.timeScale = 0f;

        if (comicImage != null)
        {
            float elapsed = 0f;
            Color color = comicImage.color;

            while (elapsed < 1f)
            {
                elapsed += Time.unscaledDeltaTime;
                color.a = Mathf.Clamp01(elapsed / 1f);
                comicImage.color = color;
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(2f);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}