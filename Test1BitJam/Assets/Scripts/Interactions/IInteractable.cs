using UnityEngine;

public interface IInteractable
{
    void OnInteract(GameObject interactor);

    string GetInteractText();

    Transform GetTransform();
}