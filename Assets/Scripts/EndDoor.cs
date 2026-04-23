using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour,IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("cutscene1");
    }
}
