using UnityEngine;
using UnityEngine.InputSystem;
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
        PlayerInput input = player.GetComponent<PlayerInput>();
        FPSController FPSC = player.GetComponent<FPSController>();
        input.enabled = false;
        FPSC.enabled = false;
        animator.enabled = true;
    }
}
