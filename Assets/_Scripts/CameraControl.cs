using UnityEngine;

public class CameraControl : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] float cameraBoundaryX;
    [SerializeField] float cameraBoundaryY;
    
    float cameraSpeed = .1f;
    Player player;

    #endregion

    #region UnityEvents

    private void Start()
    {
        player = FindObjectOfType<Player>();
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }

    void FixedUpdate()
    {
        cameraSpeed = player.PlayerSpeed;
        Vector3 relativePosition = transform.InverseTransformPoint(player.transform.position);
        Vector3 cameraPosition = transform.position;
        if (relativePosition.x > cameraBoundaryX)
        {
            cameraPosition += Vector3.right;
        }
        else if (relativePosition.x < -cameraBoundaryX)
        {
            cameraPosition -= Vector3.right;
        }

        if (player.IsGrounded)
        {
            if (relativePosition.y > cameraBoundaryY)
            {
                cameraPosition += Vector3.up;
            }
            else if (relativePosition.y < -cameraBoundaryY)
            {
                cameraPosition -= Vector3.up;
            }
        }

        cameraPosition = Vector3.Lerp(transform.position, cameraPosition, cameraSpeed);
        transform.position = cameraPosition;
    }

    #endregion
}
