 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour{
    private Camera mainCamera;
    public UnityEvent OnShoot = new UnityEvent();
    public UnityEvent<Vector2> OnMoveBody = new UnityEvent<Vector2>();
    public UnityEvent<float> OnMoveTurret = new UnityEvent<float>();

    private void Awake(){
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update(){
        GetBodyMovement();
        if (Input.GetKey(KeyCode.Q)){
            GetTurretMovement(0.5f);
        } else if (Input.GetKey(KeyCode.E)){
            GetTurretMovement(-0.5f);
        }
        GetShootingInput(); 
    }

    private void GetBodyMovement(){
        if (Input.GetKeyDown(KeyCode.Space)){
            OnShoot?.Invoke();
        }
    }

    private void GetTurretMovement(float rotation){
        OnMoveTurret?.Invoke(rotation);
    }

    // private Vector2 GetMousePosition(){
    //     Vector3 mousePosition = Input.mousePosition;
    //     mousePosition.z = mainCamera.nearClipPlane;
    //     Vector2 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
    //     return mouseWorldPosition;
    // }

    private void GetShootingInput(){
        Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        OnMoveBody?.Invoke(movementVector.normalized);
    } 
}
