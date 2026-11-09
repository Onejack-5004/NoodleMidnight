
using UnityEngine;

public class ToppingStation : MonoBehaviour, IInteractable
{
    public CookingManager cookingManager;

    public void Interact()
    {
        if (cookingManager == null)
        {
            Debug.LogError("CookingManager is missing!");
            return;
        }

        cookingManager.AddTopping();
    }
}
