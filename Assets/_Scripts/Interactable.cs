using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Enums;

public class Interactable : MonoBehaviour
{
    public UnityEvent interactableAction;
    public bool IsInteractable { get { return isInteractable; } }

    [SerializeField] uint numberOfUses;
    [SerializeField] bool isInteractable;

    private void Awake()
    {
        gameObject.tag = Tags.interactable.ToString();
        print($"{gameObject.name} tag set to {Tags.interactable}");
    }

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
}
