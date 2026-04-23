using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene1 : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("Town");
    }
}
