using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterMovement : MonoBehaviour
{
    //public CharacterController controller;
    public float movementSpeed = 1f;
    public float turnSmoothness = 0.1f;

    [HideInInspector]
    public float targetAngle;
    
    CharacterController controller;
    float turnSmoothVelocity;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            direction += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            direction += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            direction += Vector3.right;

        

        if (direction.magnitude >= 0.1f)
        {
            direction.Normalize();
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            controller.Move(direction * movementSpeed * Time.deltaTime);
        }

        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothness);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
