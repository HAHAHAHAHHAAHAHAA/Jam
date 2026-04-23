using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene2 : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("StealthMission");
    }
}
