// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ObjectPool))]

public class Turret : MonoBehaviour
{
    [SerializeField] private List<Transform> turretBarrels;

    [SerializeField] private TurretData turretData;

    // [SerializeField] private GameObject bulletPrefab;
    // [SerializeField] private float reloadDelay = 1;

    private bool canShoot = true;
    private Collider2D[] tankColliders;
    [SerializeField] private float currentDelay = 0;
    
    private ObjectPool bulletPool;
    [SerializeField] private int bulletPoolCount = 10;

    public UnityEvent OnShoot;
    public UnityEvent<float> OnReloading, OnCantShoot;

    private void Awake(){
        tankColliders = GetComponentsInParent<Collider2D>();
        bulletPool = GetComponent<ObjectPool>();
    }

    private void Start(){
        bulletPool.Initialize(turretData.bulletPrefab, bulletPoolCount);
        OnReloading?.Invoke(currentDelay);
    }

    private void Update(){
        if (canShoot == false){
            currentDelay -= Time.deltaTime;
            OnReloading?.Invoke(currentDelay);
            if (currentDelay <= 0){
                canShoot = true;
            }
        }
    }

    public void Shoot(){
        if(canShoot){
            canShoot = false;
            currentDelay = turretData.reloadDelay;

            foreach (var barrel in turretBarrels){
                //GameObject bullet = Instantiate(bulletPrefab);
                var hit = Physics2D.Raycast(barrel.position, barrel.up);
                // if (hit.collider != null)
                //     Debug.Log(hit.collider.name);
                    
                GameObject bullet = bulletPool.CreateObject(); 
                bullet.transform.position = barrel.position;
                bullet.transform.localRotation = barrel.rotation;
                bullet.GetComponent<Bullet>().Initialize(turretData.bulletData);
                foreach (var collider in tankColliders){
                    Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), collider);
                }
            }
            OnShoot?.Invoke();
            OnReloading.Invoke(currentDelay);
            OnCantShoot?.Invoke(currentDelay); 
        } else {
            OnCantShoot?.Invoke(turretData.reloadDelay); 
        }
    }
}
