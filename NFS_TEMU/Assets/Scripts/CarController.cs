using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform[] wheels;
    [SerializeField] private Transform[] frontWheels;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float steeringSpeed = 45f;
    [SerializeField] private float wheelRotationSpeed = 360f;
    private float currentSpeed;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * (maxSpeed * Time.deltaTime), ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * (maxSpeed * Time.deltaTime), ForceMode.VelocityChange);
        }
        
        var horizontal = Input.GetAxisRaw("Horizontal");
        var normalizedSpeed = rb.linearVelocity.magnitude * 2f / rb.maxLinearVelocity;
        var carRotation = horizontal * steeringSpeed * normalizedSpeed;
        transform.Rotate(0, carRotation * Time.deltaTime, 0);

        foreach (var wheel in wheels)
        {
            var degrees = Vector3.Dot(transform.forward * maxSpeed, rb.linearVelocity);
            
            wheel.Rotate(degrees * wheelRotationSpeed * Time.deltaTime, 0f, 0f);
        }

        foreach (var wheel in frontWheels)
        {
            var targetRotation = Quaternion.Euler(wheel.localRotation.eulerAngles.x,
                horizontal * 45f, 
                wheel.localRotation.eulerAngles.z);
            wheel.localRotation = Quaternion.Lerp(wheel.localRotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}