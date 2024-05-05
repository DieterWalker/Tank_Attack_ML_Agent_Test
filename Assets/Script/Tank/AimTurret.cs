using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTurret : MonoBehaviour{
    [SerializeField] private float turretRotationSpeed = 150;

    public void Aim(float pointerPosition){
        var rotationStep = turretRotationSpeed * pointerPosition * Time.deltaTime;

        transform.Rotate(Vector3.forward * rotationStep);
    }
}
