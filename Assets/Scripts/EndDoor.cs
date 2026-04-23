using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour,IInteractable
{
    public void Interact()
    {
        Debug.Log("AZAZ");
        SceneManager.LoadScene("zalupa");
    }
}
