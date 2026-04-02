using UnityEngine;

public class ITest : MonoBehaviour, IInteractable
{
    private bool interacted = false;
    [SerializeField] private string imiePrzed;
    [SerializeField] private string imiePo;
    public void OnInteract(GameObject interactor)
    {
        if (!interacted)
        {
            Debug.Log($"Interakcja przez {interactor.name}!");
            interacted = true;
        }
        else
        {
            Debug.Log("Ju¿ by³a interakcja.");
        }
    }

    public string GetInteractText()
    {
        return interacted ? imiePo : imiePrzed;
    }
    public Transform GetTransform()
    {
        return transform;
    }
}
