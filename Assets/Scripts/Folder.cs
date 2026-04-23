using UnityEngine;
using UnityEngine.SceneManagement;

public class Folder : MonoBehaviour, IInteractable
{
    public Animator animator;
    public GameObject player;
    public void Interact()
    {
        ColletFolder();
    }
    public void ColletFolder()
    {
        FPSController FPSC = player.GetComponent<FPSController>();
        FPSC.enabled = false;
        animator.enabled = true;
    }
}
