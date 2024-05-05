using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TankMover : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public TankMovementData movementData;
    private Vector2 movementVector;
    // [SerializeField] private float maxSpeed = 100;
    // [SerializeField] private float rotationSpeed = 200; 
    // [SerializeField] private float acceleration = 100; 
    // [SerializeField] private float deacceleration = 100; 
    [SerializeField] private float currentSpeed = 0; 
    [SerializeField] private float currentForewardDirection = 1; 

    public UnityEvent<float> OnSpeedChange = new UnityEvent<float>();

    private void Awake(){
        rb2d = GetComponentInParent<Rigidbody2D>();
    }

    private void CalculateSpeed(Vector2 movementVector){
        if (Mathf.Abs(movementVector.y) > 0){
            currentSpeed += movementData.acceleration * Time.deltaTime;
        } else {
            currentSpeed -= movementData.deacceleration * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, movementData.maxSpeed);
    }

    private void FixedUpdate(){
        rb2d.velocity = (Vector2)transform.up * currentSpeed * currentForewardDirection * Time.fixedDeltaTime;
        rb2d.MoveRotation(transform.rotation * Quaternion.Euler(0 , 0, -movementVector.x * movementData.rotationSpeed * Time.fixedDeltaTime));
    }

    public void Move(Vector2 movementVector){
        this.movementVector = movementVector;
        CalculateSpeed(movementVector);
        OnSpeedChange?.Invoke(this.movementVector.magnitude);
        if (movementVector.y > 0)
            currentForewardDirection = 1;
        else if (movementVector.y < 0)
            currentForewardDirection = -1;
    }

}
