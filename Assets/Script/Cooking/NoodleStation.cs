
using UnityEngine;

public class NoodleStation : MonoBehaviour, IInteractable
{
    public CookingManager cookingManager;

    public void Interact()
    {
        if (cookingManager == null)
        {
            Debug.LogError("CookingManager is missing!");
            return;
        }

        cookingManager.AddNoodle();
    }
}
