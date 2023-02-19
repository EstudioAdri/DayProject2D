using UnityEngine;

public class Chest : MonoBehaviour
{
    #region PublicMethods

    public void OpenChest() // Invoked
    {
        gameObject.GetComponent<Animator>().enabled = true;

        print("TODO: drop item");
    }

    #endregion
}
