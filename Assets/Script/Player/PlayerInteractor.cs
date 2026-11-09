
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Camera playerCamera;
    public float interactDistance = 3f;

    [Header("UI Settings")]
    public TextMeshProUGUI interactionText;

    private IInteractable currentInteractable;

    void Update()
    {
        CheckForInteractable();

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    void CheckForInteractable()
    {
        currentInteractable = null;

        if (playerCamera == null)
        {
            HideUI();
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                InteractionPrompt prompt =
                    hit.collider.GetComponent<InteractionPrompt>();

                string stationName = "Interact";

                if (prompt != null)
                {
                    stationName = prompt.stationName;
                }

                ShowUI(stationName + "  [E]");
                return;
            }
        }

        HideUI();
    }

    void TryInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void ShowUI(string message)
    {
        if (interactionText == null)
            return;

        interactionText.text = message;
        interactionText.gameObject.SetActive(true);
    }

    void HideUI()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}
