using UnityEngine;
using UnityEngine.SceneManagement;

public class Folder : MonoBehaviour, IInteractable
{
    public void Interact()
    {

    }
    public void ColletFolder()
    {
        SceneManager.LoadScene("Stoorm");
        Destroy(gameObject);
    }
}
