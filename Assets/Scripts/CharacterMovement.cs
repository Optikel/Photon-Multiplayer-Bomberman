using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterMovement : MonoBehaviour
{
    public CharacterController controller;
    public float movementSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W))
            direction = Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            direction = Vector3.back;
        if (Input.GetKey(KeyCode.A))
            direction = Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction = Vector3.right;

        controller.Move(direction * movementSpeed * 0.1f);
        if(direction.magnitude > 0)
            GetComponent<Transform>().localRotation = Quaternion.LookRotation(direction);
    }
}
