using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownManager : MonoBehaviour
{
    public static TownManager Instance { get; private set; }

    [Header("Cutscene")]
    [SerializeField] private GameObject winCutscene;
    [SerializeField] private GameObject car;
    [SerializeField] private GameObject cam;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Win()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        car.SetActive(false);
        cam.transform.position = new Vector3(0,10000000,0);
        CarAI carAI = FindObjectOfType<CarAI>();
        if (carAI != null) carAI.enabled = false;

        if (winCutscene != null)
        {
            winCutscene.SetActive(true);
        }
    }

    public void Lose()
    {
        GameOverManager.Instance.GameOver();

    }
}