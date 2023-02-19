using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    [SerializeField] bool left;
    [SerializeField] bool right;
    [SerializeField] float speed;
    [SerializeField] float rotationSpeed;
    float currentRotation;

    private void Awake()
    {       
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;
        Quaternion rotation = new Quaternion();
        if (right)
        {
            movement = Vector3.right;
            currentRotation -= rotationSpeed;
        }
        else if (left)
        {
            movement= Vector3.left;
            currentRotation += rotationSpeed;
        }
        else
        {
            movement = Vector3.zero;          
        }

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
        transform.position += movement * speed;     
    }

}
