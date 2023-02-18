using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public void OpenChest()
    {
        gameObject.GetComponent<Animator>().enabled = true;

        print("TODO: drop item");
    }
}
