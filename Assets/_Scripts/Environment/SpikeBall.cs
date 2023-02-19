using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] bool left;
    [SerializeField] bool right;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;

    float currentRotation;

    #endregion

    #region UnityEvents

    private void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;
        if (right)
        {
            movement = Vector3.right;
            currentRotation -= rotationSpeed;
        }
        else if (left)
        {
            movement = Vector3.left;
            currentRotation += rotationSpeed;
        }
        else
        {
            movement = Vector3.zero;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        transform.position += movement * speed;
    }

    #endregion
}
