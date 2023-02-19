using UnityEngine;
using UnityEngine.Events;
using static Enums;

public class Interactable : MonoBehaviour
{
    #region Parameters

    public bool IsInteractable { get { return isInteractable; } }
    
    #endregion
    
    #region PublicVariables
    
    public UnityEvent interactableAction;
    
    #endregion
    
    #region PrivateVariables
    
    [SerializeField] uint numberOfUses;
    [SerializeField] bool isInteractable;

    #endregion

    #region UnityEvents

    private void Awake()
    {
        gameObject.tag = Tags.interactable.ToString();
    }

    #endregion

    #region PublicMethods

    public void Interact()
    {
        if (!isInteractable)
            return;
        
        numberOfUses--;

        interactableAction.Invoke();

        if (numberOfUses <= 0)
        {
            isInteractable = false;
        }
    }

    #endregion
}
