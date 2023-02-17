using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool HasBeenInteracted { get
        {
            return isOpen;
        }
        set
        {
            animatorComponent.enabled = true;
            isOpen = HasBeenInteracted;
        }
    }

    bool isOpen;
    Animator animatorComponent;

    private void Start()
    {
        animatorComponent = gameObject.GetComponent<Animator>();
    }
}
