using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private GameObject interactPromptUI;
    public static InteractionSystem instance;
    private Camera playerCamera;
    private IInteractable currentInteractable;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (interactPromptUI != null) interactPromptUI.SetActive(false);
    }

    void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange) && hit.collider.CompareTag("Interactable"))
        {
            if (currentInteractable == null)
            {
                currentInteractable = hit.collider.GetComponent<IInteractable>();
                if (interactPromptUI != null) interactPromptUI.SetActive(true);
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable = null;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }

    public void OnInteract(InputValue value)
    {
        Debug.Log("EEE");
        currentInteractable.Interact();
    }
}